using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteTrail : MonoBehaviour
{
    public Sprite[] Variants;

    [SerializeField]
    protected SpriteRenderer rendererPrefab;

    private float _time = 0.05f;
    private float _delay = 0.05f;
    private int _bloodCount = 5;

    public void Update()
    {
        _delay -= Time.deltaTime;

        if(_delay <= 0)
        {
            _delay = _time;
            _bloodCount -= 1;

            if(_bloodCount < 0)
            {
                Destroy(this);
                return;
            } 

            SpriteRenderer bloodTrail = SpriteRenderer.Instantiate(rendererPrefab);
            bloodTrail.transform.position = this.transform.position;
            bloodTrail.sprite = Variants[Random.Range(0, Variants.Length)];
        }
    }
}
