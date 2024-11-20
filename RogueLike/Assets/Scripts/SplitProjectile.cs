using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitProjectile : MonoBehaviour
{
    public GameObject subProjectilePrefab;    // The smaller projectiles that spawn after splitting
    public float splitDelay = 1f;            // Time before the projectile splits
    public float subProjectileSpeed = 5f;    // Speed of the smaller projectiles
    public float splitAngle = 45f;           // Angle between each subprojectile

    private bool hasSplit = false;

    void Start()
    {
        // Initiate the split sequence
        Invoke(nameof(Split), splitDelay);
    }

    void Split()
    {
        if (hasSplit || subProjectilePrefab == null)
            return;

        hasSplit = true; // Prevent double splitting

        // The center of the main projectile
        Vector3 splitCenter = transform.position;  // This is the point from which the subprojectiles will spawn

        for (int i = 0; i < 8; i++) // Spawn 8 projectiles
        {
            // Calculate the angle for each subprojectile
            float angle = i * splitAngle - (2 * splitAngle); // Evenly distribute around the main direction
            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;

            // Spawn the subprojectile from the center of the current projectile
            GameObject subProjectile = Instantiate(subProjectilePrefab, splitCenter, Quaternion.identity);

            // Set the velocity of the subprojectile
            Rigidbody2D rb = subProjectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * subProjectileSpeed;
            }

            // Rotate the subprojectile to face its direction
            float rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            subProjectile.transform.rotation = Quaternion.Euler(0, 0, rotationAngle);

            // Destroy the subprojectile after a duration (optional)
            Destroy(subProjectile, 5f);
        }

        // Destroy the main projectile after splitting
        Destroy(gameObject);
    }
}
