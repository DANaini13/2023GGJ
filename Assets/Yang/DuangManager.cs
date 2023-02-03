using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DuangManager : MonoBehaviour
{
    [Header("手")]
    public SpriteRenderer hand;
    [Header("相机")]
    public Transform cam;

    [Header("动画参数，勿动")]
    public float maxShakerStrength = 0.7f;
    public float camShakerThreshold = 50f;
    public float mediumThreshold = 33f;
    public float heavyThreshold = 66f;
    public float handSpriteThreshold = 33f;
    public float camZoomDis = 8f;

    [Header("图片引用")]
    public Sprite handNormalSp;
    public Sprite handHoldSp;

    [Header("粒子引用")]
    public ParticleSystem psLight;
    public ParticleSystem psMedium;
    public ParticleSystem psHeavy;
    public ParticleSystem psRelax;
    public ParticleSystem psFinish;

    private float camOriSize;
    private Shaker camShaker;
    private Shaker handShaker;

    void Awake()
    {
        camOriSize = Camera.main.orthographicSize;
        camShaker = cam.gameObject.AddComponent<Shaker>();
        handShaker = hand.gameObject.AddComponent<Shaker>();
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
            Finish();
    }

    //正在拔（0-100）
    public void Pulling(float strength)
    {
        //修正
        if (strength < 0) strength = 0f;
        else if (strength > 100f) strength = 100f;

        //手部震动
        float shakerStr = strength / 100.0f * maxShakerStrength;
        handShaker.Shaking(shakerStr, 99999f);

        //镜头缩进
        if (strength <= 0f)
            Camera.main.orthographicSize = camOriSize;
        else
            Camera.main.orthographicSize = camOriSize - strength / 100.0f * camZoomDis;

        //镜头震动
        shakerStr = (strength - camShakerThreshold) / 100.0f * maxShakerStrength;

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
    }

    //完成一个阶段
    public void Relax()
    {
        Pulling(0f);
        camShaker.Shaking(maxShakerStrength * 2f, 0.3f);
        psRelax.Play();
    }

    //彻底拔出
    public void Finish()
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
            hand.sprite = handNormalSp;
            Time.timeScale = 1f;
        };

        cam.transform.DOMove(cam.position + Vector3.up * 5f, 0.3f).SetDelay(1f).SetEase(Ease.InCubic);
    }
}
