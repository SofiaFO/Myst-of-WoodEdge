using UnityEngine;

public class MapExit : MonoBehaviour
{
    public string exitName; // "ExitTop", "ExitBottom", etc.

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"✅ [MapExit] Jogador entrou na saída: {exitName}");
            InfiniteMapGenerator.Instance.SpawnNextMap(exitName, transform.position);
        }
    }
}