using System.Collections;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float damage = 25f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float explosionRadius = 1f;

    [Header("Efeitos")]
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private AudioClip explosionClip;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool hasExploded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private void Start()
    {
        // Destrói após o tempo de vida
        StartCoroutine(DestroyAfterTime());
    }

    private void FixedUpdate()
    {
        if (player == null || hasExploded) return;

        // Segue o player
        Vector2 direction = (player.position - transform.position).normalized;
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

        if (collision.CompareTag("Player"))
        {
            // Causa dano ao player
            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(damage);
            }

            // Knockback
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockDir = (collision.transform.position - transform.position).normalized;
                playerRb.linearVelocity = Vector2.zero;
                playerRb.AddForce(knockDir * 15f, ForceMode2D.Impulse);
            }

            Explode();
        }
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);

        if (!hasExploded)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Efeito de explosão
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Som de explosão
        if (explosionClip != null)
        {
            AudioSource.PlayClipAtPoint(explosionClip, transform.position);
        }

        // Verifica se tem players na área de explosão
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerController playerController = hit.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.TakeDamage(damage * 0.5f); // Dano reduzido pela área
                }
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualiza o raio de explosão no editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}