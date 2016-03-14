using UnityEngine;
using System.Collections;

public class FadeDestroy : MonoBehaviour
{
    static Color invisibleColor = new Color(0, 0, 0, 0);

    [HideInInspector]
    public float fadeOutTime;

    Material material;
    Color originalColor;
    float fadeOutTimer = 0f;

    void Start()
    {
        if (GetComponent<Renderer>())
        {
            material = GetComponent<Renderer>().material;
            originalColor = material.color;
        }

        if(!material)
        {
            Destroy(this);
        }
    }

    void Update()
    {
        fadeOutTimer += Time.deltaTime;

        if (fadeOutTimer >= fadeOutTime)
        {
            Destroy(gameObject);
        }

        material.color = Color.Lerp(originalColor, invisibleColor, fadeOutTimer / fadeOutTime);
    }
}
