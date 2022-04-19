using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullCover : MonoBehaviour
{
    public static FullCover Instance;
    public void Awake()
    {
        Instance = this;
    }

    public Image Cover;
    public float CoverTime;
    public float SpeedRise;
    public float SpeedFall;

    public void Update()
    {
        CoverTime -= Time.deltaTime;
        Color color = Cover.color;

        float step = Time.deltaTime * (CoverTime > 0 ? SpeedRise : SpeedFall);
        color.a = Mathf.Clamp(color.a + step, 0f, 1f);
        Cover.color = color;
    }
}
