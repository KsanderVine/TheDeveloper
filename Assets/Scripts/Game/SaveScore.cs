using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveScore : MonoBehaviour
{
    public int SavedScore;
    public float DestroyTime = 3f;
    public SpriteBlinks SpriteBlinks;
    public SoundSettings SaveScoreSound;

    public void Update()
    {
        DestroyTime -= Time.deltaTime;
        SpriteBlinks.IsBlinking = DestroyTime < 1f;

        if(DestroyTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == Character.Active.gameObject)
        {
            Bug[] bugs = FindObjectsOfType<Bug>(true);
            for (int i = 0; i < bugs.Length; i++)
            {
                bugs[i].SetPhysicsFreezing(1f);
            }

            Character.Active.SetPhysicsFreezing(1f);
            Character.Active.SetAnimatorState(Unit.AnimatorState.Item, 1f, 1f);
            Character.Active.SetImmortality(3f);
            Character.Active.ShowSaveScoreDemo(1f);

            SoundManager.PlayWorldSpace(SaveScoreSound, transform.position);
            GameRules.ChangeScore(SavedScore);
            Destroy(gameObject);
        }
    }
}
