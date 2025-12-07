using UnityEngine;
using System.Collections;

public class MachadoGir : MonoBehaviour
{
    public Transform player;
    public Transform axeSprite;

    public float radius = 0.4f;
    public float orbitSpeed = 4f;
    public float spinSpeed = 720f;
    public float damage = 10f;
    public float activeDuration = 4f;
    public float inactiveDuration = 10f;

    private bool isActive = false;
    private float orbitAngle;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        gameObject.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(Cycle());
    }

    private void FixedUpdate()
    {
        if (!isActive) return;

        orbitAngle += orbitSpeed * Time.deltaTime;

        Vector3 offset = new Vector3(
            Mathf.Cos(orbitAngle) * radius,
            Mathf.Sin(orbitAngle) * radius,
            0f
        );

        transform.position = player.position + offset;

        axeSprite.Rotate(0, 0, spinSpeed * Time.deltaTime);
    }

    private IEnumerator Cycle()
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        Collider2D col = GetComponent<Collider2D>();

        while (true)
        {
            isActive = true;
            if (sr) sr.enabled = true;
            if (col) col.enabled = true;

            orbitAngle = 0;

            yield return new WaitForSeconds(activeDuration);

            isActive = false;
            if (sr) sr.enabled = false;
            if (col) col.enabled = false;

            yield return new WaitForSeconds(inactiveDuration);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();

            if (enemy != null)
                enemy.TakeDamage(damage);
        }
        else if (other.CompareTag("Boss"))
        {
            Boss boss = other.GetComponent<Boss>();
            if (boss != null)
                boss.TakeDamage(damage);
        }
    }

    public void Upgrade()
    {
        damage += 3f;
        orbitSpeed += 0.3f;
    }
}
