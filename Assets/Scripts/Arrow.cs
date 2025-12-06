using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float damage = 5f;

    private Transform enemyTransform;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool hasExploded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        if (enemy != null)
        {
            enemyTransform = enemy.transform;
        }

        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        if (boss != null)
        {
            enemyTransform = boss.transform;
        }
    }

    private void FixedUpdate()
    {
        if (enemyTransform == null || hasExploded) return;

        // Segue o player
        Vector2 direction = (enemyTransform.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        // Flipa o sprite baseado na direção horizontal
        if (spriteRenderer != null && direction.x != 0)
        {
            // Se indo para esquerda (x negativo), flipa
            spriteRenderer.flipX = direction.x < 0;
        }

        // Rotaciona o projétil na direção do movimento
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasExploded) return;

        if (collision.CompareTag("Enemy"))
        {
            // Causa dano ao player
            EnemyController enemyController = collision.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.TakeDamage(damage);
            }
        }
        else if (collision.CompareTag("Boss"))
        {
            // Causa dano ao boss
            Boss bossController = collision.GetComponent<Boss>();
            if (bossController != null)
            {
                bossController.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}