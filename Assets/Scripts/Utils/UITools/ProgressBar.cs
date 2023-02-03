using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image filled_image;

    public void SetProgress(float progress)
    {
        filled_image.fillAmount = progress;
    }

    public void SetColor(Color color)
    {
        filled_image.color = color;
    }
}