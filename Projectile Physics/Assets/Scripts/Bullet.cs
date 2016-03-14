using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float hitpointsModifier = -10f;
    public bool modifierAffectedByVelocity = true;
    public float ricochetVelocityLossScale = 1f;
    public float destroyVelocity = 2f;

    [HideInInspector]
    public Vector3 velocity = Vector3.zero;
	[HideInInspector]
	public LayerMask collideMask;
    float destroyVelocitySqr;
    float startMagnitude;

    HitEffectManager hitEffectManager;

    void Start()
    {
        startMagnitude = velocity.magnitude;
        destroyVelocitySqr = Mathf.Pow(destroyVelocity, 2);
        hitEffectManager = FindObjectOfType<HitEffectManager>();
    }

    void FixedUpdate()
    {
        //Checks if something is blocking path
        RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(transform.position, velocity, out hit, velocity.magnitude * Time.fixedDeltaTime + 0.01f, collideMask))
        {
            float deltaTime = Time.fixedDeltaTime;
            Vector3 movement = velocity * deltaTime;
            float movementMagnitude = movement.magnitude;

			while (Physics.Raycast(transform.position, movement, out hit, movementMagnitude, collideMask))
            {
                //Debug line showing path to collision
                Debug.DrawLine(transform.position, hit.point, Color.red, Mathf.Infinity);

                Debug.DrawRay(hit.point, hit.normal, Color.green, Mathf.Infinity);

                //Debug line showing path from collision to unblocked position
                Debug.DrawRay(hit.point, movement.normalized * (movementMagnitude - hit.distance), Color.yellow, Mathf.Infinity);

                //Damage if damgeable.
                HealthManager healthManager = hit.transform.GetComponent<HealthManager>();
                if (healthManager)
                {
                    if (modifierAffectedByVelocity)
                    {
                        healthManager.ModifyHP(hitpointsModifier * (movementMagnitude / Time.fixedDeltaTime / startMagnitude));
                    }

                    else
                    {
                        healthManager.ModifyHP(hitpointsModifier);
                    }

                    hitEffectManager.CreateHitEffect(hit.point, hit.normal, "Blood", hit.transform);

                    Destroy(gameObject);
                }

                else
                {
                    hitEffectManager.CreateHitEffect(hit.point, hit.normal, "Stone", hit.transform);
                }

                //Time in seconds to go to collision
                float timeTravelled = hit.distance / (movementMagnitude / deltaTime);
                deltaTime -= timeTravelled;

                //Ricochet with velocity loss based on angle
                velocity = Vector3.Reflect(velocity, hit.normal) * Quaternion.Angle(Quaternion.FromToRotation(Vector3.up, -velocity), Quaternion.FromToRotation(Vector3.up, hit.normal)) / (180f * ricochetVelocityLossScale) + Physics.gravity * timeTravelled;

                //Destruction if speed < destroy velocity
                if (velocity.sqrMagnitude < destroyVelocitySqr)
                {
                    Destroy(gameObject);
                }

                movement = velocity * deltaTime;
                movementMagnitude = movement.magnitude;

                transform.position = hit.point;
            }

            velocity += Physics.gravity * deltaTime;
            movement = velocity * deltaTime;

            Debug.DrawRay(transform.position, movement, Color.magenta, Mathf.Infinity);

            transform.position += movement;
        }

        else
        {
            //Applies gravity
            velocity += Physics.gravity * Time.fixedDeltaTime;

            Vector3 distance = velocity * Time.fixedDeltaTime;

            //Debug line showing current position to next
            Debug.DrawRay(transform.position, distance, Color.blue, Mathf.Infinity);

            transform.position += distance;
        }

        //Bullet faces in direction of velocity
        transform.forward = velocity;
    }
}
