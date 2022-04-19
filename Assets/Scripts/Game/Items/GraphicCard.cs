using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicCard : Item
{
    public SoundSettings EmotionalDamageSound;

    protected override void OnTrigger(Collider2D collision)
    {
        Bug bug = collision.GetComponent<Bug>();
        if (bug != null)
        {
            bug.Die();
            return;
        }

        FullCover.Instance.CoverTime = 0.1f;
        SoundManager.PlayWorldSpace(EmotionalDamageSound, transform.position);
        GameRules.KillAllBugs();

        SpawnDebris();
        GameRules.ChangeResolveCoast(Coast);
        Destroy(gameObject);
    }
}
