using System.Collections.Generic;
using UnityEngine;

public class InfiniteMapGenerator : MonoBehaviour
{
    
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
    }
}
