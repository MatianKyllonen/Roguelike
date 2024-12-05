using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 50;
    public int playerNumber; // Track which player fired this bullet
    public Movement player;
    public GameObject lifeStealEffect;
    public float lifeStealChance = 0f;

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
                    Gamemanager.instance.UpdatePlayerStats(playerNumber, 1, 0, 0, 0); 
                }

                if(lifeStealChance > 0)
                {
                    if(Random.Range(0, 100) < lifeStealChance)
                    {
                        if(lifeStealEffect != null)
                            Instantiate(lifeStealEffect, transform.position, Quaternion.identity);

                        int healingAmount = Mathf.RoundToInt(damage / 5);
                        player.Heal(healingAmount);
                        Gamemanager.instance.UpdatePlayerStats(playerNumber, 0, 0, 0, healingAmount); 
                        GetComponent<BoxCollider2D>().enabled = false;
                        GetComponent<SpriteRenderer>().enabled = false;
                    }

                }         
            }

            Destroy(gameObject);

        }    
    }



}
