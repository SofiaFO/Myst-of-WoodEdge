using UnityEngine;
using System.Collections;

public class StarPrefab : MonoBehaviour
{
    [Header("Configuração")]
    public string animationName = "Attack";
    public float damage = 20f;
    public float colliderActivationDelay = 0.6f;

    private Animator childAnimator;
    private Collider2D col;
    private bool colliderActivated = false;

    void Awake()
    {
        childAnimator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();

        // Inicia com o collider desligado
        if (col != null)
        {
            col.enabled = false;
        }
    }

    void Start()
    {
        // Ativa o collider após o delay
        StartCoroutine(ActivateColliderAfterDelay());
    }

    void Update()
    {
        if (childAnimator == null)
            return;

        AnimatorStateInfo state = childAnimator.GetCurrentAnimatorStateInfo(0);

        Destroy(gameObject, 1f);
    }

    private IEnumerator ActivateColliderAfterDelay()
    {
        yield return new WaitForSeconds(colliderActivationDelay);

        if (col != null)
        {
            col.enabled = true;
            colliderActivated = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!colliderActivated) return;

        if (collision.CompareTag("Enemy"))
        {
            // Causa dano ao inimigo
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
    }
}
