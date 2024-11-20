using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveShotDrone : MonoBehaviour
{
    public float shootingRange = 10f;
    public float fireRate = 1f;
    public float fireRateMultiplier = 1f;

    public float bulletForce = 20f;
    public float damage = 25f;
    public float damageMultiplier = 1f;
    public float explosionRadius = 5f;   // Radius of explosion
    public float explosionDamage = 50f;   // Damage done by the explosion
    public float explosionDelay = 0.5f;   // Delay before explosion occurs after impact

    public GameObject bulletPrefab;
    public Transform gunMuzzle;
    public LayerMask enemyLayer;

    private float nextFireTime = 0f;

    void Update()
    {
        if (FindFirstObjectByType<UpgradeManager>().shopOpen == true)
        {
            return;
        }

        if (Time.time > nextFireTime)
        {
            GameObject nearestEnemy = FindNearestEnemy();

            if (nearestEnemy != null)
            {
                Shoot(nearestEnemy);
            }
        }
    }

    GameObject FindNearestEnemy()
    {
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
        GameObject bullet = Instantiate(bulletPrefab, gunMuzzle.position, Quaternion.identity);

        // Get the ExplosiveBullet component from the bullet prefab
        ExplosiveBullet explosiveBullet = bullet.GetComponent<ExplosiveBullet>();
        if (explosiveBullet != null)
        {
            // Set the explosion damage and properties from the drone
            explosiveBullet.explosionRadius = explosionRadius;
            explosiveBullet.explosionDamage = explosionDamage;

            // Optionally, set the bullet damage if you want to apply initial damage to the first enemy hit
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.damage = Mathf.RoundToInt(damage * damageMultiplier);
            }
        }

        // Set direction towards the target
        Vector2 direction = (target.transform.position - gunMuzzle.position).normalized;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * bulletForce;
        }

        Destroy(bullet, 5f); // Destroy the bullet after 5 seconds if it hasn't exploded yet

        // Set the fire rate cooldown
        nextFireTime = Time.time + 1f / (fireRate * fireRateMultiplier);
    }
}
