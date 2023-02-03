using UnityEngine;
using System.Globalization;
using UnityEngine.UI;

public class UIShowFPS : MonoBehaviour
{
    private float m_LastUpdateShowTime = 0f;    //上一次更新帧率的时间;

    private float m_UpdateShowDeltaTime = 0.5f;//更新帧率的时间间隔;

    private int m_FrameUpdate = 0;//帧数;

    private float m_FPS = 0;

    public Text fpsText;

    void Awake()
    {
        Application.targetFrameRate = 200;
    }

    // Use this for initialization
    void Start()
    {
        m_LastUpdateShowTime = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        m_FrameUpdate++;
        if (Time.realtimeSinceStartup - m_LastUpdateShowTime >= m_UpdateShowDeltaTime)
        {
            m_FPS = m_FrameUpdate / (Time.realtimeSinceStartup - m_LastUpdateShowTime);
            m_FrameUpdate = 0;
            m_LastUpdateShowTime = Time.realtimeSinceStartup;
            fpsText.text = m_FPS.ToString(CultureInfo.InvariantCulture) ;
        }
    }


    public void OnNearChange(float near)
    {
        CameraUtil.MainCamera.nearClipPlane = near;
    }

    //void OnGUI()
    //{
    //    GUIStyle gs = new GUIStyle();
    //    gs.fontSize = 20;
    //    gs.normal.textColor = Color.white;
    //    GUI.Label(new Rect(400, 10, 100, 100), "FPS: " + m_FPS,gs);
    //}
}
