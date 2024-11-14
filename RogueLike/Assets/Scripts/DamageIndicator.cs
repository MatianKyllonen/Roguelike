using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageIndicator : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float lifetime = 0.6f;
    public float minDist = 2f;
    public float maxDist = 3f;
    private Vector2 iniPos;
    private Vector2 targetPos;
    private float timer;

    void Start()
    {
        // Set initial position and random target position in 2D space
        iniPos = transform.position;

        // Generate a random direction in 2D by picking an angle and creating an offset
        float angle = Random.Range(0f, 360f);
        float dist = Random.Range(minDist, maxDist);

        // Calculate target position using direction and distance, in 2D
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;
        targetPos = iniPos + offset;

        // Start with the text fully transparent
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float fadeStart = lifetime / 2f;

        // Destroy the object after the lifetime has elapsed
        if (timer > lifetime)
        {
            Destroy(gameObject);
        }
        else
        {
            // Gradually fade out the text color during the second half of the lifetime
            if (timer > fadeStart)
            {
                text.color = Color.Lerp(text.color, Color.clear, (timer - fadeStart) / (lifetime - fadeStart));
            }

            // Move the indicator towards the target position with a slight curve effect
            float progress = Mathf.Sin(timer / lifetime * Mathf.PI); // Smooth effect with sine curve
            transform.position = Vector2.Lerp(iniPos, targetPos, progress);

            // Scale the indicator smoothly over time
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);
        }
    }

    // Set the text to display the damage amount
    public void SetDamageText(int damage)
    {
        text.text = "-" + damage.ToString();
    }
}
