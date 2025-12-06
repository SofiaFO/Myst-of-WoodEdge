using UnityEngine;
using System.Collections;

public class LaserPlayer : MonoBehaviour
{
    [Header("Referências")]
    private Animator childAnimator;
    public AudioClip attackLaserClip;

    [Header("Configuração")]
    public string animationName = "LaserPlayer";
    public float damage = 30f;
    public float activeDuration = 0f;

    private bool isActive = false;

    void Awake()
    {
        childAnimator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        // Toca o som
        if (attackLaserClip != null)
        {
            AudioSource.PlayClipAtPoint(attackLaserClip, transform.position);
        }

        // Inicia o ciclo de vida
        StartCoroutine(LifeCycle());
    }

    private IEnumerator LifeCycle()
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        Collider2D col = GetComponent<Collider2D>();

        // ATIVAR
        isActive = true;
        if (sr) sr.enabled = true;
        if (col) col.enabled = true;

        yield return new WaitForSeconds(activeDuration);

        // DESATIVAR e DESTRUIR
        isActive = false;
        if (sr) sr.enabled = false;
        if (col) col.enabled = false;

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive) return;

        if (collision.CompareTag("Enemy"))
        {
            EnemyController enemyController = collision.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.TakeDamage(damage);
            }
        }
        else if (collision.CompareTag("Boss"))
        {
            Boss bossController = collision.GetComponent<Boss>();
            if (bossController != null)
            {
                bossController.TakeDamage(damage);
            }
        }
    }

    public void Upgrade()
    {
        damage += 5f;
    }
}
