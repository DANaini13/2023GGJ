using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScaler : MonoBehaviour
{
    public Vector3 scale;

    void Awake()
    {
        this.transform.localScale = scale;
    }

    void Update()
    {
        if (this.transform.localScale != scale)
            this.transform.localScale = scale;
    }
}
