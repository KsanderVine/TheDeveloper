using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    public SpriteRenderer UnitRenderer;
    public Rigidbody2D UnitPhysics;
    public Animator UnitAnimator;

    public float MotionSpeed = 0.5f;

    public enum AnimatorState
    {
        Idle,
        Run,
        Jump,
        Fall,
        Read,
        Item,
        Throw,
        ThrowFall,
        Restart
    }

    protected AnimatorState _state;
    public AnimatorState State
    {
        set
        {
            if (UnitAnimator == null)
            {
                _state = value;
                return;
            }

            AnimatorState targetState = value;

            if (UnitAnimator.HasState(0, Animator.StringToHash(targetState.ToString())))
            {
                if (!IsAnimation(targetState.ToString()))
                {
                    UnitAnimator.Play(targetState.ToString(), 0, 0);
                }
            }
            else
            {
                if (!IsAnimation("Idle"))
                {
                    UnitAnimator.Play("Idle", 0, 0);
                }
            }

            _state = targetState;
        }
        get
        {
            return _state;
        }
    }

    protected void ForcedAnimation(AnimatorState animationState)
    {
        _state = animationState;
        UnitAnimator.Play(animationState.ToString(), 0, 0);
    }

    protected bool IsAnimation(string animation)
    {
        return UnitAnimator.GetCurrentAnimatorStateInfo(0).IsName(animation);
    }
}