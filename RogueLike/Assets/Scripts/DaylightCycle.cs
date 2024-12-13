using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DaylightCycle : MonoBehaviour
{
    private Light2D sunLight;

    [Header("Gradient Settings")]
    public Gradient colorGradient; // A Gradient that defines the color transition
    public float cycleDuration = 10; // Duration of a full cycle (seconds)

    private float timeElapsed = 0.0f;

    private void Start()
    {
        // Get the Light2D component attached to this GameObject
        sunLight = GetComponent<Light2D>();
        if (sunLight == null)
        {
            Debug.LogError("Light2D component not found!");
        }

        // Start the cycle in the middle of the gradient (normalizedTime = 0.5)
        timeElapsed = cycleDuration * 0.5f;  // Start the cycle halfway through
        sunLight.color = colorGradient.Evaluate(0.5f); // At time 0.5, use the middle color in the gradient
    }

    private void Update()
    {
        if (sunLight == null)
            return;

        // Increment elapsed time
        timeElapsed += Time.deltaTime;

        // Calculate the normalized time (0 to 1) based on the cycle duration
        float normalizedTime = (timeElapsed / cycleDuration) % 1f; // Loops from 0 to 1

        // Set the color based on the normalized time using the gradient
        sunLight.color = colorGradient.Evaluate(normalizedTime);
    }
}
