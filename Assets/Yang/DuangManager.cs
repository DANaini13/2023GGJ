using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DuangManager : MonoBehaviour
{
    [Header("手")]
    public SpriteRenderer hand;
    [Header("植物")]
    public Transform plantParent;
    [Header("手+植物")]
    public Transform handParent;
    [Header("相机，这两个不能是同一个go")]
    public Transform cam;
    public Transform camParent;
    [Header("土地")]
    public Transform earth;
    [Header("太阳")]
    public Transform sun;
    [Header("UI父对象")]
    public Transform commentParent;
    public Transform commentSingleParent;
    [Header("音效播放")]
    public AudioSource audioSource;
    public AudioClip commentSfx;

    [Header("动画参数，勿动")]
    public float maxShakerStrength = 0.7f;
    public float camShakerThreshold = 0.50f;
    public float mediumThreshold = 0.33f;
    public float heavyThreshold = 0.66f;
    public float handSpriteThreshold = 0.33f;
    public float camZoomDis = 8f;
    public Color addColor;

    [Header("图片引用")]
    public Sprite handNormalSp;
    public Sprite handHoldSp;

    [Header("粒子引用")]
    public ParticleSystem psLight;
    public ParticleSystem psMedium;
    public ParticleSystem psHeavy;
    public ParticleSystem psRelax;
    public ParticleSystem psFinish;

    [Header("UI引用")]
    public CommentBar commentBar;
    public CommentBar commentSingleBar;

    private float camOriSize;
    private Vector3 camOriPos;
    private Shaker camShaker;
    private Shaker handShaker;
    private Vector3 sunOriPos;
    private Vector3 earthOriPos;
    private Material handMat;
    private SpriteRenderer plantRenderer;
    private Material plantMat;

    void Awake()
    {
        camOriSize = Camera.main.orthographicSize;
        camOriPos = camParent.transform.position;
        camShaker = cam.gameObject.AddComponent<Shaker>();
        handShaker = hand.gameObject.AddComponent<Shaker>();

        earthOriPos = earth.transform.position;
        sunOriPos = sun.transform.position;

        handMat = hand.material;
    }

    bool BindPlantMat()
    {
        if (plantMat && plantRenderer) return true;
        if (plantParent.childCount == 0) return false;
        plantRenderer = plantParent.GetChild(0).GetComponent<SpriteRenderer>();
        plantMat = plantRenderer.material;
        return true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            Pulling(0f);
        if (Input.GetKeyDown(KeyCode.X))
            Pulling(25f);
        if (Input.GetKeyDown(KeyCode.C))
            Pulling(50f);
        if (Input.GetKeyDown(KeyCode.V))
            Pulling(75f);
        if (Input.GetKeyDown(KeyCode.B))
            Pulling(100f);
        if (Input.GetKeyDown(KeyCode.A))
            Relax();
        if (Input.GetKeyDown(KeyCode.S))
            Finish(new string[5]);
        if (Input.GetKeyDown(KeyCode.Q))
            Restore();
    }

    //正在拔（0-100）
    public void Pulling(float strength)
    {
        //修正
        if (strength < 0) strength = 0f;
        else if (strength > 1f) strength = 1f;

        //手部震动
        float shakerStr = strength * maxShakerStrength;
        handShaker.Shaking(shakerStr, 99999f);

        //镜头缩进
        if (strength <= 0f)
            Camera.main.orthographicSize = camOriSize;
        else
            Camera.main.orthographicSize = camOriSize - strength * camZoomDis;

        //镜头震动
        shakerStr = (strength - camShakerThreshold) * maxShakerStrength;

        if (strength < camShakerThreshold) shakerStr = 0f;
        if (shakerStr < 0f) shakerStr = 0f;

        camShaker.Shaking(shakerStr, 99999f);

        //-----粒子
        //被关停
        if (strength <= 0f)
        {
            psLight.Stop();
            psMedium.Stop();
            psHeavy.Stop();
        }
        else
        {
            //小力气
            if (strength > 0f)
                if (!psLight.isPlaying) psLight.Play();

            //中力气
            if (strength >= mediumThreshold)
                if (!psMedium.isPlaying) psMedium.Play();

            //大力气
            if (strength >= heavyThreshold)
                if (!psHeavy.isPlaying) psHeavy.Play();
        }

        //-----手部图片
        if (strength < handSpriteThreshold)
            hand.sprite = handNormalSp;
        else
            hand.sprite = handHoldSp;

        //图片变色
        handMat.SetColor("_AddColor", Color.Lerp(Color.black, addColor, strength));
        if (BindPlantMat())
            plantMat.SetColor("_AddColor", Color.Lerp(Color.black, addColor, strength));
    }

    //完成一个阶段
    public void Relax()
    {
        Pulling(0f);
        camShaker.Shaking(maxShakerStrength * 2f, 0.3f);
        psRelax.Play();
    }

    //彻底拔出
    private List<CommentBar> commentBarList = new List<CommentBar>();
    public void Finish()
    {
        Finish(new string[5]);
    }
    public void Finish(string[] comments)
    {
        //清空施加的力
        Pulling(0f);
        //紧握、震动、特效、减速
        hand.sprite = handHoldSp;
        camShaker.Shaking(maxShakerStrength * 3f, 0.5f);
        psFinish.Play();
        Time.timeScale = 0.15f;
        //之后恢复正常速度
        DOTween.To(v => { }, 0, 0, 0.2f).onComplete += () =>
        {
            earth.gameObject.SetActive(true);
            hand.sprite = handNormalSp;
            Time.timeScale = 1f;
        };

        //移动镜头
        camParent.transform.DOMove(camParent.position + Vector3.left * 12f, 0.3f).SetDelay(1f).SetEase(Ease.InCubic);
        //移开太阳和土地
        sun.DOMove(sunOriPos + Vector3.up * 50f, 0.3f).SetDelay(1f).SetEase(Ease.InCubic);
        earth.DOMove(earthOriPos + Vector3.down * 50f, 0.3f).SetDelay(1f).SetEase(Ease.InCubic);
        //放大植物
        handParent.DOScale(Vector3.one * 1.5f, 0.3f).SetDelay(1f).SetEase(Ease.InCubic);
        //显示评论
        DOTween.To(v => { }, 0, 0, 1.5f).onComplete += () =>
        {
            for (int i = 0; i < comments.Length; i++)
            {
                DOTween.To(v => { }, 0, 0, 0.15f * i).onComplete += () =>
                {
                    audioSource.PlayOneShot(commentSfx);
                };
                var bar = Instantiate(commentBar, commentParent);
                bar.Born(i * 0.15f, comments[i]);
                commentBarList.Add(bar);
            }
        };
    }

    public void Restore()
    {
        Pulling(0f);
        Camera.main.orthographicSize = camOriSize;
        camParent.transform.position = camOriPos;
        sun.position = sunOriPos;
        earth.position = earthOriPos;
        handParent.localScale = Vector3.one;
        foreach (var bar in commentBarList)
            Destroy(bar.gameObject);
        commentBarList.Clear();
    }

    public void ShowSingleComment(string str)
    {
        audioSource.PlayOneShot(commentSfx);
        var bar = Instantiate(commentSingleBar, commentSingleParent);
        bar.SingleBorn(str);
        commentBarList.Add(bar);
    }
}
