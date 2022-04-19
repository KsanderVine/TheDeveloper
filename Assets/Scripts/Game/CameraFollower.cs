using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Character Character;
    public float Speed = 5f;
    public bool IsLookingForward = true;
    public float ForwardOffset = 4f;

    protected float shockTime = 0.1f;
    protected Vector2 shockPoint;

    protected const float positionZ = -10f;

    public void Update()
    {
        if(shockTime > 0)
        {
            shockTime -= Time.deltaTime;

            shockTime -= Time.deltaTime;
            if (Mathf.FloorToInt(Time.time) % 2f == 0)
            {
                Vector2 offset = new Vector2(Random.Range(-1f, 1f) * .1f, Random.Range(-1f, 1f) * .3f);
                Vector3 position = shockPoint + offset;
                position.z = positionZ;
                transform.position = position;
            }
            return;
        }

        Vector3 targetPosition = Character.transform.position;

        if(IsLookingForward)
        {
            Vector3 offset = (Character.IsLookingRight == true ? Vector3.right : Vector3.left);
            targetPosition += (offset * ForwardOffset);
        }

        Vector3 smoothPosition = Vector2.Lerp(transform.position, targetPosition, Time.deltaTime * Speed);
        smoothPosition.z = positionZ;

        transform.position = smoothPosition;
    }

    public void Shock (float time)
    {
        shockTime = time;
        shockPoint = transform.position;
    }
}
