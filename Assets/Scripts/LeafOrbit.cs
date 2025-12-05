using UnityEngine;

public class LeafOrbit : MonoBehaviour
{
    public Transform player;     
    public float orbitRadius = 1.2f;
    public float orbitSpeed = 180f;

    public float spinDuration = 3f;    // tempo girando
    public float shootForce = 7f;      // velocidade quando atira
    public GameObject leafProjectilePrefab;

    private float angle;
    private float timer;
    private bool spinning = true;

    void Start()
    {
        timer = spinDuration;

        // Posiciona em uma posição aleatória do círculo
        angle = Random.Range(0f, 360f);
    }

    void Update()
    {
        if (player == null) return;

        if (spinning)
        {
            SpinAroundPlayer();
        }
        else
        {
            ShootAndDestroy();
        }
    }

    void SpinAroundPlayer()
    {
        timer -= Time.deltaTime;

        // Aumenta o ângulo
        angle += orbitSpeed * Time.deltaTime;

        // Calcula posição ao redor do player
        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * orbitRadius;

        transform.position = player.position + offset;

        if (timer <= 0)
        {
            spinning = false;
        }
    }

    void ShootAndDestroy()
    {
        // Instancia projétil
        GameObject proj = Instantiate(leafProjectilePrefab, transform.position, Quaternion.identity);

        // Direção aleatória
        Vector2 randomDir = Random.insideUnitCircle.normalized;

        proj.GetComponent<Rigidbody2D>().AddForce(randomDir * shootForce, ForceMode2D.Impulse);

        Destroy(gameObject); // destrói folha orbitando
    }
}