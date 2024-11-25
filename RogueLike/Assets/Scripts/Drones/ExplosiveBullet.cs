using UnityEngine;

public class ExplosiveBullet : MonoBehaviour
{
    public float explosionRadius = 5f;
    public float explosionDamage = 50f;
    public GameObject explosionEffect;
    public int playerNumber;

    private bool hasExploded = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasExploded && collision.gameObject.layer == 6)
        {
            hasExploded = true;

            BasicEnemy enemy = collision.gameObject.GetComponent<BasicEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage((int)explosionDamage);
                Gamemanager.instance.UpdatePlayerStats(playerNumber, 0, (int)explosionDamage, 0, 0);

                if (enemy.health <= 0)
                {
                    Gamemanager.instance.UpdatePlayerStats(playerNumber, 1, 0, 0, 0);
                }
            }

            Explode();
        }
    }

    void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D enemy in enemiesInRange)
        {
            if (enemy.gameObject.layer == 6)
            {
                BasicEnemy enemyScript = enemy.GetComponent<BasicEnemy>();
                if (enemyScript != null)
                {
                    enemyScript.TakeDamage((int)explosionDamage);
                    Gamemanager.instance.UpdatePlayerStats(playerNumber, 0, (int)explosionDamage, 0, 0);

                    if (enemyScript.health <= 0)
                    {
                        Gamemanager.instance.UpdatePlayerStats(playerNumber, 1, 0, 0, 0);
                    }
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
