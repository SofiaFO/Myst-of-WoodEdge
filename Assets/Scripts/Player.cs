using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

<<<<<<< HEAD

/// Espera parâmetros de Animator:
///   - "LookX" (float), "LookY" (float) -> direção que o personagem está olhando
///   - "Speed" (float) -> magnitude do movimento (para blend/idle)
///   - "Attack" (trigger) -> animação de ataque
///   - "Die" (trigger) -> animação de morte
/// Input System callbacks esperados:
///   - OnMove(InputValue) -> Vector2
///   - OnAttack(InputValue) -> botão/souce (pressed)

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
   
   private PlayerStats _stats;
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D _rb;
    private Vector2 _moveInput;

    // direção que o jogador está olhando (unit vector). Inicializa olhando para baixo.
    private Vector2 _facing = Vector2.down;

    [SerializeField] private float attackRate = 0.6f;     // segundos entre ataques
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float attackRange = 0.6f;    // raio do círculo de ataque
    [SerializeField] private float attackOffset = 0.75f;  // distância do ponto de ataque em relação ao jogador
    [SerializeField] private LayerMask enemyLayer;        // layer(s) que serão considerados inimigos
    [SerializeField] private Transform attackPoint;      // referência opcional para posicionar o ponto de ataque
    [SerializeField] private AudioClip attackClip;
    private float _lastAttackTime = -99f;

   

    [SerializeField] private AudioClip deathClip;
    private Animator _anim;
    private bool _isDead = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _stats = GetComponent<PlayerStats>();
        _stats.OnDeath += Die;  // conecta evento de morte

        // Se não tiver attackPoint setado no inspector, cria um temporário (fallback)
        if (attackPoint == null)
        {
            GameObject ap = new GameObject("AttackPoint");
            ap.transform.SetParent(transform);
            ap.transform.localPosition = _facing * attackOffset;
            attackPoint = ap.transform;
        }
    }

    private void Start()
    {
        _currentHealth = maxHealth;
        if (healthBar != null) healthBar.value = _currentHealth / maxHealth;
    }

    private void Update()
    {
        if (_isDead) return;

        // Atualiza parâmetros de animação relacionados à direção e velocidade
        if (_anim != null)
        {
            _anim.SetFloat("LookX", _facing.x);
            _anim.SetFloat("LookY", _facing.y);
            _anim.SetFloat("Speed", _moveInput.sqrMagnitude); // pode ser usado para blend tree
        }

        // Atualiza posição do ponto de ataque com base na direção de olhar
        if (attackPoint != null)
        {
            attackPoint.localPosition = _facing * attackOffset;
        }
    }

    private void FixedUpdate()
    {
        if (_isDead) return;
        Move();
    }

    private void Move()
    {
        // Normaliza para evitar movimento mais rápido nas diagonais
        Vector2 velocity = _moveInput;
        if (velocity.sqrMagnitude > 1f) velocity = velocity.normalized;

        _rb.velocity = velocity * moveSpeed;

        // Se o jogador está se movendo, atualiza a direção que ele olha
        if (velocity.magnitude > 0.1f)
        {
            _facing = velocity.normalized;
        }
    }

    // Input System callback (setar no Input Actions: Action type = Value, Control Type = Vector2)
    public void OnMove(InputValue value)
    {
        if (_isDead) return;
        _moveInput = value.Get<Vector2>();
    }

    // Input System callback para ataque (Action type = Button)
    public void OnAttack(InputValue value)
=======
public class PlayerController : MonoBehaviour
{
   
    PlayerStats _stats;
    [SerializeField] float moveSpeed;
    [SerializeField] GameObject attackPrefab1;
    [SerializeField] GameObject attackPrefab2;
    Rigidbody2D _rb;
    float xDir;
    float yDir;
    bool isRunning = false;
    bool damaging = false;
    Vector2 _moveInput;
    [SerializeField] AudioClip deathClip;
    Animator _anim;

    [SerializeField] float attackRate;     // segundos entre ataques
    [SerializeField] public float attackDamage;
    [SerializeField] LayerMask enemyLayer;        // layer(s) que serão considerados inimigos
    [SerializeField] Transform attackPoint;      // referência opcional para posicionar o ponto de ataque
    [SerializeField] AudioClip attackClip;
    private PlayerStats _playerStats;
    float _lastAttackTime = -99f;

