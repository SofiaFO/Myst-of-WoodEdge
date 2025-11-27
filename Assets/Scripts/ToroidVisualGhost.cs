using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ToroidalVisualGhost : MonoBehaviour
{
    private SpriteRenderer mainRenderer;
    private SpriteRenderer[] ghostRenderers;

    private TorusMap map;

    void Start()
    {
        map = TorusMap.Instance;
        mainRenderer = GetComponent<SpriteRenderer>();

        // Criar 8 fantasmas ao redor (N, NE, E, SE, S, SW, W, NW)
        ghostRenderers = new SpriteRenderer[8];

        for (int i = 0; i < 8; i++)
        {
            GameObject ghost = new GameObject("Ghost_" + i);
            ghost.transform.parent = transform;
            ghost.transform.localScale = Vector3.one;

            SpriteRenderer sr = ghost.AddComponent<SpriteRenderer>();
            sr.sprite = mainRenderer.sprite;
            sr.sortingLayerID = mainRenderer.sortingLayerID;
            sr.sortingOrder = mainRenderer.sortingOrder;

            ghostRenderers[i] = sr;
        }
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;

        float w = map.mapWidth;
        float h = map.mapHeight;

        // Vetores de offset para cada fantasma
        Vector3[] offsets = new Vector3[]
        {
            new Vector3( w, 0, 0),
            new Vector3(-w, 0, 0),
            new Vector3(0,  h, 0),
            new Vector3(0, -h, 0),

            new Vector3( w,  h, 0),
            new Vector3( w, -h, 0),
            new Vector3(-w,  h, 0),
            new Vector3(-w, -h, 0)
        };

        // Atualizar posição dos fantasmas
        for (int i = 0; i < 8; i++)
            ghostRenderers[i].transform.position = pos + offsets[i];
    }
}