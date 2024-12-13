using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [Header("Movement Settings")]
    public float minSpeed = 1f; // Minimum movement speed
    public float maxSpeed = 3f; // Maximum movement speed

    [Header("Rotation Settings")]
    public float minRotationSpeed = 10f; // Minimum rotation speed
    public float maxRotationSpeed = 50f; // Maximum rotation speed

    [Header("Wrapping Settings")]
    public float wrapMargin = 0.1f; // Distance beyond the camera bounds where wrapping occurs

    private Vector2 direction; // Movement direction
    private float speed; // Current movement speed
    private float rotationSpeed; // Current rotation speed
    private Camera mainCamera; // Reference to the main camera
    private Renderer asteroidRenderer; // Renderer to check visibility

    void Start()
    {
        // Initialize random direction, speed, and rotation speed
        direction = Random.insideUnitCircle.normalized; // Random direction
        speed = Random.Range(minSpeed, maxSpeed); // Random speed within range
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed); // Random rotation speed within range

        // Get reference to the main camera and asteroid renderer
        mainCamera = Camera.main;
        asteroidRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // Move the asteroid
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // Rotate the asteroid
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Handle screen wrapping
        WrapAroundScreen();
    }

    private void WrapAroundScreen()
    {
        // Get the asteroid's position in viewport space (0 to 1)
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);

        // If the asteroid is outside the screen (with a margin), wrap it to the opposite side
        if (viewportPosition.x < -wrapMargin) viewportPosition.x = 1 + wrapMargin; // Wrap left
        else if (viewportPosition.x > 1 + wrapMargin) viewportPosition.x = -wrapMargin; // Wrap right

        if (viewportPosition.y < -wrapMargin) viewportPosition.y = 1 + wrapMargin; // Wrap bottom
        else if (viewportPosition.y > 1 + wrapMargin) viewportPosition.y = -wrapMargin; // Wrap top

        // Convert the viewport position back to world space
        transform.position = mainCamera.ViewportToWorldPoint(viewportPosition);
    }
}
