using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lurker : Bug
{
    public float MaxSpeed = 2f;
    public float MinSpeed = 1f;
    public float BoostTime = 1f;
    private float _boostDelay;

    protected override void Ability()
    {
        _boostDelay -= Time.deltaTime;
        if(_boostDelay <= 0)
        {
            _boostDelay = BoostTime;
            MotionSpeed = MaxSpeed;
        }

        MotionSpeed = Mathf.Clamp(MotionSpeed - Time.deltaTime * 5f, MinSpeed, MaxSpeed);
    }
}
