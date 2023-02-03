using UnityEngine;
using UnityEngine.UI;

public class CanvasUtil : MonoBehaviour
{
    static public Canvas canvas;
    static public CanvasScaler canvasScaler;
    void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvasScaler = GetComponent<CanvasScaler>();
    }
}