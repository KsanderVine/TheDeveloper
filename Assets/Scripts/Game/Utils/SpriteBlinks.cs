using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteBlinks : MonoBehaviour
{
    [SerializeField]
    protected SpriteRenderer originalRenderer;
    [SerializeField]
    protected SpriteRenderer blinkRenderer;

    public bool IsBlinking { get; set; }

    public void Update()
    {
        if (IsBlinking)
        {
            blinkRenderer.flipX = originalRenderer.flipX;
            blinkRenderer.sprite = originalRenderer.sprite;
            blinkRenderer.enabled = Mathf.FloorToInt(Time.time * 10f) % 2f == 0;
            blinkRenderer.transform.rotation = originalRenderer.transform.rotation;
        }
        else
        {
            blinkRenderer.enabled = false;
        }
    }
}
