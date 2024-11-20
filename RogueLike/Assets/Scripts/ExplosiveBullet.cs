using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBullet : MonoBehaviour
{
    public float explosionRadius = 5f;  // Radius of explosion
    public float explosionDamage = 50f; // Damage done by explosion
    public GameObject explosionEffect;  // Optional: Explosion effect prefab

    private bool hasExploded = false;   // Prevent multiple explosions

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasExploded && collision.gameObject.layer == 6)
        {
            hasExploded = true;


            collision.gameObject.GetComponent<BasicEnemy>().TakeDamage((int)explosionDamage);


            Explode();
        }
    }

    void Explode()
    {
        // Instantiate explosion effect (if any)
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Find all enemies within explosion radius
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D enemy in enemiesInRange)
        {
            if (enemy.gameObject.layer == 6) 
            {
                BasicEnemy enemyScript = enemy.GetComponent<BasicEnemy>();
                if (enemyScript != null)
                {
                    enemyScript.TakeDamage((int)explosionDamage);
                }
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
