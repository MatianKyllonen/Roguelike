using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingBulllet : MonoBehaviour
{
    public int damage = 50;
    public int playerNumber; // Track which player fired this bullet
    public float pierceChance = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            BasicEnemy enemy = collision.gameObject.GetComponent<BasicEnemy>();

            if (enemy != null)
            {
                // Apply damage
                enemy.TakeDamage(damage);

                // Update damage stats for the player
                Gamemanager.instance.UpdatePlayerStats(playerNumber, 0, damage, 0, 0);

                // Check if enemy is dead and update kill stats
                if (enemy.health <= 0)
                {
                    Gamemanager.instance.UpdatePlayerStats(playerNumber, 1, 0, 0, 0); // Add 1 to kills
                }
            }

            if(Random.Range(0, 100) > pierceChance)
                Destroy(gameObject); // Destroy bullet
        }
    }
}