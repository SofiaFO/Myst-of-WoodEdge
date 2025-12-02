using UnityEngine;

public class ToroidalObject : MonoBehaviour
{
    private TorusMap map;
    private Rigidbody2D rb;
    private float objectSize = 0.1f;
    
    [Header("Ajuste de Teleporte")]
    [Tooltip("Distância extra da borda antes de teleportar (ajuste se aparecer área azul)")]
    [Range(0f, 15f)]
    public float teleportMargin = 0.2f; // Margem maior para evitar mostrar a borda

    private void Start()
    {
        map = TorusMap.Instance;
        TryGetComponent(out rb);

        // Pega o tamanho do collider principal
        Collider2D[] cols = GetComponents<Collider2D>();
        foreach (var c in cols)
        {
            if (!c.isTrigger)
            {
                objectSize = Mathf.Max(c.bounds.extents.x, c.bounds.extents.y);
                break;
            }
        }
    }

    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        bool teleported = false;

        float w = map.mapWidth;
        float h = map.mapHeight;
        
        // Limites de teleporte: ANTES da borda para a câmera não mostrar área azul
        // Precisamos de margem suficiente para a câmera (que tem ~9 unidades de raio)
        float wrapX = map.halfWidth - objectSize - teleportMargin;
        float wrapY = map.halfHeight - objectSize - teleportMargin;

        // Teleporte horizontal
        if (pos.x > wrapX)
        {
            // Calcular quanto passou da borda
            float overflow = pos.x - wrapX;
            // Aparecer do outro lado com o mesmo overflow
            pos.x = -wrapX + overflow;
            teleported = true;
            Debug.Log($"Teleporte DIREITA -> ESQUERDA | Antes: {pos.x + wrapX - overflow:F2} | Depois: {pos.x:F2} | Overflow: {overflow:F2}");
        }
        else if (pos.x < -wrapX)
        {
            float overflow = -wrapX - pos.x;
            pos.x = wrapX - overflow;
            teleported = true;
            Debug.Log($"Teleporte ESQUERDA -> DIREITA | Antes: {pos.x - wrapX + overflow:F2} | Depois: {pos.x:F2} | Overflow: {overflow:F2}");
        }

        // Teleporte vertical
        if (pos.y > wrapY)
        {
            float overflow = pos.y - wrapY;
            pos.y = -wrapY + overflow;
            teleported = true;
            Debug.Log($"Teleporte CIMA -> BAIXO | Antes: {pos.y + wrapY - overflow:F2} | Depois: {pos.y:F2} | Overflow: {overflow:F2}");
        }
        else if (pos.y < -wrapY)
        {
            float overflow = -wrapY - pos.y;
            pos.y = wrapY - overflow;
            teleported = true;
            Debug.Log($"Teleporte BAIXO -> CIMA | Antes: {pos.y - wrapY + overflow:F2} | Depois: {pos.y:F2} | Overflow: {overflow:F2}");
        }

        if (teleported)
        {
            transform.position = pos;
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (map == null) return;
        
        // Desenhar os limites de teleporte
        Gizmos.color = Color.cyan;
        float wrapX = map.halfWidth - objectSize;
        float wrapY = map.halfHeight - objectSize;
        
        Vector3 topLeft = new Vector3(-wrapX, wrapY, 0);
        Vector3 topRight = new Vector3(wrapX, wrapY, 0);
        Vector3 bottomLeft = new Vector3(-wrapX, -wrapY, 0);
        Vector3 bottomRight = new Vector3(wrapX, -wrapY, 0);
        
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}