using UnityEngine;
using System.Collections;

public class ParticleSystemDestroyer : MonoBehaviour
{
    ParticleSystem particleSystem;

    float timer;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        if (!particleSystem)
            Destroy(this);
    }

    void Update()
    {
        if(particleSystem.particleCount == 0)
        {
            timer += Time.deltaTime;

            if (timer > 2f)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            timer = 0f;
        }
    }
}
