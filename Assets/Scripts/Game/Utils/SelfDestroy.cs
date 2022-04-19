using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    public float DestroyDelay = 1f;
    public ParticleSystem[] ParticleSystems;

    private float _delay;
    private bool _inited;

    public void Start ()
    {
        _delay = DestroyDelay;

        if (ParticleSystems != null)
        {
            for (int i = 0; i < ParticleSystems.Length; i++)
            {
                if (ParticleSystems[i] != null)
                {
                    ParticleSystems[i].loop = (_delay > 0.25f);
                    ParticleSystems[i].Play();
                }
            }
        }

        _inited = true;
    }

    public void Update ()
    {
        if (!_inited) return;

        _delay -= Time.deltaTime;

        if (ParticleSystems != null)
        {
            for(int i = 0; i < ParticleSystems.Length; i++)
            {
                if(ParticleSystems[i] != null)
                ParticleSystems[i].loop = (_delay > 1f);
            }
        }

        if(_delay < 0)
        {
            Destroy(gameObject);
        }
    }

    public void RestartTime()
    {
        _delay = DestroyDelay;
        if (ParticleSystems != null)
        {
            for (int i = 0; i < ParticleSystems.Length; i++)
            {
                if (ParticleSystems[i] != null)
                {
                    ParticleSystems[i].loop = (_delay > 0.25f);
                    ParticleSystems[i].Play();
                }
            }
        }
    }
}
