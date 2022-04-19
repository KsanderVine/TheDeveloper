using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBug : Bug
{
    public int HealthCount = 2;
    public float ImmortalTIme = 0.25f;
    public float TimeToSplit = 12f;

    public Bug ChildrenPrefab;
    public int ChildrenCount;
    public SoundSettings ShieldDamagedSound;

    public SpriteBlinks ImmortalBlinks;
    private float _immortalityTime;

    protected override void Ability()
    {
        TimeToSplit -= Time.deltaTime;
        _immortalityTime -= Time.deltaTime;
        ImmortalBlinks.IsBlinking = _immortalityTime > 0;

        if(TimeToSplit <= 0)
        {
            for(int i = 0; i < ChildrenCount; i++)
            {
                Bug bug = Bug.Instantiate(ChildrenPrefab);
                bug.transform.position = transform.position;
            }
            base.Die();
        }
    }

    public override void Die()
    {
        if (IsDead || _immortalityTime > 0)
            return;

        HealthCount--;
        if(HealthCount < 0)
        {
            base.Die();
        }
        else
        {
            SoundManager.PlayWorldSpace(ShieldDamagedSound, transform.position);
            _immortalityTime = ImmortalTIme;
        }
    }
}
