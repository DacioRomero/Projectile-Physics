using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    private UnityEngine.AI.NavMeshAgent agent;

    private void Start ()
    {
        Respawn();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        StartCoroutine("UpdateNav", target);
    }

    IEnumerator UpdateNav()
    {
        while(target)
        {
            agent.destination = target.position;
            yield return new WaitForSeconds(1f / 60);
        }

        yield break;
    }

    public void Die()
    {
        Respawn();
    }

    private void Respawn()
    {
        transform.position = new Vector3(Random.Range(-128, 128), 1, Random.Range(-128, 128));
        gameObject.SendMessage("FullyHeal");
    }
}
