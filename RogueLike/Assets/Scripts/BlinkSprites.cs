using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlinkSprites : MonoBehaviour
{
    private Image[] images;  // Array to hold references to Image components
    private float fadeDuration = 1f;  // Duration for each fade (in and out)
    private float totalDuration = 5f;  // Total time for the fade in and out cycle
    private float timer = 0f;  // Timer to track the fade process

    // Start is called before the first frame update
    void Start()
    {
        // Get all child Image components attached to this object
        images = GetComponentsInChildren<Image>();
        StartCoroutine(FadeInAndOut());  // Start the fade-in and fade-out cycle
    }

    // Coroutine to handle the fade-in and fade-out effect
    private IEnumerator FadeInAndOut()
    {
        float elapsedTime = 0f;

        // Continue the effect for the set total duration
        while (elapsedTime < totalDuration)
        {
            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            // Fade in and out based on the elapsed time
            float lerpedAlpha = Mathf.PingPong(elapsedTime / fadeDuration, 1f);

            // Apply the fade effect to all Image components
            foreach (var image in images)
            {
                Color color = image.color;
                color.a = lerpedAlpha + 0.4f;
                image.color = color;
            }

            // Wait until the next frame
            yield return null;
        }

        // Ensure the alpha value is completely 0 before destroying the object
        while (elapsedTime < totalDuration + fadeDuration)
        {
            // Increment the elapsed time for the final fade-out
            elapsedTime += Time.deltaTime;

            // Gradually reduce the alpha to 0
            float fadeOutAlpha = Mathf.Lerp(1f, 0f, (elapsedTime - totalDuration) / fadeDuration);

            // Apply the fade-out effect to all Image components
            foreach (var image in images)
            {
                Color color = image.color;
                color.a = fadeOutAlpha;
                image.color = color;
            }

            // Wait until the next frame
            yield return null;
        }

        // After the fade-out is done, destroy the object
        Destroy(gameObject);
    }
}
