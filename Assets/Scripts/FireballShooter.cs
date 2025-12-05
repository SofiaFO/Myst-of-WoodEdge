using UnityEngine;

public class FireballShooter : MonoBehaviour
{
    public Transform player;
    public GameObject fireballPrefab;

    public float shootInterval = 1.5f;
    public float projectileSpeed = 7f;
    public float range = 10f;

    private float timer;

    void Start()
    {
        timer = shootInterval;
        if (player == null)
            player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            ShootFireball();
            timer = shootInterval;
        }

        // Sempre seguir o jogador
        transform.position = player.position;
    }

    void ShootFireball()
    {
        Transform target = GetClosestEnemy();
        if (target == null) return;

        GameObject fb = Instantiate(fireballPrefab, transform.position, Quaternion.identity);

        Vector2 dir = (target.position - transform.position).normalized;

        fb.GetComponent<Rigidbody2D>().linearVelocity = dir * projectileSpeed;
    }

    Transform GetClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < minDist && dist <= range)
            {
                minDist = dist;
                closest = enemy.transform;
            }
        }

        return closest;
    }

    public void Upgrade()
    {
        projectileSpeed += 2f;
        shootInterval = Mathf.Max(0.4f, shootInterval - 0.2f);
        range += 2f;
    }
}