using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float shootingRange = 10f;          // Range at which the gun can shoot
    public float fireRate = 1f;                // Time between each shot
    public float bulletForce = 25f;
    public GameObject bulletPrefab;            // The bullet or projectile to shoot
    public Transform gunMuzzle;                // The point from where the bullet will be shot
    public LayerMask enemyLayer;               // Layer that represents enemies (set this in the inspector)

    public SpriteRenderer gunSprite;
    private float nextFireTime = 0f;           // Time to control firing rate

    void Update()
    {
        // If it's time to fire and the player presses the fire button
        if (Time.time > nextFireTime)
        {
            // Find the nearest enemy within range
            GameObject nearestEnemy = FindNearestEnemy();

            if (nearestEnemy != null)
            {
                // Rotate the gun towards the nearest enemy
                RotateGunTowardsEnemy(nearestEnemy);

                // Shoot at the nearest enemy
                Shoot(nearestEnemy);
            }
        }
    }

    GameObject FindNearestEnemy()
    {
        // Find all enemies in range using a collider overlap or Physics checks
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, shootingRange, enemyLayer);

        GameObject nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider2D enemy in enemiesInRange)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy.gameObject;
            }
        }

        return nearestEnemy;
    }

    void Shoot(GameObject target)
    {
        // Create the bullet instance
        GameObject bullet = Instantiate(bulletPrefab, gunMuzzle.position, Quaternion.identity);
        Destroy(bullet, 2f);

        // Calculate direction towards the target
        Vector2 direction = (target.transform.position - gunMuzzle.position).normalized;

        // Set bullet velocity or direction (depending on the method you're using)
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * bulletForce;  // Bullet speed (adjust as needed)
        }

        // Update the next fire time based on fire rate
        nextFireTime = Time.time + 1f / fireRate;
    }

    // Rotates the gun towards the nearest enemy
    void RotateGunTowardsEnemy(GameObject target)
    {
        Vector2 direction = (target.transform.position - gunMuzzle.position).normalized;

        // Calculate the angle between the gun's current position and the target's position
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate the gun to face the target direction
        gunSprite.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
