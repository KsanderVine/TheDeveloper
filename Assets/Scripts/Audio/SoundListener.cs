using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundListener : MonoBehaviour
{
    public static SoundListener Instance;
    public void Awake()
    {
        Instance = this;
    }

    public Transform IistenerTransform;

    public static void SetPosition (Vector2 position)
    {
        if (Instance == null) return;
        Instance.transform.position = position;
    }

    public static void SetZooming (float zoomingLevel)
    {
        if (Instance == null) return;
        Instance.IistenerTransform.localPosition = new Vector3(0, 0, Mathf.Lerp(0, -2.5f, zoomingLevel));
    }
}
