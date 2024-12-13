using System.Collections;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    private Vector3 originalPosition;

    private void Update()
    {
        // Update the original position every frame to account for camera movement.
        originalPosition = transform.localPosition;
    }

    public void TriggerShake(float duration = 0.2f, float magnitude = 0.1f)
    {
        StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Generate random offsets for the shake effect.
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            // Apply the offsets to the current camera position.
            transform.localPosition = new Vector3(originalPosition.x + offsetX, originalPosition.y + offsetY, originalPosition.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset to the updated original position.
        transform.localPosition = originalPosition;
    }
}
