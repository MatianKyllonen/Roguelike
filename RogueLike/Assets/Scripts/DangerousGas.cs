using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerousGas : MonoBehaviour
{
    public float initialDamagePercentage = 5f;
    public float rampUpRate = 2f;
    public float damageInterval = 1f;

    private Dictionary<GameObject, float> playersInGas = new Dictionary<GameObject, float>();

    void Update()
    {
        foreach (var player in new List<GameObject>(playersInGas.Keys))
        {
            if (player == null)
            {
                playersInGas.Remove(player);
                continue;
            }

            float elapsedDamageTime = playersInGas[player];
            float currentDamagePercentage = initialDamagePercentage + rampUpRate * (Time.time - elapsedDamageTime);

            if (Time.time >= elapsedDamageTime + damageInterval)
            {
                int damage = Mathf.RoundToInt(player.GetComponent<Movement>().maxHealth * (currentDamagePercentage / 100f));
                player.GetComponent<Movement>().TakeDamage(damage);
                playersInGas[player] = Time.time;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !playersInGas.ContainsKey(collision.gameObject))
        {
            playersInGas[collision.gameObject] = Time.time;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (playersInGas.ContainsKey(collision.gameObject))
        {
            playersInGas.Remove(collision.gameObject);
        }
    }
}
