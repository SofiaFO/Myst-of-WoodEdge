using System.Collections.Generic;
using UnityEngine;

public class InfiniteMapGenerator : MonoBehaviour
{
    public static InfiniteMapGenerator Instance;

    [Header("Configurações")]
    public Transform player;
    public List<GameObject> mapPrefabs;
    public float mapSize = 20f; // tamanho de um mapa (largura e altura em unidades)
    public float unloadDistance = 60f;

    private readonly Dictionary<Vector2Int, GameObject> loadedMaps = new();
    private Vector2Int currentCoord = Vector2Int.zero;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SpawnMap(Vector2Int.zero, Vector3.zero);
    }

    private void Update()
    {
        Vector3 playerPos = player.position;

        foreach (var kvp in loadedMaps)
        {
            float distance = Vector3.Distance(playerPos, kvp.Value.transform.position);
            kvp.Value.SetActive(distance <= unloadDistance);
        }
    }

    private void SpawnMap(Vector2Int coord, Vector3 position)
    {
        GameObject prefab = GetRandomMapPrefab();
        GameObject map = Instantiate(prefab, position, Quaternion.identity, transform);
        map.name = $"Map_{coord.x}_{coord.y}";
        loadedMaps[coord] = map;
    }

    public void SpawnNextMap(string exitName, Vector3 exitPosition)
    {
        Vector2Int dir = GetDirection(exitName);
        Vector2Int newCoord = currentCoord + dir;

        if (loadedMaps.ContainsKey(newCoord))
        {
            loadedMaps[newCoord].SetActive(true);
            currentCoord = newCoord;
            return;
        }

        GameObject prefab = GetRandomMapPrefab();
        Vector3 newPos = GetNextMapPosition(exitName);
        GameObject newMap = Instantiate(prefab, newPos, Quaternion.identity, transform);
        newMap.name = $"Map_{newCoord.x}_{newCoord.y}";
        loadedMaps[newCoord] = newMap;

        currentCoord = newCoord;
    }

    private Vector3 GetNextMapPosition(string exitName)
    {
        switch (exitName)
        {
            case "ExitTop": return transform.position + new Vector3(0, mapSize, 0);
            case "ExitBottom": return transform.position + new Vector3(0, -mapSize, 0);
            case "ExitLeft": return transform.position + new Vector3(-mapSize, 0, 0);
            case "ExitRight": return transform.position + new Vector3(mapSize, 0, 0);
            default: return Vector3.zero;
        }
    }

    private Vector2Int GetDirection(string exitName)
    {
        return exitName switch
        {
            "ExitTop" => new Vector2Int(0, 1),
            "ExitBottom" => new Vector2Int(0, -1),
            "ExitLeft" => new Vector2Int(-1, 0),
            "ExitRight" => new Vector2Int(1, 0),
            _ => Vector2Int.zero
        };
    }

    private GameObject GetRandomMapPrefab()
    {
        int index = Random.Range(0, mapPrefabs.Count);
        return mapPrefabs[index];
    }
}
