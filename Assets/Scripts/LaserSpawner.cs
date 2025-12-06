using UnityEngine;
using System.Collections;

public class LaserSpawner : MonoBehaviour
{
    [Header("Configuração de Spawn")]
    public GameObject laserPrefab;
    public float damage = 30f;

    [Header("Ciclo")]
    public float spawnInterval = 5f;        // Tempo entre cada spawn (igual ao inactiveDuration do laser)

    private Transform player;

    void Awake()
    {
        // Encontra o player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        gameObject.SetActive(false);
    }

    void Start()
    {
        StartCoroutine(SpawnCycle());
    }

    private IEnumerator SpawnCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            SpawnLaser();
        }
    }

    private void SpawnLaser()
    {
        if (laserPrefab == null || player == null)
            return;

        // Instancia o laser na posição do player
        GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);

        // Passa o dano para o laser
        LaserPlayer laserScript = laser.GetComponent<LaserPlayer>();
        if (laserScript != null)
        {
            laserScript.damage = damage;
        }

        // Passa o dano para o laser
    }

    public void Upgrade()
    {
        damage += 5f;
    }
}
