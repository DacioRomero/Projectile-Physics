using UnityEngine;
using System.Collections;

public class AmmoPack : MonoBehaviour {
    public int clips = 4;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Overlapping " + other.name);
        Gun gunScript = other.GetComponentInChildren<Gun>();

        if (gunScript)
        {
            gunScript.rounds += gunScript.magazineSize * clips;
            Destroy(gameObject);
        }
    }
}
