using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    public float moveSpeed = 3f;  // Speed at which the enemy moves
    private Transform targetPlayer;  // The transform of the nearest player
    private EnemySpawner spawner;

    public int maxHealth = 100;
    int health = 0;

    // Start is called before the first frame update
    void Start()
    {
        spawner = FindFirstObjectByType<EnemySpawner>();
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        // Find all game objects tagged as "Player"
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // If there are no players, return
        if (players.Length == 0)
            return;

        // Find the closest player
        float closestDistance = Mathf.Infinity;
        GameObject nearestPlayer = null;

        foreach (GameObject player in players)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer < closestDistance)
            {
                closestDistance = distanceToPlayer;
                nearestPlayer = player;
            }
        }

        // If a nearest player was found, set the target to that player's transform
        if (nearestPlayer != null)
        {
            targetPlayer = nearestPlayer.transform;

            // Move the enemy towards the player
            MoveTowardsPlayer();
        }
    }

    // Moves the enemy towards the nearest player
    void MoveTowardsPlayer()
    {
        if (targetPlayer == null)
            return;

        // Calculate the direction from the enemy to the player
        Vector3 direction = (targetPlayer.position - transform.position).normalized;

        // Move the enemy towards the player
        transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, moveSpeed * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if(health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        spawner.EnemyDestroyed();
        Destroy(gameObject);
    }
}
