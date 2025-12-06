using UnityEngine;

public class BossLaser : MonoBehaviour
{
    [Header("Referências")]
    private Animator childAnimator;   // Animator do objeto FILHO
    public AudioClip attackLaserClip;

    [Header("Configuração")]
    public string animationName = "Laser2";

    private Transform player;

    void Awake()
    {
        // Obtém o Animator e o objeto FILHO
        childAnimator = GetComponentInChildren<Animator>();

        // Encontra o player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void OnEnable()
    {
        // Flipa na direção do player antes de qualquer coisa
        FlipTowardsPlayer();

        // Toca o som do ataque ao ativar o objeto
        if (attackLaserClip != null)
        {
            AudioSource.PlayClipAtPoint(attackLaserClip, transform.position);
        }
    }

    void Update()
    {
        if (childAnimator == null)
            return;

        AnimatorStateInfo state = childAnimator.GetCurrentAnimatorStateInfo(0);

        // Verifica se a animação terminou
        if (state.IsName(animationName) && state.normalizedTime >= 1f)
        {
            gameObject.SetActive(false);
        }
    }

    private void FlipTowardsPlayer()
    {
        if (player == null)
            return;

        // Calcula a direção do player em relação ao laser
        float direction = player.position.x - transform.position.x;

        // Ajusta a escala X para flipar
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (direction > 0 ? 1 : -1);
        transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // DANO
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(40f);
            }

            // KNOCKBACK
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Direção do empurrão (do inimigo para o player)
                Vector2 knockDir = (collision.transform.position - transform.position).normalized;
                float knockForce = 20f; // ajuste a força aqui
                rb.linearVelocity = Vector2.zero; // zera velocidade atual
                rb.AddForce(knockDir * knockForce, ForceMode2D.Impulse);
            }
        }
    }
}