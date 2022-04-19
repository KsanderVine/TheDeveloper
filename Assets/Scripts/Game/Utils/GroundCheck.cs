using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public bool UseCircle;
    public LayerMask overlapMask;

    public Collider2D lastCollider { get; private set; }

    private float _groundTimer;
    public float groundTimer
    {
        get => _groundTimer;
        private set => _groundTimer = Mathf.Max(0, value);
    }
    public bool isGrounded { private set; get; }

    public void FixedUpdate()
    {
        if (!UseCircle)
        {
            lastCollider = Physics2D.OverlapPoint(this.transform.position + new Vector3(0, -0.1f, 0), overlapMask);
        }
        else
        {
            lastCollider = Physics2D.OverlapCircle(this.transform.position + new Vector3(0, -0.1f, 0), 0.1f, overlapMask);
        }

        isGrounded = lastCollider != null ? true : false;

        if (isGrounded)
        {
            groundTimer = 0.3f;
        }
        else
        {
            groundTimer -= Time.fixedDeltaTime;
        }
    }
}
