using UnityEngine;

public class TorusMap : MonoBehaviour
{
    public static TorusMap Instance;

    [Header("Tamanho total do mapa (em unidades)")]
    public float mapWidth = 36f;
    public float mapHeight = 28f;

    [HideInInspector] public float halfWidth;
    [HideInInspector] public float halfHeight;

    private void Awake()
    {
        Instance = this;

        halfWidth = mapWidth / 2f;
        halfHeight = mapHeight / 2f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(mapWidth, mapHeight, 0));
    }
}