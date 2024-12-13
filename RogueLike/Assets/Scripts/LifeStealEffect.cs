using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeStealEffect : MonoBehaviour
{
    public float fadeDuration = 0.7f; // Duration of fade-out

    private void Start()
    {
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.color;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(originalColor.a, 0f, elapsedTime / fadeDuration);
                renderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null; // Wait for the next frame
            }
        }

        Destroy(gameObject); // Destroy the effect object
    }
}