using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform player;

    [Header("Enemy Types")]
    public GameObject[] enemyPrefabs;

    [Header("Spawn Settings")]
    public int maxEnemies = 1;
    public float spawnInterval = 1.5f;

    public float despawnDistanceX = 13f;
    public float despawnDistanceY = 13f;

    private float nextSpawn;
    private List<GameObject> enemies = new List<GameObject>();


    void Start()
    {
        nextSpawn = Time.time + spawnInterval;
    }

    void Update()
    {

        if (player == null) return;

        if (Time.time >= nextSpawn && enemies.Count < maxEnemies)
        {
            SpawnEnemy();
            nextSpawn = Time.time + spawnInterval;
        }

        Cleanup();
    }

    // ================================
    // SPAWN SISTEMA
    // ================================
    void SpawnEnemy()
    {
        Vector3 pos = GetOffScreenPosition();

        GameObject prefab = enemyPrefabs[0];
        GameObject e = Instantiate(prefab, pos, Quaternion.identity);
        enemies.Add(e);
    }

    Vector3 GetOffScreenPosition()
    {
        float minDist = 9f;
        float maxDist = 10f;

        Vector2 raw = Random.insideUnitCircle;

        // se o vetor for muito pequeno, force outra direção
        if (raw.sqrMagnitude < 0.01f)
            raw = new Vector2(1, 0); // fallback seguro

        Vector2 dir = raw.normalized;
        float dist = Random.Range(minDist, maxDist);

        Vector3 spawnPos = player.position + new Vector3(dir.x, dir.y, 0) * dist;

        float dx = Mathf.Abs(spawnPos.x - player.position.x);
        float dy = Mathf.Abs(spawnPos.y - player.position.y);

        return spawnPos;
    }


    // ================================
    // DESPAWN SISTEMA
    // ================================
    // Substitua sua função Cleanup() por esta:
    void Cleanup()
    {

        // Itera de trás para frente para remover com segurança
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            GameObject e = enemies[i];

            if (e == null)
            {
                enemies.RemoveAt(i);
                continue;
            }

            Vector3 enemyPos = e.transform.position;
            Vector3 playerPos = player.position;

            // calcula deslocamento relativo (pode ser negativo) e então pega absoluto
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
