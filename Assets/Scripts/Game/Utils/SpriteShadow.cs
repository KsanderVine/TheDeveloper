using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteShadow : MonoBehaviour
{
    [SerializeField]
    protected Material shadowMaterial;

    [SerializeField]
    protected SpriteRenderer originalRenderer;
    protected SpriteRenderer shadowRenderer;

    [SerializeField]
    protected Vector3 offset = new Vector3(0.18f, 0.05f, 0f);
    [SerializeField]
    protected bool isStatic = true;

    
    void Start()
    {
        if (originalRenderer == null)
        {
            originalRenderer = GetComponent<SpriteRenderer>();
        }

        GameObject copyGameObject = new GameObject("Shadow");

        Transform shadowTransform = copyGameObject.transform;
        shadowTransform.parent = transform;
        shadowTransform.localScale = new Vector3(0.95f, 0.95f, 1);
        shadowTransform.localRotation = Quaternion.identity;
        shadowTransform.position = originalRenderer.transform.position;

        shadowRenderer = copyGameObject.AddComponent<SpriteRenderer>();
        shadowRenderer.material = shadowMaterial;

        shadowRenderer.sortingLayerName = originalRenderer.sortingLayerName;
        shadowRenderer.sortingOrder = -1000;

        shadowRenderer.sprite = originalRenderer.sprite;
        shadowRenderer.color = Color.black * 0.7f;

        shadowRenderer.sprite = originalRenderer.sprite;
        shadowRenderer.flipX = originalRenderer.flipX;
        shadowRenderer.transform.position = originalRenderer.transform.position - offset;

        shadowRenderer.color = new Color(0, 0, 0, originalRenderer.color.a * 0.7f);

        if (isStatic)
        {
            Destroy(this);
        }
    }

    void Reset()
    {
        if (originalRenderer == null)
        {
            originalRenderer = GetComponent<SpriteRenderer>();
        }
    }

    void LateUpdate()
    {
        shadowRenderer.color = new Color(0, 0, 0, originalRenderer.color.a * 0.7f);
        shadowRenderer.sprite = originalRenderer.sprite;

        shadowRenderer.flipX = originalRenderer.flipX;
        shadowRenderer.transform.rotation = originalRenderer.transform.rotation;
        shadowRenderer.transform.position = originalRenderer.transform.position - offset;
        shadowRenderer.enabled = originalRenderer.enabled;
    }
}