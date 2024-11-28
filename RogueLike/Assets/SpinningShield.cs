using UnityEngine;

public class SpinningShield : MonoBehaviour
{
    [Header("Shield Stats")]
    public int maxHealth = 1;
    public float regenTime = 30f;
    private float regenTimer = 0;

    private int health = 1;
    private bool isActive = true;

    private BoxCollider2D shield;
    private SpriteRenderer shieldSprite;

    public GameObject shieldCenter;
    //public Sprite brokenSprite;

    [Header("Rotation Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float rotationSpeed = 100f;
    void Start()
    {
        isActive = true;
        shield = GetComponent<BoxCollider2D>();
        shieldSprite = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (target != null)
        {
            shieldCenter.gameObject.transform.RotateAround(target.position, Vector3.forward, rotationSpeed * Time.deltaTime);
        }

        if(!isActive)
        {
            if(0 < regenTimer)
            {
                regenTimer -= Time.deltaTime;
            }
            else
            {
                EnableShield();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.layer == LayerMask.NameToLayer("EnemyProjectile") && isActive)
        {         
            health -= 1;
            Debug.Log(health);
            Destroy(collision.gameObject);
            if (health <= 0)
            {
                DisableShield();
            }
        }
    }

    public void IncreaseHealth(int amount)
    {
        maxHealth += amount;
        health = maxHealth;
    }

    public void DisableShield()
    {
        isActive = false;
        shield.enabled = false;
        shieldSprite.enabled = false;
        regenTimer = regenTime;
    }

    public void EnableShield()
    {
        health = maxHealth;
        isActive = true;
        shield.enabled = true;
        shieldSprite.enabled = true;
    }
}
