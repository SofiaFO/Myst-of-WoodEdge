using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private Transform player;
    private TorusMap map;

    [Header("Enemy Types")]
    public GameObject[] enemyPrefabs;

    [Header("Spawn Settings")]
    public int initialMaxEnemies = 10;
    public float initialSpawnInterval = 2f;

    [Header("Progressão Temporal")]
    [Tooltip("A cada quantos segundos a dificuldade aumenta")]
    public float difficultyIncreaseInterval = 30f;

    [Tooltip("Quantos inimigos a mais spawnam a cada intervalo")]
    public int additionalEnemiesPerInterval = 5;

    [Tooltip("Redução no intervalo de spawn (em %)")]
    [Range(0f, 0.2f)]
    public float spawnIntervalReduction = 0.05f; // 5% mais rápido

    [Tooltip("Intervalo mínimo de spawn (para não ficar caótico)")]
    public float minSpawnInterval = 0.3f;

    [Tooltip("Inimigos máximos simultaneamente (evita lag)")]
    public int absoluteMaxEnemies = 200;

    [Header("Wave Settings")]
    [Tooltip("Tamanho inicial da wave")]
    public int initialWaveSize = 3;

    [Tooltip("Tamanho máximo da wave")]
    public int maxWaveSize = 10;

    [Tooltip("Aumenta wave size a cada X níveis de dificuldade")]
    public int waveSizeIncreaseFrequency = 3;

    [Header("Despawn Settings")]
    public float despawnDistanceX = 13f;
    public float despawnDistanceY = 13f;

    private float currentSpawnInterval;
    private int currentMaxEnemies;
    private int currentWaveSize;
    private float nextSpawn;
    private float nextDifficultyIncrease;
    private int currentEnemyTier = 0;
    private int difficultyLevel = 0;
    private List<GameObject> enemies = new List<GameObject>();
    private bool isSpawningEnabled = true;

    void Awake()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        map = TorusMap.Instance;
    }

    void Start()
    {
        currentMaxEnemies = initialMaxEnemies;
        currentSpawnInterval = initialSpawnInterval;
        currentWaveSize = initialWaveSize;

        nextSpawn = Time.time + currentSpawnInterval;
        nextDifficultyIncrease = Time.time + difficultyIncreaseInterval;

        Debug.Log($"🎮 Spawner iniciado | Max Enemies: {currentMaxEnemies} | Spawn Interval: {currentSpawnInterval}s");
    }

    void Update()
    {
        if (player == null) return;

        // SÓ SPAWNA SE ESTIVER HABILITADO
        if (isSpawningEnabled && Time.time >= nextSpawn && enemies.Count < currentMaxEnemies)
        {
            SpawnEnemy();
            nextSpawn = Time.time + currentSpawnInterval;
        }

        // SÓ AUMENTA DIFICULDADE SE ESTIVER HABILITADO
        if (isSpawningEnabled && Time.time >= nextDifficultyIncrease)
        {
            IncreaseDifficulty();
            nextDifficultyIncrease = Time.time + difficultyIncreaseInterval;
        }

        Cleanup();
    }

    // ================================
    // CONTROLE DE SPAWN
    // ================================

    /// <summary>
    /// Para o spawn de inimigos e limpa todos os inimigos existentes
    /// </summary>
    public void StopSpawning()
    {
        isSpawningEnabled = false;
        ClearAllEnemies();
        Debug.Log("Spawn de inimigos desativado!");
    }

    /// <summary>
    /// Reativa o spawn de inimigos
    /// </summary>
    public void StartSpawning()
    {
        isSpawningEnabled = true;
        nextSpawn = Time.time + currentSpawnInterval;
        Debug.Log("Spawn de inimigos reativado!");
    }

    /// <summary>
    /// Destrói todos os inimigos spawned
    /// </summary>
    public void ClearAllEnemies()
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i] != null)
            {
                Destroy(enemies[i]);
            }
        }
        enemies.Clear();
        Debug.Log("Todos os inimigos foram removidos!");
    }

    // ================================
    // AUMENTO DE DIFICULDADE
    // ================================
    void IncreaseDifficulty()
    {
        difficultyLevel++;

        // Aumenta número máximo de inimigos
        currentMaxEnemies += additionalEnemiesPerInterval;
        currentMaxEnemies = Mathf.Min(currentMaxEnemies, absoluteMaxEnemies);

        // Reduz intervalo de spawn (spawna mais rápido)
        currentSpawnInterval *= (1f - spawnIntervalReduction);
        currentSpawnInterval = Mathf.Max(currentSpawnInterval, minSpawnInterval);

        // Aumenta tamanho das waves periodicamente
        if (difficultyLevel % waveSizeIncreaseFrequency == 0)
        {
            currentWaveSize = Mathf.Min(currentWaveSize + 1, maxWaveSize);
        }

        // Desbloqueia novos tipos de inimigos
        if (currentEnemyTier < enemyPrefabs.Length - 1)
        {
            currentEnemyTier++;
            Debug.Log($"🔓 Novo tipo de inimigo desbloqueado! Tier: {currentEnemyTier}");
        }

        Debug.Log($"⬆️ DIFICULDADE AUMENTOU (Nível {difficultyLevel})");
        Debug.Log($"   Max Enemies: {currentMaxEnemies}");
        Debug.Log($"   Spawn Interval: {currentSpawnInterval:F2}s");
        Debug.Log($"   Wave Size: {currentWaveSize}");
        Debug.Log($"   Enemy Tier: {currentEnemyTier}");
    }

    // ================================
    // SPAWN SISTEMA
    // ================================
    void SpawnEnemy()
    {
        // Tamanho da wave varia um pouco
        int waveSize = Random.Range(currentWaveSize - 1, currentWaveSize + 2);
        waveSize = Mathf.Max(1, waveSize);

        for (int i = 0; i < waveSize; i++)
        {
            // Não ultrapassa o limite absoluto
            if (enemies.Count >= absoluteMaxEnemies) break;

            Vector3 pos = GetOffScreenPosition();

            // Escolhe um tipo de inimigo (favorece os mais fortes conforme o tempo passa)
            GameObject prefab = GetRandomEnemyPrefab();
            GameObject e = Instantiate(prefab, pos, Quaternion.identity);
            enemies.Add(e);
        }
    }

    GameObject GetRandomEnemyPrefab()
    {
        // Sistema de pesos: inimigos mais fortes têm mais chance conforme o tier aumenta
        int maxTier = Mathf.Min(currentEnemyTier, enemyPrefabs.Length - 1);

        // Chance maior para inimigos do tier atual
        float rand = Random.value;

        if (rand < 0.6f)
        {
            // 60% de chance: inimigo do tier atual
            return enemyPrefabs[maxTier];
        }
        else if (rand < 0.9f && maxTier > 0)
        {
            // 30% de chance: inimigo do tier anterior
            return enemyPrefabs[maxTier - 1];
        }
        else
        {
            // 10% de chance: qualquer inimigo desbloqueado
            return enemyPrefabs[Random.Range(0, maxTier + 1)];
        }
    }

    Vector3 GetOffScreenPosition()
    {
        float minDist = 9f;
        float maxDist = 12f;

        Vector2 raw = Random.insideUnitCircle;

        if (raw.sqrMagnitude < 0.01f)
            raw = new Vector2(1, 0);

        Vector2 dir = raw.normalized;
        float dist = Random.Range(minDist, maxDist);
        Vector3 spawnPos = player.position + new Vector3(dir.x, dir.y, 0) * dist;

        return spawnPos;
    }

    // ================================
    // CLEANUP/DESPAWN SISTEMA
    // ================================
    void Cleanup()
    {
        // Itera de trás para frente para remover com segurança
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            GameObject e = enemies[i];

            // Remove se foi destruído
            if (e == null)
            {
                enemies.RemoveAt(i);
                continue;
            }

            // Despawn baseado em distância
            Vector3 enemyPos = e.transform.position;
            Vector3 playerPos = player.position;
            Vector3 delta = enemyPos - playerPos;
            float dx = Mathf.Abs(delta.x);
            float dy = Mathf.Abs(delta.y);

            if (dx > despawnDistanceX || dy > despawnDistanceY)
            {
                Destroy(e);
                enemies.RemoveAt(i);
            }
        }
    }
}