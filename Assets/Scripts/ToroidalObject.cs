using UnityEngine;

public class ToroidalObject : MonoBehaviour
{
    private TorusMap map;
    private Rigidbody2D rb;
    private float radius = 0.5f;

    private void Start()
    {
        map = TorusMap.Instance;
        TryGetComponent(out rb);

        // Pega SOMENTE o collider que NÃO é trigger
        Collider2D[] cols = GetComponents<Collider2D>();
        foreach (var c in cols)
        {
            if (!c.isTrigger)
            {
                radius = Mathf.Max(c.bounds.extents.x, c.bounds.extents.y);
                break;
            }
        }
    }

    // 🔥 O warp tem que rodar aqui! Nunca no Update()
    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        bool teleported = false;

        float limitX = map.halfWidth;
        float limitY = map.halfHeight;

        if (pos.x > limitX)
        {
            pos.x = -limitX + radius;
            teleported = true;
        }
        else if (pos.x < -limitX)
        {
            pos.x = limitX - radius;
            teleported = true;
        }

        if (pos.y > limitY)
        {
            pos.y = -limitY + radius;
            teleported = true;
        }
        else if (pos.y < -limitY)
        {
            pos.y = limitY - radius;
            teleported = true;
        }

        if (teleported)
            transform.position = pos;
    }
}