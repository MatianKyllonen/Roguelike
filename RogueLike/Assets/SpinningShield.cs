using UnityEngine;
using UnityEngine.UI;

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
    public Sprite brokenSprite;
    private Sprite defaulSprite;

    [Header("Rotation Settings")]
    public Transform target;
    public float rotationSpeed = 100f;

    public GameObject shieldBarUI;
    public Slider shieldSlider;
    void Start()
    {

        isActive = true;
        shield = GetComponent<BoxCollider2D>();
        shieldSprite = GetComponent<SpriteRenderer>();
        defaulSprite = shieldSprite.sprite;
    }
    private void Update()
    {
        if (target != null)
        {
            shieldCenter.gameObject.transform.RotateAround(target.position, Vector3.forward, rotationSpeed * Time.deltaTime);
        }

        if(!isActive)
        {
            if (0 < regenTimer)
            {
                regenTimer -= Time.deltaTime;
                shieldSlider.value = regenTimer / regenTime;
            }
            else
            {
                EnableShield();
            }

        }

        if (shieldBarUI != null)
        {
            // Set the position to match the shield or desired anchor, but reset rotation
            shieldBarUI.transform.position = shieldCenter.transform.position;
            shieldBarUI.transform.rotation = Quaternion.identity;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.layer == LayerMask.NameToLayer("EnemyProjectile") && isActive)
        {         
            health -= 1;
            CheckSprite();
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
        CheckSprite();
    }

    public void DisableShield()
    {
        isActive = false;
        shield.enabled = false;
        shieldSprite.enabled = false;
        regenTimer = regenTime;
        shieldBarUI.SetActive(true);
    }

    public void EnableShield()
    {    
        health = maxHealth;
        isActive = true;
        shield.enabled = true;
        shieldSprite.enabled = true;
        CheckSprite();
        shieldBarUI.SetActive(false);
    }

    private void CheckSprite()
    {
        if (health == 1 && maxHealth != 1)
            shieldSprite.sprite = brokenSprite;
        else
            shieldSprite.sprite = defaulSprite;
    }
}
