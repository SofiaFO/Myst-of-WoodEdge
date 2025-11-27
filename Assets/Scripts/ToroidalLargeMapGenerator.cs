using UnityEngine;

public class ToroidalLargeMapGenerator : MonoBehaviour
{
    [Header("Prefabs do mapa")]
    public GameObject[] mapPrefabs;

    [Header("Tamanho de UM prefab (como estava no TorusMap)")]
    public float singleMapWidth = 36f;
    public float singleMapHeight = 28f;

    [Header("Quantidade de mapas")]
    public int gridX = 3;
    public int gridY = 3;

    [Header("Referencia ao TorusMap")]
    public TorusMap torusMap;

    private void Start()
    {
        GenerateLargeMap();
    }

    private void GenerateLargeMap()
    {
        // 1. Calcula tamanho total do mapa toroidal
        float totalWidth = singleMapWidth * gridX;
        float totalHeight = singleMapHeight * gridY;

        // 2. Ajusta valores no TorusMap
        torusMap.mapWidth = totalWidth;
        torusMap.mapHeight = totalHeight;

        torusMap.halfWidth = totalWidth / 2f;
        torusMap.halfHeight = totalHeight / 2f;

        // 3. Calcula ponto inicial (centro)
        Vector3 origin = new Vector3(
            -totalWidth / 2f + singleMapWidth / 2f,
            -totalHeight / 2f + singleMapHeight / 2f,
            0
        );

        // 4. Instancia grade de mapas
        for (int y = 0; y < gridY; y++)
        {
            for (int x = 0; x < gridX; x++)
            {
                GameObject prefab = GetRandomMapPrefab();

                Vector3 pos = origin + new Vector3(
                    x * singleMapWidth,
                    y * singleMapHeight,
                    0
                );

                Instantiate(prefab, pos, Quaternion.identity, transform);
            }
        }

        Debug.Log("🟢 Mapão toroidal criado com sucesso!");
    }

    private GameObject GetRandomMapPrefab()
    {
        int index = Random.Range(0, mapPrefabs.Length);
        return mapPrefabs[index];
    }
}