    bool _isDead = false;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
        _stats = GetComponentInChildren<PlayerStats>();
        _playerStats = GetComponent<PlayerStats>();

    }

    void Start()
    {
        _anim.SetBool("isWalking", false);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            TakeDamage(0.5f);
        }
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return; // não recebe dano se estiver invencível ou morto

        damaging = true;                // começa invencibilidade temporária
        _playerStats.TakeDamage(damage); // aplica dano
        _anim.SetBool("Damage", true);   // inicia animação de dano

        if (_playerStats.CurrentHealth <= 0)
        {
            xDir = 0;
            yDir = 0;
            moveSpeed = 0;
            GetComponentInChildren<Collider2D>().enabled = false;
            _isDead = true;
            AudioSource.PlayClipAtPoint(deathClip, transform.position);
            _anim.SetTrigger("Destroy");
            StartCoroutine(LoadSceneAfterDelay());
            return;
        }

        // inicia coroutine para resetar o dano depois da animação
        StartCoroutine(EndDamageAfterDelay(0.6f)); // 0.6 = duração da animação de dano
    }

    private IEnumerator EndDamageAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);
        damaging = false;
        _anim.SetBool("Damage", false);
    }

    void Update()
    {
        if (_isDead) return;
    }

    void FixedUpdate()
    {
        if (_isDead) return;
        Movimentar();
    }

    System.Collections.IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(2f); // espera o tempo definido
        SceneManager.LoadScene("GameOver");     // carrega a cena
    }

    void Movimentar()
    {
        _rb.linearVelocityX = xDir * moveSpeed;
        _rb.linearVelocityY = yDir * moveSpeed;
        isRunning = Mathf.Max( Mathf.Abs(_rb.linearVelocityX), Mathf.Abs(_rb.linearVelocityY)) > Mathf.Epsilon;
        _anim.SetBool("isWalking", isRunning);
        if(isRunning)
            Flipar();
    }

    void Flipar()
    {
        transform.localScale = new Vector3(Mathf.Sign(_rb.linearVelocityX)*4, 4, 4);
    }

    void OnMove(InputValue input)
    {
        xDir = input.Get<Vector2>().x;
        yDir = input.Get<Vector2>().y;

    }

    // Input System callback para ataque (Action type = Button)
    void OnAttack(InputValue value)
>>>>>>> af165bf5ec1353f0b3db43aca4f1e26936bb0197
    {
        if (_isDead) return;
        if (value.isPressed)
        {
            TryAttack();
        }
    }

<<<<<<< HEAD
    private void TryAttack()
=======
    public void ResetDamage()
    {
        _anim.SetBool("Damage", false);
        damaging = false;
    }

    void TryAttack()
>>>>>>> af165bf5ec1353f0b3db43aca4f1e26936bb0197
    {
        if (Time.time - _lastAttackTime < attackRate) return;
        _lastAttackTime = Time.time;

        // animação de ataque (o evento de animação pode sincronizar som/effects se quiser)
<<<<<<< HEAD
        if (_anim != null) _anim.SetTrigger("Attack");

        // som de ataque
        if (attackClip != null) AudioSource.PlayClipAtPoint(attackClip, transform.position);

        // calcula ponto do ataque e detecta inimigos com OverlapCircle
        Vector2 attackPos = (attackPoint != null) ? (Vector2)attackPoint.position : (Vector2)transform.position + _facing * attackOffset;
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPos, attackRange, enemyLayer);

        foreach (var hit in hits)
        {
            // tenta chamar TakeDamage em componentes conhecidos (preferível criar uma interface IDamageable)
            var enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
            }
            else
            {
                // fallback: tenta SendMessage para métodos com nome TakeDamage
                hit.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    
    private IEnumerator LoadGameOverAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // ajuste o nome da cena se necessário
        SceneManager.LoadScene("GameOver");
    }

    // Gizmo para visualizar área de ataque no editor
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying && attackPoint == null) return;

        Vector2 pos = (attackPoint != null) ? (Vector2)attackPoint.position : (Vector2)transform.position + _facing * attackOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, (Vector3)pos);
=======
        _anim.SetBool("isAttacking", true);
        StartCoroutine(SpawnAttackAfterDelay(0.5f)); // ajusta o tempo conforme a animação

        // som de ataque
        if (attackClip != null) AudioSource.PlayClipAtPoint(attackClip, transform.position);
    }

    IEnumerator SpawnAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (Mathf.Approximately(transform.localScale.x, -4f))
            Instantiate(attackPrefab1, attackPoint.position, Quaternion.identity);
        else
            Instantiate(attackPrefab2, attackPoint.position, Quaternion.identity);

        if (attackClip != null)
            AudioSource.PlayClipAtPoint(attackClip, transform.position);
>>>>>>> af165bf5ec1353f0b3db43aca4f1e26936bb0197
    }
}
