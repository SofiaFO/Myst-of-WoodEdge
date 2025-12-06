using UnityEngine;

public class FolhasMaster : MonoBehaviour
{
    public GameObject leafOrbitPrefab;
    public Transform player;

    public int leafCount = 2;
    public float orbitRadius = 1.2f;
    public float orbitSpeed = 180f;

    public float shootForce = 7f;
    public float projectileDamage = 5f;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").transform;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        SpawnLeaves();
    }

    public void SpawnLeaves()
    {
        for (int i = 0; i < leafCount; i++)
        {
            GameObject leaf = Instantiate(leafOrbitPrefab, player.position, Quaternion.identity);

            LeafOrbit lo = leaf.GetComponent<LeafOrbit>();

            lo.player = player;
            lo.orbitRadius = orbitRadius;
            lo.orbitSpeed = orbitSpeed;
            lo.shootForce = shootForce;

            lo.leafProjectilePrefab.GetComponent<LeafProjectile>().damage = projectileDamage;
        }
    }

    public void Upgrade()
    {
        leafCount += 1;
        orbitSpeed += 40f;
        projectileDamage += 3f;
        shootForce += 2f;

        SpawnLeaves();
    }
}