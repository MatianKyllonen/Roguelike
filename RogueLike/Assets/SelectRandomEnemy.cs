using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectRandomEnemy : MonoBehaviour
{
    public List<GameObject> enemyPrefabs; // List of enemy prefabs to choose from        // Location where the enemy will spawn

    private void Start()
    {
        SpawnRandomEnemy();
    }
    // Spawn a random enemy from the list
    public void SpawnRandomEnemy()
    {
        // Check if the list has any prefabs
        if (enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("No enemy prefabs assigned!");
            return;
        }

        // Select a random enemy from the list
        int randomIndex = Random.Range(0, enemyPrefabs.Count);
        GameObject selectedEnemy = enemyPrefabs[randomIndex];

        // Spawn the selected enemy at the spawn point's position and rotation
        Instantiate(selectedEnemy, transform.position, Quaternion.identity);
    }
}
