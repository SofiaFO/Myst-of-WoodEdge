using System.Collections.Generic;
using UnityEngine;

public class InfiniteMapGenerator : MonoBehaviour
{
<<<<<<< HEAD
    
    public Transform player;
    public GameObject[] tilePrefabs;


    public int gridSize = 5;          // Número de tiles por eixo (ex: 5x5)
    public float tileSize = 10f;      // Tamanho do tile (depende do prefab)

    private Dictionary<Vector2Int, GameObject> tiles = new Dictionary<Vector2Int, GameObject>();
    private Vector2Int playerTileCoord;


    void Start()
    {
        GenerateInitialTiles();
        UpdatePlayerTileCoord();
    }

    void Update()
    {
        Vector2Int newCoord = GetTileCoordFromPosition(player.position);
        if (newCoord != playerTileCoord)
        {
            playerTileCoord = newCoord;
            UpdateTilesPosition();
        }
    }

    void GenerateInitialTiles()
    {
        int half = gridSize / 2;
        for (int x = -half; x <= half; x++)
        {
            for (int y = -half; y <= half; y++)
            {
                Vector2Int coord = new Vector2Int(x, y);
                Vector3 pos = new Vector3(x * tileSize, 0, y * tileSize);
                GameObject prefab = tilePrefabs[Random.Range(0, tilePrefabs.Length)];
                GameObject instance = Instantiate(prefab, pos, Quaternion.identity, transform);
                tiles.Add(coord, instance);
            }
        }
    }

    void UpdateTilesPosition()
    {
        // Reposiciona os tiles para sempre cobrir o entorno do jogador
        int half = gridSize / 2;
        List<Vector2Int> usedCoords = new List<Vector2Int>();

        foreach (var kvp in tiles)
        {
            Vector2Int coord = kvp.Key;
            GameObject tile = kvp.Value;

            Vector2Int offset = new Vector2Int(
                Mathf.RoundToInt(playerTileCoord.x + coord.x - Mathf.FloorToInt(gridSize / 2)),
                Mathf.RoundToInt(playerTileCoord.y + coord.y - Mathf.FloorToInt(gridSize / 2))
            );

            Vector3 newPos = new Vector3(offset.x * tileSize, 0, offset.y * tileSize);
            tile.transform.position = newPos;
            usedCoords.Add(offset);
        }
    }

    Vector2Int GetTileCoordFromPosition(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / tileSize);
        int y = Mathf.FloorToInt(pos.z / tileSize);
        return new Vector2Int(x, y);
    }

    void UpdatePlayerTileCoord()
    {
        playerTileCoord = GetTileCoordFromPosition(player.position);
=======
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
>>>>>>> af165bf5ec1353f0b3db43aca4f1e26936bb0197
    }
}
