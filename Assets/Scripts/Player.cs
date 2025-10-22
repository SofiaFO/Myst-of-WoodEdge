using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
   
    PlayerStats _stats;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] GameObject attackPrefab1;
    [SerializeField] GameObject attackPrefab2;
    Rigidbody2D _rb;
    float xDir;
    float yDir;
    bool isRunning = false;
    Vector2 _moveInput;
    float healthBar;
    float maxHealth;
    float _currentHealth;
    [SerializeField] AudioClip deathClip;
    Animator _anim;

    [SerializeField] float attackRate = 0.6f;     // segundos entre ataques
    [SerializeField] float attackDamage = 20f;
    [SerializeField] float attackRange = 0.6f;    // raio do círculo de ataque
    [SerializeField] float attackOffset = 0.75f;  // distância do ponto de ataque em relação ao jogador
    [SerializeField] LayerMask enemyLayer;        // layer(s) que serão considerados inimigos
    [SerializeField] Transform attackPoint;      // referência opcional para posicionar o ponto de ataque
    [SerializeField] AudioClip attackClip;
    float _lastAttackTime = -99f;

    bool _isDead = false;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
        _stats = GetComponentInChildren<PlayerStats>();

        
    }

    void Start()
    {
        _anim.SetBool("isWalking", false);
        _currentHealth = maxHealth;
        if (healthBar != null) healthBar = _currentHealth / maxHealth;
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
    {
        if (_isDead) return;
        if (value.isPressed)
        {
            TryAttack();
        }
    }

    void TryAttack()
    {
        if (Time.time - _lastAttackTime < attackRate) return;
        _lastAttackTime = Time.time;

        // animação de ataque (o evento de animação pode sincronizar som/effects se quiser)
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
    }
}
