using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    
    public Transform player;

    
    public GameObject[] enemyPrefabs;
    public int maxEnemies = 20;
    public float spawnRadius = 15f;
    public float minDistanceFromPlayer = 5f;
    public float spawnInterval = 2f;

    
    public float difficultyIncreaseInterval = 30f; // A cada 30s aumenta a dificuldade
    public int additionalEnemiesPerInterval = 5;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private float nextSpawnTime;
    private float nextDifficultyIncrease;

    void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;
        nextDifficultyIncrease = Time.time + difficultyIncreaseInterval;
    }

    void Update()
    {
        if (player == null) return;

        // Spawna inimigos conforme o tempo
        if (Time.time >= nextSpawnTime)
        {
            TrySpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }

        // Aumenta a dificuldade com o tempo
        if (Time.time >= nextDifficultyIncrease)
        {
            maxEnemies += additionalEnemiesPerInterval;
            nextDifficultyIncrease = Time.time + difficultyIncreaseInterval;
        }

        CleanupNullEnemies();
    }

    void TrySpawnEnemy()
    {
        if (spawnedEnemies.Count >= maxEnemies) return;

        Vector3 spawnPos = GetValidSpawnPosition();

        if (spawnPos != Vector3.zero)
        {
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
            spawnedEnemies.Add(enemy);
        }
    }

    Vector3 GetValidSpawnPosition()
    {
        // Tenta achar uma posição válida perto do player
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized * Random.Range(minDistanceFromPlayer, spawnRadius);
            Vector3 candidate = player.position + new Vector3(randomDir.x, randomDir.y, 0);

            // Aqui você pode colocar uma verificação de colisão com o terreno, se quiser
            return candidate;
        }

        return Vector3.zero;
    }

    void CleanupNullEnemies()
    {
        spawnedEnemies.RemoveAll(e => e == null);
    }
}
