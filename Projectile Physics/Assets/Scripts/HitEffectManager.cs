using UnityEngine;
using System.Collections;

public class HitEffectManager : MonoBehaviour
{
    [System.Serializable]
    public class HitEffects
    {
        public string label;
        public ParticleSystem particleSystem;
		//[HideInInspector]
		public ParticleSystem _particleSystem;
        public GameObject[] holes;
        public float holeDestroyTime;
        public bool onlyBurst
        {
            get
            {
                return particleSystem.emissionRate == 0;
            }
        }

        public HitEffects(string _label, ParticleSystem _particleSystem)
        {
            label = _label;
            particleSystem = _particleSystem;
        }
    }

    public HitEffects[] hitEffects;

    public void CreateHitEffect(Vector3 point, Vector3 direction, string label, Transform parent)
    {
        Debug.Log("Creating hit effect");

        string l_label = label.ToLower();
        for (int i = 0; i < hitEffects.Length; i++)
        {
            if(hitEffects[i].label.ToLower() == l_label)
            {
                CreateHitEffect(point, direction, i, parent);
                break;
            }
        }
    }

    public void CreateHitEffect(Vector3 point, Vector3 direction, int hitEffectIndex, Transform parent)
    {
        Quaternion directionRotation = Quaternion.FromToRotation(Vector3.forward, direction);

        int index = 0;
		if (hitEffectIndex < hitEffects.Length && hitEffectIndex >= 0) {
			index = hitEffectIndex;
		}

        HitEffects hitEffect = hitEffects[index];
        
		ParticleSystem particleSystem;

		if (hitEffect.onlyBurst) {
			if (hitEffect._particleSystem != null) {
				particleSystem = hitEffect._particleSystem; 
			} else {
				particleSystem = hitEffect._particleSystem = Instantiate (hitEffect.particleSystem) as ParticleSystem;
			}
		}

		else {
			particleSystem = Instantiate (hitEffect.particleSystem) as ParticleSystem;
		}

		particleSystem.transform.position = point + direction * 0.01f;
		particleSystem.transform.rotation = directionRotation;
		particleSystem.Play ();

        if (hitEffect.holes.Length > 0)
        {
            Debug.Log("Creating hole");

            GameObject hole = Instantiate(hitEffect.holes[Random.Range(0, hitEffect.holes.Length)], point + direction * 0.01f, Quaternion.Inverse(directionRotation)) as GameObject;

            Transform holeParent = null;
            foreach(Transform child in parent.GetComponentsInChildren<Transform>())
            {
                if(child.name == "AntiScaler")
                {
                    holeParent = child;
                    break;
                }
            }

            if(!holeParent)
            {
                holeParent = new GameObject("AntiScaler").transform;
                holeParent.parent = parent;
                holeParent.localScale = new Vector3(1 / parent.localScale.x, 1 / parent.localScale.x, 1 / parent.localScale.z);
            }

            hole.transform.parent = holeParent;

            FadeDestroy holeFD = hole.GetComponent<FadeDestroy>();

            if(holeFD)
            {
                holeFD.fadeOutTime = hitEffect.holeDestroyTime;
            }

            else
            {
                Destroy(hole, hitEffect.holeDestroyTime);
            }
        }

        particleSystem.transform.parent = parent;
    }
}
