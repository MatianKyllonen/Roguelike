using UnityEngine;
public class CameraFollow : MonoBehaviour
{
    public Transform target1;      // First target (object) to follow
    public Transform target2;      // Second target (object) to follow
    public float smoothSpeed = 0.125f; // Smoothing speed for movement
    public Vector3 offset;        // Optional offset to keep a fixed distance from midpoint

    void LateUpdate()
    {
        // Calculate the midpoint of both targets
        Vector3 midpoint = (target1.position + target2.position) / 2;

        // Set the desired camera position based on the midpoint and offset
        Vector3 desiredPosition = midpoint + offset;

        // Smoothly transition the camera position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update the camera position
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z); // Keep Z constant for 2D
    }
}