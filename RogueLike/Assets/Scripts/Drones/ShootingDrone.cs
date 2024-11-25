using UnityEngine;

public class ShootingDrone : MonoBehaviour
{
    public float shootingRange = 10f;
    public float fireRate = 1f;
    public float fireRateMultiplier = 1f;

    public float bulletForce = 20f;
    public float damage = 25f;

    public GameObject bulletPrefab;
    public Transform gunMuzzle;
    public LayerMask enemyLayer;

    private float nextFireTime = 0f;
    public int playerNumber;

    private void Start()
    {
        // Get the player number from the drone's target
        playerNumber = GetComponent<DroneBasic>().target.GetComponent<Movement>().playerNumber;
    }

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

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = Mathf.RoundToInt(damage);
            bulletScript.playerNumber = playerNumber; 
        }

        Vector2 direction = (target.transform.position - gunMuzzle.position).normalized;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * bulletForce;
        }

        Destroy(bullet, 2f);

        nextFireTime = Time.time + 1f / (fireRate * fireRateMultiplier);
    }
}
