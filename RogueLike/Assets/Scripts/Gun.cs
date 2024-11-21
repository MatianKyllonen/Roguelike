using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float shootingRange = 10f;
    public float fireRate = 1f;
    public float fireRateMultiplier = 1;

    public float bulletForce = 25f;

    public float damage = 50;
    public float damageMultiplier = 1;

    public GameObject bulletPrefab;
    public Transform gunMuzzle;
    public LayerMask enemyLayer;

    public SpriteRenderer gunSprite;
    private float nextFireTime = 0f;
    private AudioSource audioSoure;
    public AudioClip shotSound;

    private void Start()
    {
        audioSoure = GetComponent<AudioSource>();
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
                RotateGunTowardsEnemy(nearestEnemy);

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
        bullet.GetComponent<Bullet>().damage = Mathf.RoundToInt(damage * damageMultiplier);

        Destroy(bullet, 2f);

        audioSoure.PlayOneShot(shotSound, 0.2f);

        Vector2 direction = (target.transform.position - gunMuzzle.position).normalized;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * bulletForce;
        }

        nextFireTime = Time.time + 1f / (fireRate * fireRateMultiplier);
    }

    void RotateGunTowardsEnemy(GameObject target)
    {
        Vector2 direction = (target.transform.position - gunMuzzle.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        gunSprite.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        gunSprite.transform.position = transform.position + (Vector3)(Quaternion.Euler(0, 0, angle) * Vector2.right * 0.5f);
        gunSprite.transform.rotation = Quaternion.Euler(0, 0, angle);

        if (direction.x < 0)
        {
            gunSprite.transform.localScale = new Vector3(1, -1, 1);  
        }
        else
        {
            gunSprite.transform.localScale = new Vector3(1, 1, 1);  
        }
    }
}
