using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Shaker : MonoBehaviour
{
    private float strength;
    private float timer;
    private bool isShaking = false;

    private Vector3 oriPos;
    private Vector3 target;

    public void Shaking(float strength, float timer)
    {
        this.timer = timer;
        this.strength = strength;
    }

    void Awake()
    {
        oriPos = this.transform.localPosition;
        target = oriPos;
    }

    void Update()
    {
        if (strength <= 0f || timer <= 0f)
        {
            if (isShaking)
            {
                isShaking = false;
                this.transform.localPosition = oriPos;
            }
            return;
        }

        isShaking = true;
        timer -= Time.deltaTime;

        //重新找目标
        if (Vector3.Distance(this.transform.localPosition, target) <= 0.01f)
        {
            target = oriPos + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f) * strength;
        }
        //向目标点靠拢
        else
        {
            var delta = Time.deltaTime * 800f;
            if (delta > 1f) delta = 1f;
            this.transform.localPosition += (target - this.transform.localPosition) * 0.8f * delta;
        }
    }
}
