using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public GameObject ItemDebris;
    public Rigidbody2D ItemPhysics;
    public SpriteRenderer ItemRenderer;
    public SoundSettings ItemPuffSound;
    public int Coast;
    public float Speed = 5f;
    public bool IsFacingByPhysics = true;
    protected Vector3 direction = Vector3.right;

    public void Throw (Vector3 direction)
    {
        Vector2 dir = Vector2.ClampMagnitude(direction * 10000f, 1f) * Speed;
        this.direction = Vector2.ClampMagnitude(direction * 10000f, 1f) * Speed;
        ItemPhysics.velocity = this.direction;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        OnTrigger(collision);
    }

    protected virtual void OnTrigger(Collider2D collision)
    {
        Bug bug = collision.GetComponent<Bug>();
        if (bug != null)
        {
            bug.Die();
            return;
        }

        SpawnDebris();
        GameRules.ChangeResolveCoast(Coast);
        Destroy(gameObject);
    }

    protected void FixedUpdate()
    {
        if (IsFacingByPhysics)
        {
            Vector2 vectorFrom = Vector3.zero;
            Vector2 vectorTo = ItemPhysics.velocity.normalized;
            Vector2 diff = vectorTo - vectorFrom;

            float sign = (vectorTo.y < vectorFrom.y) ? -1.0f : 1.0f;
            float angle = (Vector2.Angle(Vector2.right, diff) * sign) - 90;
            ItemRenderer.transform.localEulerAngles = new Vector3(0, 0, angle);
        }
    }

    protected virtual void SpawnDebris()
    {
        if (ItemDebris != null)
        {
            GameObject effect = GameObject.Instantiate(ItemDebris);
            effect.transform.position = transform.position;
            SoundManager.PlayWorldSpace(ItemPuffSound, transform.position);
        }
    }
}
