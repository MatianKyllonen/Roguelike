using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasCloud : MonoBehaviour
{
    public float initialDamagePercentage = 5f;  // Initial damage percentage
    public float rampUpRate = 2f;               // Damage increase rate over time
    public float damageInterval = 1f;           // Time interval for damage application

    private Dictionary<GameObject, float> entitiesInCloud = new Dictionary<GameObject, float>();

    void Update()
    {
        // Iterate over all entities in the gas cloud
        foreach (var entity in new List<GameObject>(entitiesInCloud.Keys))
        {
            if (entity == null)
            {
                entitiesInCloud.Remove(entity);
                continue;
            }

            // Calculate the ramping up damage
            float elapsedDamageTime = entitiesInCloud[entity];
            float currentDamagePercentage = initialDamagePercentage + rampUpRate * (Time.time - elapsedDamageTime);

            // Deal damage at regular intervals
            if (Time.time >= elapsedDamageTime + damageInterval)
            {
                Movement health = entity.GetComponent<Movement>();  // Assuming Movement has a TakeDamage method

                if (health != null)
                {
                    int damage = Mathf.RoundToInt(health.maxHealth * (currentDamagePercentage / 100f));  // Calculate damage based on max health
                    health.TakeDamage(damage);
                    entitiesInCloud[entity] = Time.time;  // Update the last damage time
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If a player enters the gas cloud, start tracking their damage
        if (other.CompareTag("Player") && !entitiesInCloud.ContainsKey(other.gameObject))
        {
            entitiesInCloud[other.gameObject] = Time.time;  // Track when the player entered the cloud
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Remove the player from the tracking list when they exit the cloud
        if (entitiesInCloud.ContainsKey(other.gameObject))
        {
            entitiesInCloud.Remove(other.gameObject);
        }
    }
}
