using UnityEngine;

public class ExplosiveShotDrone : MonoBehaviour
{
    public float shootingRange = 10f;
    public float fireRate = 1f;
    public float fireRateMultiplier = 1f;

    public float bulletForce = 20f;
    public float damage = 25f;
    public float damageMultiplier = 1f;
    public float explosionRadius = 5f;
    public float explosionDamage = 50f;

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
        // Instantiate the bullet at the gun's muzzle position
        GameObject bullet = Instantiate(bulletPrefab, gunMuzzle.position, Quaternion.identity);

        // Set up explosive bullet parameters if applicable
        ExplosiveBullet explosiveBullet = bullet.GetComponent<ExplosiveBullet>();
        if (explosiveBullet != null)
        {
            explosiveBullet.explosionRadius = explosionRadius;
            explosiveBullet.explosionDamage = explosionDamage;
            explosiveBullet.playerNumber = playerNumber; // Assign player number
        }

        // Set up regular bullet parameters
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = Mathf.RoundToInt(damage * damageMultiplier);
            bulletScript.playerNumber = playerNumber; // Assign player number
        }

        // Get direction towards the target
        Vector2 direction = (target.transform.position - gunMuzzle.position).normalized;

        // Get the Rigidbody2D component of the bullet and set its velocity
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * bulletForce;
        }

        // Calculate the angle to rotate the bullet towards the target direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Get angle in degrees

        // Rotate the bullet to face the target
        bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Destroy the bullet after 5 seconds
        Destroy(bullet, 5f);

        // Reset the fire rate timer
        nextFireTime = Time.time + 1f / (fireRate * fireRateMultiplier);
    }

}
