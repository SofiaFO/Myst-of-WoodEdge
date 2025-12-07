using UnityEngine;

public class CameraToroidalFollow : MonoBehaviour
{
    public Transform player;
    
    [Header("Configurações de Suavização")]
    [Range(0.01f, 0.5f)]
    public float smoothSpeed = 0.15f;
    
    [Header("Ajuste de Margem por Lado")]
    [Range(0f, 5f)]
    public float edgeOffsetLeft = 1f;
    [Range(0f, 5f)]
    public float edgeOffsetRight = 1f;
    [Range(0f, 5f)]
    public float edgeOffsetTop = 1f;
    [Range(0f, 5f)]
    public float edgeOffsetBottom = 1f;

    private Vector3 cameraPos;
    private Vector3 targetPos; // posição alvo que a câmera quer seguir
    private TorusMap map;

    private float camWidth, camHeight;

    private void Start()
    {
        map = TorusMap.Instance;

        // Posição inicial
        cameraPos = player.position;
        targetPos = player.position;

        // Calcular tamanho da câmera
        var cam = Camera.main;
        camHeight = cam.orthographicSize;
        camWidth = camHeight * cam.aspect;
    }

    void LateUpdate()
    {
        if (player == null)
        {
            // tenta reencontrar o player automaticamente
            var found = GameObject.FindWithTag("Player");
            if (found != null)
            {
                player = found.transform;
                targetPos = player.position;
                cameraPos = player.position;
            }
            else
            {
                return; // sem player, não faz nada
            }
        }
        Vector3 playerPos = player.position;
        float w = map.mapWidth;
        float h = map.mapHeight;

        // Calcular caminho mais curto do TARGET até o player
        float dx = playerPos.x - targetPos.x;
        float dy = playerPos.y - targetPos.y;

        // Normalizar distância considerando wrap-around
        if (dx > w / 2f) dx -= w;
        if (dx < -w / 2f) dx += w;
        if (dy > h / 2f) dy -= h;
        if (dy < -h / 2f) dy += h;

        // Atualizar posição alvo (segue o player sem restrições)
        targetPos += new Vector3(dx, dy, 0) * smoothSpeed;

        // Normalizar target para ficar dentro do mapa
        while (targetPos.x > map.halfWidth) targetPos.x -= w;
        while (targetPos.x < -map.halfWidth) targetPos.x += w;
        while (targetPos.y > map.halfHeight) targetPos.y -= h;
        while (targetPos.y < -map.halfHeight) targetPos.y += h;

        // Agora vamos posicionar a CÂMERA considerando os limites visuais
        // Encontrar a melhor posição da câmera que mantém o target visível
        float bestCamX = targetPos.x;
        float bestCamY = targetPos.y;

        // Limites onde a câmera pode estar (com offset de segurança por lado)
        // A câmera não pode ir além desses pontos sem mostrar a borda azul
        float minX = -map.halfWidth + camWidth + edgeOffsetLeft;
        float maxX = map.halfWidth - camWidth - edgeOffsetRight;
        float minY = -map.halfHeight + camHeight + edgeOffsetBottom;
        float maxY = map.halfHeight - camHeight - edgeOffsetTop;

        // Se o target está fora da área segura, precisamos encontrar alternativas
        // Testar as 9 posições possíveis (original + 8 wraps) e escolher a melhor
        float bestDist = float.MaxValue;
        
        for (int wx = -1; wx <= 1; wx++)
        {
            for (int wy = -1; wy <= 1; wy++)
            {
                float testX = targetPos.x + (wx * w);
                float testY = targetPos.y + (wy * h);

                // Aplicar clamp nesta posição teste
                float clampedX = Mathf.Clamp(testX, minX, maxX);
                float clampedY = Mathf.Clamp(testY, minY, maxY);

                // Calcular distância entre a posição clampada e a posição teste
                float dist = Vector2.Distance(
                    new Vector2(clampedX, clampedY),
                    new Vector2(testX, testY)
                );

                // Se essa combinação é melhor (menor distorção), usar ela
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestCamX = clampedX;
                    bestCamY = clampedY;
                }
            }
        }

        // Interpolar suavemente até a melhor posição
        cameraPos.x = Mathf.Lerp(cameraPos.x, bestCamX, 0.3f);
        cameraPos.y = Mathf.Lerp(cameraPos.y, bestCamY, 0.3f);
        cameraPos.z = transform.position.z;

        // Atualizar posição da câmera
        transform.position = cameraPos;
    }

    // Método auxiliar para debug
    private void OnDrawGizmos()
    {
        if (map == null || Camera.main == null) return;

        // Desenhar limites do mapa
        Gizmos.color = Color.red;
        Vector3 topLeft = new Vector3(-map.halfWidth, map.halfHeight, 0);
        Vector3 topRight = new Vector3(map.halfWidth, map.halfHeight, 0);
        Vector3 bottomLeft = new Vector3(-map.halfWidth, -map.halfHeight, 0);
        Vector3 bottomRight = new Vector3(map.halfWidth, -map.halfHeight, 0);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);

        // Desenhar área segura da câmera (com margens independentes)
        float minX = -map.halfWidth + camWidth + edgeOffsetLeft;
        float maxX = map.halfWidth - camWidth - edgeOffsetRight;
        float minY = -map.halfHeight + camHeight + edgeOffsetBottom;
        float maxY = map.halfHeight - camHeight - edgeOffsetTop;

        Gizmos.color = Color.yellow;
        topLeft = new Vector3(minX, maxY, 0);
        topRight = new Vector3(maxX, maxY, 0);
        bottomLeft = new Vector3(minX, minY, 0);
        bottomRight = new Vector3(maxX, minY, 0);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);

        // Desenhar posição atual da câmera
        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(cameraPos, new Vector3(camWidth * 2, camHeight * 2, 0));
            
            // Desenhar target
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(targetPos, 0.5f);
        }
    }
}