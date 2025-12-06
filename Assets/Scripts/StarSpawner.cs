using UnityEngine;
using System.Collections;

public class StarSpawner : MonoBehaviour
{
    [Header("Configuração de Spawn")]
    public GameObject prefabToSpawn;
    public int spawnCount = 3;              // Quantidade de prefabs a instanciar
    public float spawnRadius = 2f;          // Raio ao redor do player
    public float damage = 5f;
    public AudioClip starClip;

    [Header("Ciclo")]
    public float spawnDelay = 0.2f;         // Delay entre cada spawn
    public float inactiveDuration = 8f;     // Tempo de espera

    private Transform player;
    private bool isActive = false;


    void Awake()
    {
        // Encontra o player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

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

            AudioSource.PlayClipAtPoint(starClip, transform.position);

            // Spawna os prefabs com delay entre cada um
            yield return StartCoroutine(SpawnPrefabs());

            yield return new WaitForSeconds(inactiveDuration);
        }
    }

    private IEnumerator SpawnPrefabs()
    {
        if (prefabToSpawn == null || player == null)
            yield break;

        for (int i = 0; i < spawnCount; i++)
        {
            // Calcula posição ao redor do player
            float angle = (360f / spawnCount) * i;
            Vector3 offset = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * spawnRadius,
                Mathf.Sin(angle * Mathf.Deg2Rad) * spawnRadius,
                0f
            );

            Vector3 spawnPosition = player.position + offset;

            // Instancia o prefab
            GameObject instance = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

            // Passa o dano para o prefab
            StarPrefab spawnedScript = instance.GetComponent<StarPrefab>();
            if (spawnedScript != null)
            {
                spawnedScript.damage = damage;
            }

            // Aguarda o delay antes de spawnar a próxima estrela
            if (i < spawnCount - 1)
            {
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }

    public void Upgrade()
    {
        damage += 5f;
        spawnCount += 1;
    }
}