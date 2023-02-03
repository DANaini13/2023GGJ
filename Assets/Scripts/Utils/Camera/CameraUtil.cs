using UnityEngine;

public static class CameraUtil
{
    static private Camera main_camera = null;

    static public Camera MainCamera
    {
        get
        {
            if (main_camera == null)
            {
                main_camera = Camera.main;
            }
            return main_camera;
        }
    }

    static public Vector3 WorldToScreenPoint(Vector3 pos)
    {
        return MainCamera.WorldToScreenPoint(pos);
    }

    static public Vector3 WorldToViewportPoint(Vector3 pos)
    {
        return MainCamera.WorldToViewportPoint(pos);
    }
}