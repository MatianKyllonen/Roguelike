using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    public int damage = 10;               // Damage dealt by the projectile
    public float speed = 5f;             // Speed of the projectile
    public float homingStrength = 2f;    // How strongly the projectile homes toward the target
    public float homingDuration = 2f;    // How long the homing behavior lasts

    private Transform target;            // The current target (nearest player)
    private bool isHoming = true;        // Tracks whether the projectile is homing
    private Rigidbody2D rb;              // Reference to the Rigidbody2D
    private float homingTimer = 0f;      // Timer to track homing duration

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * speed; // Initial velocity in the forward direction
        Destroy(gameObject, 10f);          // Destroy after 10 seconds
    }

    private void Update()
    {
        homingTimer += Time.deltaTime;

        if (homingTimer >= homingDuration)
        {
            isHoming = false; // Disable homing after the specified duration
        }

        if (isHoming)
        {
            FindNearestPlayer();

            if (target != null)
            {
                // Calculate direction towards the target
                Vector2 direction = (target.position - transform.position).normalized;

                // Smoothly adjust the projectile's velocity to home in on the target
                Vector2 currentVelocity = rb.velocity;
                Vector2 desiredVelocity = direction * speed;

                // Apply homing logic with linear interpolation
                Vector2 newVelocity = Vector2.Lerp(currentVelocity, desiredVelocity, Time.deltaTime * homingStrength);
                rb.velocity = newVelocity;
            }
        }
    }

    private void FindNearestPlayer()
    {
        // Find all GameObjects tagged as "Player"
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        float closestDistance = Mathf.Infinity;
        Transform nearestPlayer = null;

        // Iterate through all players to find the nearest one
        foreach (GameObject player in players)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);

            if (distance < closestDistance && player.GetComponent<Movement>().knocked == false)
            {
                closestDistance = distance;
                nearestPlayer = player.transform;
            }
        }

        target = nearestPlayer; // Set the nearest player as the target
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the projectile hits a player
        if (collision.gameObject.tag == "Player")
        {
            // Apply damage to the player
            collision.gameObject.GetComponent<Movement>().TakeDamage(damage);

            // Destroy the projectile upon impact
            Destroy(gameObject);
        }
    }
}
