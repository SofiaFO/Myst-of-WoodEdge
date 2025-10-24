using System.Collections.Generic;
using UnityEngine;

public class InfiniteMapGenerator : MonoBehaviour
{
    public static InfiniteMapGenerator Instance;

    [Header("Configurações")]
    public Transform player;
    public List<GameObject> mapPrefabs;

    [Tooltip("Largura total do mapa em unidades")]
    public float mapWidth = 36f;

    [Tooltip("Altura total do mapa em unidades")]
    public float mapHeight = 28f;

    [Tooltip("Distância máxima antes de descarregar mapas")]
    public float unloadDistance = 60f;

    private readonly Dictionary<Vector2Int, GameObject> loadedMaps = new();
    private readonly Dictionary<Vector2Int, Vector3> mapPositions = new(); // NOVO: rastreia posições exatas

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Debug.Log("🟢 [InfiniteMapGenerator] Inicializando mapa inicial...");
        SpawnMap(Vector2Int.zero, Vector3.zero);
    }

    private void Update()
    {
        Vector3 playerPos = player.position;

        // Ativa/desativa mapas com base na distância do jogador
        foreach (var kvp in loadedMaps)
        {
            float distance = Vector3.Distance(playerPos, kvp.Value.transform.position);
            kvp.Value.SetActive(distance <= unloadDistance);
        }
    }

    // Retorna em qual mapa o jogador está baseado na posição
    private Vector2Int GetPlayerMapCoord()
    {
        Vector3 playerPos = player.position;
        
        // Calcula qual grid o jogador está baseado na posição
        // Ajuste importante: centraliza o cálculo corretamente
        int x = Mathf.FloorToInt(playerPos.x / mapWidth + 0.5f);
        int y = Mathf.FloorToInt(playerPos.y / mapHeight + 0.5f);
        
        Vector2Int coord = new Vector2Int(x, y);
        
        Debug.Log($"🎮 [GetPlayerMapCoord] Player pos: {playerPos} → Calculado [{x}, {y}] → Mapa [{coord.x}, {coord.y}]");
        
        // Verifica se esse mapa realmente existe
        if (!loadedMaps.ContainsKey(coord))
        {
            Debug.LogWarning($"⚠️ [GetPlayerMapCoord] Mapa calculado {coord} não existe! Procurando mapa mais próximo...");
            
            // Busca o mapa mais próximo como fallback
            Vector2Int closest = Vector2Int.zero;
            float minDist = float.MaxValue;
            
            foreach (var kvp in mapPositions)
            {
                float dist = Vector3.Distance(playerPos, kvp.Value);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = kvp.Key;
                }
            }
            
            Debug.Log($"🔍 [GetPlayerMapCoord] Usando mapa mais próximo: {closest} (dist: {minDist})");
            return closest;
        }
        
        return coord;
    }

    // Spawna um mapa em coordenadas específicas
    private void SpawnMap(Vector2Int coord, Vector3 position)
    {
        if (loadedMaps.ContainsKey(coord))
        {
            Debug.LogWarning($"⚠️ Tentando spawnar mapa que já existe em {coord}");
            return;
        }

        GameObject prefab = GetRandomMapPrefab();
        GameObject map = Instantiate(prefab, position, Quaternion.identity, transform);
        map.name = $"Map_{coord.x}_{coord.y}";

        loadedMaps[coord] = map;
        mapPositions[coord] = position; // Salva a posição exata

        Debug.Log($"🧱 [SpawnMap] Mapa {coord} criado na posição {position}");
    }

    // MÉTODO CORRIGIDO: Agora calcula baseado na posição do jogador, não no exitPosition
    public void SpawnNextMap(string exitName, Vector3 exitPosition)
    {
        Debug.Log($"🚪 [SpawnNextMap] Chamado! Exit: {exitName}, Pos: {exitPosition}, Player: {player.position}");
        
        // Descobre em qual mapa o jogador está AGORA
        Vector2Int currentCoord = GetPlayerMapCoord();
        
        if (!loadedMaps.ContainsKey(currentCoord))
        {
            Debug.LogError($"❌ [SpawnNextMap] Mapa atual {currentCoord} não existe no dicionário!");
            Debug.Log($"📋 Mapas carregados: {string.Join(", ", loadedMaps.Keys)}");
            return;
        }

        // Calcula coordenada do próximo mapa
        Vector2Int dir = GetDirection(exitName);
        Vector2Int newCoord = currentCoord + dir;

        Debug.Log($"📍 [SpawnNextMap] Mapa atual: {currentCoord}, Direção: {dir}, Novo mapa: {newCoord}");

        // Se já existe, não precisa criar
        if (loadedMaps.ContainsKey(newCoord))
        {
            Debug.Log($"✅ [SpawnNextMap] Mapa {newCoord} já existe!");
            return;
        }

        // Calcula posição do novo mapa baseado na posição do mapa atual
        Vector3 currentMapPos = mapPositions[currentCoord];
        Vector3 offset = exitName switch
        {
            "ExitTop" => new Vector3(0, mapHeight, 0),
            "ExitDown" => new Vector3(0, -mapHeight, 0),
            "ExitLeft" => new Vector3(-mapWidth, 0, 0),
            "ExitRight" => new Vector3(mapWidth, 0, 0),
            _ => Vector3.zero
        };
        Vector3 newPos = currentMapPos + offset;

        Debug.Log($"🌍 [SpawnNextMap] Criando novo mapa em {newCoord} na posição {newPos}");
        
        // Cria o novo mapa
        SpawnMap(newCoord, newPos);
    }

    // Converte nome da saída em direção de coordenadas
    private Vector2Int GetDirection(string exitName)
    {
        return exitName switch
        {
            "ExitTop" => new Vector2Int(0, 1),
            "ExitDown" => new Vector2Int(0, -1),
            "ExitLeft" => new Vector2Int(-1, 0),
            "ExitRight" => new Vector2Int(1, 0),
            _ => Vector2Int.zero
        };
    }

    // Pega prefab aleatório
    private GameObject GetRandomMapPrefab()
    {
        int index = Random.Range(0, mapPrefabs.Count);
        return mapPrefabs[index];
    }

    // MÉTODO HELPER para debug(DEBUG APENAS TIRAR DEPOIS)
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // Desenha o grid dos mapas
        Gizmos.color = Color.cyan;
        foreach (var kvp in mapPositions)
        {
            Vector3 pos = kvp.Value;
            Gizmos.DrawWireCube(pos, new Vector3(mapWidth, mapHeight, 0));
            
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(pos, $"[{kvp.Key.x},{kvp.Key.y}]");
            #endif
        }

        // Desenha posição do player
        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.position, 1f);
        }
    }
}