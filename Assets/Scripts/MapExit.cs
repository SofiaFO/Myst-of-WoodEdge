using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MapExit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && InfiniteMapGenerator.Instance != null)
        {
            InfiniteMapGenerator.Instance.SpawnNextMap(gameObject.name, transform.position);
        }
    }
}