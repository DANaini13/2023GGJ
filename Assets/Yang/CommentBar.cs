using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CommentBar : MonoBehaviour
{
    public Text text;
    public void Born(float delay, string str)
    {
        text.text = str;
        this.transform.localScale = Vector3.zero;
        this.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InCubic).SetDelay(delay);
    }
    public void SingleBorn(string str)
    {
        text.text = str;
        this.transform.localScale = Vector3.one * 3f;
        this.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InCubic);
    }
}
