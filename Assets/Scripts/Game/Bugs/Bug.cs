using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bug : Unit
{
    public Colorizer Colorizer;
    public Transform BugTransform;
    public SoundSettings DeathSound;
    public GameObject BugDebris;
    public float AngleSpeed = 0.9f;
    public int Score;

    private float _freezePhysicsTime;
    private Vector2 _targetValocity;
    private Vector2 _smoothVelocity;
    private float _deviationFactor;
    protected bool IsDead;

    public void Awake()
    {
        Colorizer.Red = GetRandomColor();
        Colorizer.Green = GetRandomColor();
        Colorizer.Blue = GetRandomColor();

        _deviationFactor = Random.Range(8f, 10f) * 0.1f;
        GameRules.ChangeBugsCounter(1);
    }

    private Color GetRandomColor ()
    {
        float r = Random.Range(0f, 255f) / 255f;
        float g = Random.Range(0f, 255f) / 255f;
        float b = Random.Range(0f, 255f) / 255f;
        return new Color(r, g, b, 1);
    }

    public void FixedUpdate()
    {
        if (IsDead)
            return;

        if (_freezePhysicsTime > 0)
        {
            UnitPhysics.simulated = false;
        }
        else
        {
            UnitPhysics.simulated = true;
            UnitPhysics.velocity = Vector3.Slerp(UnitPhysics.velocity, _smoothVelocity, Time.fixedDeltaTime * 2f);

            Vector2 vectorFrom = Vector3.zero;
            Vector2 vectorTo = UnitPhysics.velocity.normalized;
            Vector2 diff = vectorTo - vectorFrom;

            float sign = (vectorTo.y < vectorFrom.y) ? -1.0f : 1.0f;
            float angle = (Vector2.Angle(Vector2.right, diff) * sign) - 90;

            BugTransform.localEulerAngles = new Vector3(0, 0, angle);
        }
    }

    public void Update()
    {
        if (IsDead)
            return;

        _freezePhysicsTime -= Time.deltaTime;

        Vector3 randomOffset = new Vector2(0, 0.5f);
        _targetValocity = (Character.Active.transform.position + randomOffset) - transform.position;
        _smoothVelocity = Vector3.Slerp(_smoothVelocity, _targetValocity, Time.deltaTime * (AngleSpeed * _deviationFactor));
        _smoothVelocity = Vector2.ClampMagnitude(_smoothVelocity, (MotionSpeed * _deviationFactor));
        Ability();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Character.Active.gameObject)
        {
            Character.Active.DieAndRestart();
        }
    }

    public void SetPhysicsFreezing(float time)
    {
        _freezePhysicsTime = time;
    }

    public void PushImpulse (Vector2 direction)
    {
        UnitPhysics.velocity = direction;
        _smoothVelocity = direction;
    }

    public void RemoveByGame()
    {
        if (IsDead)
        {
            return;
        }
        IsDead = true;
        Destroy(gameObject);
        GameRules.ChangeBugsCounter(-1);
    }

    public virtual void Die ()
    {
        if (IsDead)
            return;

        IsDead = true;
        GameObject debris = GameObject.Instantiate(BugDebris);
        debris.transform.position = transform.position;
        debris.transform.rotation = transform.rotation;

        SoundManager.PlayWorldSpace(DeathSound, transform.position);

        GameRules.ChangeBugsResolvedCounter(1);
        GameRules.ChangeScore(Score);
        GameRules.ChangeBugsCounter(-1);
        Destroy(gameObject, 0.15f);
    }

    protected virtual void Ability() { }
}
