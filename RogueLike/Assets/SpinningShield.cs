using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpinningShield : MonoBehaviour
{
    [Header("Shield Stats")]
    public int maxHealth = 1;
    public float regenTime = 30f;
    private float regenTimer = 0;
    public int healingAmount = 5;

    private int health = 1;
    private bool isActive = true;

    private BoxCollider2D shield;
    private SpriteRenderer shieldSprite;

    public GameObject shieldCenter;
    public Sprite brokenSprite;
    private Sprite defaulSprite;
    private Movement player;

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
        player = target.GetComponent<Movement>();
    }

    private void Update()
    {
        if (target != null)
        {
            shieldCenter.gameObject.transform.RotateAround(target.position, Vector3.forward, rotationSpeed * Time.deltaTime);
        }

        if (health < maxHealth)
        {
            if (regenTimer > 0)
            {
                regenTimer -= Time.deltaTime;
                shieldSlider.value = regenTimer / regenTime;
            }
            else
            {
                // Heal only once per regen cycle
                if (health < maxHealth)
                {
                    RegenShield();
                }
            }
        }
        else
        {
            isActive = true; 
        }

        if (shieldBarUI != null)
        {
            // Set the position to match the shield or desired anchor, but reset rotation
            Vector3 barPosition = new Vector3(shieldCenter.transform.position.x, shieldCenter.transform.position.y + 1, shieldCenter.transform.position.z);
            shieldBarUI.transform.position = barPosition;
            shieldBarUI.transform.rotation = Quaternion.identity;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.layer == LayerMask.NameToLayer("EnemyProjectile") && isActive)
        {
            player.Heal(healingAmount);
            Gamemanager.instance.UpdatePlayerStats(player.playerNumber, 0, 0, 0, healingAmount);
            shieldBarUI.SetActive(true);
            health -= 1;
            if (regenTimer <= 0) // Start regen timer only if it's not already running
            {
                regenTimer = regenTime;
            }
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

    public void RegenShield()
    {
        // Heal by 1 point per cycle, not all at once
        if (health < maxHealth)
        {
            health += 1;
        }

        isActive = true;
        shield.enabled = true;
        shieldSprite.enabled = true;
        CheckSprite();

        if (health >= maxHealth)
        {
            shieldBarUI.SetActive(false); // Hide the shield bar when fully healed
        }
    }

    private void CheckSprite()
    {
        if (health == 1 && maxHealth != 1)
            shieldSprite.sprite = brokenSprite;
        else
            shieldSprite.sprite = defaulSprite;
    }
}
