using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Gun : MonoBehaviour
{
    public AudioClip gunShot;
    public GameObject bulletPrefab;
    public Transform emissionPoint;
    public float bulletSpeed = 64f;
    [Range(0f, 90f)]
    public float deviationAngle = 5f;
    public LayerMask collideMask;
    public int projectileCount = 1;
    public float fireSpeed = .5f;
    public float recoilMaxAngle = 30;
    public float recoilRecoverTime = .5f;
    public int magazineSize = 15;
    public float maxReloadSpeed = 2;
    public int rounds = 15 * 21;
    public Transform magazineObject;
    public AnimationCurve reloadAnimationHeight;
    public AudioClip magazineOut;
    public AudioClip magazineIn;
    public AudioClip magazineEmpty;
    public Text magazineDisplay;
    int magazine;
    bool reloading;
    float recoilAngle;
    float fireTimer;

    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    void Update()
    {
        Time.timeScale = Mathf.MoveTowards(Time.timeScale, Input.GetKey(KeyCode.Q) ? 0.5f : 1, Time.unscaledDeltaTime);
        audioSource.pitch = Time.timeScale;
        fireTimer += Time.deltaTime;
        RaycastHit hit = new RaycastHit();
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out hit))
        {
            float angle = Mathf.Atan2(emissionPoint.localPosition.y, hit.distance) * Mathf.Rad2Deg;
			transform.LookAt(hit.point);
            transform.Rotate(angle, 0, 0);
        }

        else
        {
            transform.localRotation = Quaternion.identity;
        }

		Debug.DrawRay (emissionPoint.position, emissionPoint.forward * 64, Color.red);

        recoilAngle = Mathf.MoveTowardsAngle(recoilAngle, 0, recoilMaxAngle / recoilRecoverTime * Time.deltaTime);
        transform.Rotate(-recoilAngle, 0, 0);

        if (!reloading)
        {
            if (Input.GetButtonDown("Fire1") && fireTimer > fireSpeed)
            {
                if (magazine != 0)
                {
                    if (audioSource && gunShot)
                    {
                        audioSource.PlayOneShot(gunShot);
                    }

                    for (int i = 0; i < projectileCount; i++)
                    {
                        if (magazine == 0)
                        {
                            break;
                        }
                        Bullet bullet = (Instantiate(bulletPrefab, emissionPoint.position, Quaternion.identity) as GameObject).GetComponent<Bullet>();
                        bullet.collideMask = collideMask;
                        Vector3 direction = emissionPoint.forward + Quaternion.Euler(0, 0, Random.Range(0f, 360f - float.MinValue)) * emissionPoint.up * Random.value * (deviationAngle / 180f);
                        direction.Normalize();
                        bullet.transform.forward = bullet.velocity = direction * bulletSpeed;
                        recoilAngle = recoilMaxAngle;
                        magazine -= 1;
                    }

                    fireTimer = 0;
                }

                else
                {
                    audioSource.PlayOneShot(magazineEmpty);
                }
            }

            else if (Input.GetKeyDown(KeyCode.R) && magazine < magazineSize)
            {
                StartCoroutine(Reload());
            }
        }

        if (Input.GetButton("Fire2"))
        {
            Camera.main.fieldOfView = 40;
        }
        else {
            Camera.main.fieldOfView = 90;
        }

        if(magazineDisplay)
        {
            magazineDisplay.text = magazine + " / " + magazineSize + "  " + rounds;
        }
    }

    IEnumerator Reload()
    {
        Debug.Log("Reloading");
        int roundsNeeded = magazineSize - magazine;
        if(roundsNeeded > rounds)
        {
            roundsNeeded = rounds;
            if(roundsNeeded == 0)
            {
                reloading = false;
                yield break;
            }
        }

        float reloadTime = (maxReloadSpeed + maxReloadSpeed * ((float)roundsNeeded / magazineSize)) / 2;
        float reloadTimer = reloadTime;
        Vector3 localPosition = magazineObject.localPosition;
        reloading = true;
        rounds -= roundsNeeded;
        audioSource.PlayOneShot(magazineOut);
        while(reloadTimer > 0)
        {
            magazineObject.localPosition = localPosition + magazineObject.parent.InverseTransformVector(-magazineObject.up) * reloadAnimationHeight.Evaluate(reloadTimer / reloadTime);
            reloadTimer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        audioSource.PlayOneShot(magazineIn);
        magazine += roundsNeeded;
        magazineObject.localPosition = localPosition;
        reloading = false;
    }
}
