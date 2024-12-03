using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemy : MonoBehaviour
{
    public float shootingRange = 10f;           // Range at which the enemy will shoot
    public float fireRate = 1f;
    public float projectileSpeed = 7f;
    public GameObject projectilePrefab;         // The projectile prefab to shoot
    private float nextFireTime = 0f;            // Time at which the enemy is allowed to fire

    private UpgradeManager upgradeManager;
    private Transform targetPlayer;             // The nearest player
    private SpriteRenderer spriteRenderer;      // For flipping the sprite based on player position

    void Start()
    {
        upgradeManager = FindFirstObjectByType<UpgradeManager>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (upgradeManager.shopOpen == true)
        {
            return;
        }

        // Find all players
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0)
            return;

        // Find the closest player
        float closestDistance = Mathf.Infinity;
        GameObject nearestPlayer = null;

        foreach (GameObject player in players)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer < closestDistance && !player.gameObject.GetComponent<Movement>().knocked)
            {
                closestDistance = distanceToPlayer;
                nearestPlayer = player;
            }
        }

        // If within shooting range, stop moving and shoot
        if (nearestPlayer != null)
        {
            targetPlayer = nearestPlayer.transform;

            // If within range, stop moving and start shooting
            if (closestDistance <= shootingRange)
            {
                ShootAtPlayer();
            }
            else
            {
                // Optionally, you could add movement here if you want to move towards the player at some range
            }

            // Flip the sprite to face the player
            FlipSprite();
        }
    }

    void ShootAtPlayer()
    {
        if (Time.time > nextFireTime && projectilePrefab != null)
        {
            // Fire the projectile
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            Destroy(projectile, 10);
            // Get the direction towards the player
            Vector2 direction = (targetPlayer.position - transform.position).normalized;

            // Set the velocity of the projectile to move towards the player
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * projectileSpeed;  // Adjust projectile speed as needed
            }

            // Make the projectile face the player by rotating it
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;  // Get angle in degrees
            projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));  // Set the rotation

            // Reset the fire rate timer
            nextFireTime = Time.time + 1f / fireRate;
        }
    }


    // Flip the sprite to face the player based on their position
    void FlipSprite()
    {
        if (targetPlayer == null)
            return;

        if (targetPlayer.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;  // Flip sprite to the left
        }
        else
        {
            spriteRenderer.flipX = false;  // Flip sprite to the right
        }
    }
}
