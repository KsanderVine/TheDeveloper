using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimatedColor : MonoBehaviour {
    
    public Color TargetColor;
    public float DestroyDelay = -100;
    public float PreDelay = 0.6f;
    public float Speed;

    [SerializeField]
    protected SpriteRenderer spriteRenderer;

    public void FixedUpdate()
    {
        if(PreDelay > 0)
        {
            PreDelay -= Time.fixedDeltaTime;
            return;
        }

        if(DestroyDelay != -100)
        {
            DestroyDelay -= Time.fixedDeltaTime;
            if(DestroyDelay < 0)
            {
                Destroy(this);
                return;
            }
        }
        if (spriteRenderer == null) return;
        spriteRenderer.color = Color.Lerp(spriteRenderer.color, TargetColor, Speed * Time.fixedDeltaTime);
    }
}
