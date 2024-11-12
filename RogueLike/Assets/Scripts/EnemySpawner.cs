using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject enemyPrefab;      // The single enemy prefab to spawn
    [SerializeField] private TextMeshProUGUI waveCounter; // UI Text to display the wave count
    [SerializeField] private List<Transform> spawnPoints; // List of potential spawn points

    [Header("Attributes")]
    [SerializeField] private int baseEnemiesPerWave = 4;  // Number of enemies to spawn per wave
    [SerializeField] private float spawnInterval = 1f;    // Time between enemy spawns
    [SerializeField] private float timeBetweenWaves = 5f; // Time between each wave

    private int currentWave = 0;            // The current wave number
    private int enemiesLeftToSpawn;         // Number of enemies left to spawn in the current wave
    private int enemiesAlive = 0;           // Number of enemies currently alive
    private bool isSpawning = false;        // Whether the spawner is actively spawning enemies

    private void Start()
    {
        StartCoroutine(StartWave()); // Start the first wave
    }

    private void Update()
    {
        // If enemies are still alive or spawning, don't end the wave
        if (enemiesAlive == 0 && !isSpawning)
        {
            StartCoroutine(StartWave()); // Start the next wave
        }
    }

    private IEnumerator StartWave()
    {
        currentWave += 1;
        isSpawning = true;
        enemiesLeftToSpawn = baseEnemiesPerWave + currentWave * 2; // Increase enemies per wave based on wave count
        waveCounter.text = "Wave: " + currentWave;                 // Update wave counter UI
        yield return new WaitForSeconds(timeBetweenWaves);         // Wait before starting the wave

        while (enemiesLeftToSpawn > 0)
        {
            SpawnEnemy();               // Spawn an enemy
            enemiesLeftToSpawn--;       // Decrement count of enemies left to spawn
            enemiesAlive++;             // Increase count of enemies alive
            yield return new WaitForSeconds(spawnInterval); // Wait before spawning the next enemy
        }

        isSpawning = false;             // Wave spawning finished
    }

    private void SpawnEnemy()
    {
        // Select a random spawn point from the list
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        // Instantiate the enemy at the random spawn point
        Instantiate(enemyPrefab, randomSpawnPoint.position, Quaternion.identity);
    }

    public void EnemyDestroyed()
    {
        enemiesAlive--;                 // Reduce enemies alive count when an enemy is destroyed
    }
}
