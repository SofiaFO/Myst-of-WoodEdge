using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gerencia inimigos em um mapa toroidal, mantendo-os sincronizados com o player
/// Adicione este script em um GameObject vazio na cena (ex: "EnemyManager")
/// </summary>
public class ToroidalEnemyManager : MonoBehaviour
{
    public static ToroidalEnemyManager Instance;

    [Header("Configurações")]
    [Tooltip("Tag dos inimigos que devem ser gerenciados")]
    public string enemyTag = "Enemy";
    
    [Tooltip("Player de referência para calcular wrap")]
    public Transform player;
    
    [Tooltip("Distância mínima para considerar wrap (metade do mapa)")]
    public float wrapThreshold = 20f;

    private TorusMap map;
    private HashSet<Transform> enemies = new HashSet<Transform>();
    private Vector3 lastPlayerPos;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        map = TorusMap.Instance;
        
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        // Registrar todos os inimigos existentes
        RegisterAllEnemies();
        
        if (player != null)
            lastPlayerPos = player.position;
    }

    private void LateUpdate()
    {
        if (player == null || map == null) return;

        Vector3 currentPlayerPos = player.position;
        Vector3 delta = currentPlayerPos - lastPlayerPos;

        // Se o player teletransportou (mudança grande e súbita)
        float w = map.mapWidth;
        float h = map.mapHeight;

        bool playerWrappedX = Mathf.Abs(delta.x) > wrapThreshold;
        bool playerWrappedY = Mathf.Abs(delta.y) > wrapThreshold;

        if (playerWrappedX || playerWrappedY)
        {
            Debug.Log($"🔄 Player teletransportou! Delta: {delta}");
            
            // Determinar direção do wrap
            Vector3 wrapOffset = Vector3.zero;
            
            if (playerWrappedX)
            {
                // Se player foi pra direita mas a posição diminuiu = wrap esquerda->direita
                wrapOffset.x = (delta.x < 0) ? w : -w;
            }
            
            if (playerWrappedY)
            {
                wrapOffset.y = (delta.y < 0) ? h : -h;
            }

            // Mover todos os inimigos junto
            WrapAllEnemies(wrapOffset);
        }

        lastPlayerPos = currentPlayerPos;
    }

    /// <summary>
    /// Move todos os inimigos pelo offset especificado
    /// </summary>
    private void WrapAllEnemies(Vector3 offset)
    {
        int count = 0;
        foreach (Transform enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.position += offset;
                count++;
            }
        }
        
        Debug.Log($"✅ {count} inimigos movidos por {offset}");
    }

    /// <summary>
    /// Registra todos os inimigos com a tag especificada
    /// </summary>
    public void RegisterAllEnemies()
    {
        GameObject[] foundEnemies = GameObject.FindGameObjectsWithTag(enemyTag);
        
        foreach (GameObject enemy in foundEnemies)
        {
            RegisterEnemy(enemy.transform);
        }
        
        Debug.Log($"🎯 {enemies.Count} inimigos registrados no sistema toroidal");
    }

    /// <summary>
    /// Registra um inimigo específico
    /// </summary>
    public void RegisterEnemy(Transform enemy)
    {
        if (enemy != null && !enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    /// <summary>
    /// Remove um inimigo do sistema (quando morre, por exemplo)
    /// </summary>
    public void UnregisterEnemy(Transform enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        // Desenhar linha entre player e cada inimigo
        Gizmos.color = Color.magenta;
        foreach (Transform enemy in enemies)
        {
            if (enemy != null)
            {
                Gizmos.DrawLine(player.position, enemy.position);
            }
        }
    }
}
