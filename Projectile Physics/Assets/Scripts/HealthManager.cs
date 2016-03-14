using UnityEngine;
using System.Collections;

public class HealthManager : MonoBehaviour
{
    public float maxHitpoints = 100f;
    [SerializeField]
    float _hitpoints = 100f;
    public float hitpoints
    {
        get
        {
            return _hitpoints;
        }
        set
        {
            _hitpoints = Mathf.Clamp(value, 0, maxHitpoints);
        }
    }

    public void ModifyHP(float modifier)
    {
        hitpoints += modifier;
        if (hitpoints == 0)
        {
            gameObject.SendMessage("Die");
        }
    }

    public void FullyHeal()
    {
        hitpoints = maxHitpoints;
    }
}
