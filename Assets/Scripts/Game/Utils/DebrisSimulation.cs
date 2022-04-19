using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisSimulation : MonoBehaviour
{
    public Rigidbody2D DebrisPhysics;

    public float MinForce = 1f;
    public float MaxForce = 2f;
    public float AngleTorque = 0f;

    private float _speed;
    private Vector2 _direction;

    public void Awake()
    {
        _speed = Random.Range(MinForce, MaxForce);
        _direction = transform.localPosition;
    }

    public void Start()
    {
        DebrisPhysics.AddForce(_direction * _speed, ForceMode2D.Impulse);

        if (AngleTorque != 0f)
        {
            DebrisPhysics.AddTorque((Random.Range(0, 100) > 50 ? 1 : -1) * AngleTorque * _speed, ForceMode2D.Force);
        }
    }
}
