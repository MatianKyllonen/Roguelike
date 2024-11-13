using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{

    public int maxHealth = 100;
    int health = 0;

    public float moveSpeed = 5f;
    public int playerNumber = 1;

    private Vector2 movementInput;
    private float horizontalMove;
    private float verticalMove;
    private SpriteRenderer spriteRenderer;
    Rigidbody2D rb;

    public Slider healthbar;
    public TextMeshProUGUI healthCount;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = maxHealth;
    }

    void Update()
    {


        horizontalMove = Input.GetAxis("Horizontal" + playerNumber);
        verticalMove = Input.GetAxis("Vertical" + playerNumber);

        Vector2 movement = new Vector2(horizontalMove, verticalMove).normalized;
        rb.velocity = movement * moveSpeed;


        if (horizontalMove != 0)
        {
            spriteRenderer.flipX = horizontalMove < 0;
        }

        movementInput.Normalize();

        MovePlayer(movementInput);
    }

    void MovePlayer(Vector2 movementInput)
    {
        transform.Translate(movementInput * moveSpeed * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        CalculateHealth();

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void CalculateHealth()
    {
        healthCount.text = health.ToString() + "/" + maxHealth.ToString();
        healthbar.value = ((float)health / (float)maxHealth);
        

    }
}
