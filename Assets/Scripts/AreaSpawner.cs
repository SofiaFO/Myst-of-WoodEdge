using UnityEngine;
using System.Collections;

public class AreaSpawner : MonoBehaviour
{
    [Header("Configuração de Spawn")]
    public GameObject prefabToSpawn;
    public Transform spawnPoint;            // Ponto específico de spawn
    public float spawnInterval = 0.2f;      // Intervalo entre spawns
    public float spawnDuration = 5f;        // Duração total do spawn
    public float damage = 15f;

    [Header("Ciclo")]
    public float inactiveDuration = 10f;    // Tempo de espera entre ciclos

    private bool isActive = false;

    void Awake()
    {
        // Inicia desativado
        if (!isActive)
            gameObject.SetActive(false);
    }

    void Start()
    {
        StartCoroutine(Cycle());
    }

    private IEnumerator Cycle()
    {
        while (true)
        {
            // ATIVAR
            isActive = true;

            // Spawna durante X segundos
            yield return StartCoroutine(SpawnRoutine());

            // DESATIVAR
            isActive = false;

            yield return new WaitForSeconds(inactiveDuration);
        }
    }

    private IEnumerator SpawnRoutine()
    {
        if (prefabToSpawn == null || spawnPoint == null)
            yield break;

        float elapsedTime = 0f;

        while (elapsedTime < spawnDuration)
        {
            // Instancia o prefab no ponto específico
            GameObject instance = Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);

            // Passa o dano para o prefab
            AreaPrefab areaDamageScript = instance.GetComponent<AreaPrefab>();
            if (areaDamageScript != null)
            {
                areaDamageScript.damage = damage;
            }

            // Aguarda o intervalo
            yield return new WaitForSeconds(spawnInterval);
            elapsedTime += spawnInterval;
        }
    }

    public void Upgrade()
    {
        damage += 3f;
        spawnDuration += 1f;
    }
}