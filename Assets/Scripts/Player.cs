using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
   
    PlayerStats _stats;
    [SerializeField] float moveSpeed;
    [SerializeField] GameObject attackPrefab1;
    [SerializeField] GameObject attackPrefab2;
    private GameManager _gameManager;
    Rigidbody2D _rb;
    float xDir;
    float yDir;
    bool isRunning = false;
    bool damaging = false;
    Vector2 _moveInput;
    [SerializeField] AudioClip deathClip;
    Animator _anim;
    private float _damageSoundCooldown = 0.25f; // 250ms de intervalo
    private float _lastDamageSoundTime = -999f; // tempo do último som
    public bool upgrade = false;


    [SerializeField] float attackRate;     // segundos entre ataques
    [SerializeField] public float attackDamage;
    [SerializeField] LayerMask enemyLayer;        // layer(s) que serão considerados inimigos
    [SerializeField] Transform attackPoint;      // referência opcional para posicionar o ponto de ataque
    [SerializeField] AudioClip attackClip;
    [SerializeField] AudioClip hurtClip;
    private PlayerStats _playerStats;
    float _lastAttackTime = -99f;

    bool _isDead = false;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
        _stats = GetComponentInChildren<PlayerStats>();
        _playerStats = GetComponent<PlayerStats>();
        _gameManager = FindObjectOfType<GameManager>();

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
        if (_playerStats.CurrentHealth <= 0)
        {
            Death();
            return;
        }

        _anim.SetBool("Damage", true);   // inicia animação de dano
        if (Time.time - _lastDamageSoundTime >= _damageSoundCooldown)
        {
            AudioSource.PlayClipAtPoint(hurtClip, transform.position);
            _lastDamageSoundTime = Time.time;
        }

        _gameManager.AddCoins(_playerStats.Money);
        // inicia coroutine para resetar o dano depois da animação
        StartCoroutine(EndDamageAfterDelay(0.6f)); // 0.6 = duração da animação de dano
        
    }

    public void Death()
    {
        xDir = 0;
        yDir = 0;
        moveSpeed = 0;
        _rb.linearVelocity = Vector2.zero;
        _rb.mass = float.MaxValue;
        GetComponentInChildren<Collider2D>().enabled = false;
        _isDead = true;
        AudioSource.PlayClipAtPoint(deathClip, transform.position);
        PlayerPrefs.DeleteKey("ITEM_Machado Giratório");
        PlayerPrefs.DeleteKey("ITEM_Varinha Mágica");
        PlayerPrefs.DeleteKey("ITEM_Colar Estelar");
        PlayerPrefs.DeleteKey("ITEM_Botas Chamariz");
        PlayerPrefs.Save();
        _anim.SetTrigger("Destroy");
        StartCoroutine(LoadSceneAfterDelay());
        return;
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
        Destroy(gameObject); // destrói o objeto após 2 segundos
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
    {
        Debug.Log("Machado level: " + PlayerPrefs.GetInt("ITEM_Machado Giratório", 0));
        if (_isDead) return;
        if (value.isPressed)
        {
            TryAttack();
        }
    }

    public void ResetDamage()
    {
        _anim.SetBool("Damage", false);
        damaging = false;
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

        if (upgrade)
        {
            // MODO UPGRADE: Spawna dos dois lados
            if (Mathf.Approximately(transform.localScale.x, -4f))
            {
                // Virado para ESQUERDA - spawna um no attackPoint e outro 4 unidades à DIREITA
                Instantiate(attackPrefab1, attackPoint.position, Quaternion.identity);

                Vector3 oppositePosition = new Vector3(attackPoint.position.x + 3f, attackPoint.position.y, attackPoint.position.z);
                Instantiate(attackPrefab2, oppositePosition, Quaternion.identity);
            }
            else
            {
                // Virado para DIREITA - spawna um no attackPoint e outro 4 unidades à ESQUERDA
                Instantiate(attackPrefab2, attackPoint.position, Quaternion.identity);

                Vector3 oppositePosition = new Vector3(attackPoint.position.x - 3f, attackPoint.position.y, attackPoint.position.z);
                Instantiate(attackPrefab1, oppositePosition, Quaternion.identity);
                print("Ataque UPGRADE para direita!");
            }
        }
        else
        {
            // MODO NORMAL: Spawna baseado na direção
            if (Mathf.Approximately(transform.localScale.x, -4f))
                Instantiate(attackPrefab1, attackPoint.position, Quaternion.identity);
            else
                Instantiate(attackPrefab2, attackPoint.position, Quaternion.identity);
        }

        if (attackClip != null)
            AudioSource.PlayClipAtPoint(attackClip, transform.position);
    }

    public void UpgradeAttack()
    {
        upgrade = true;
        print("Upgrade ativado!");
        attackDamage += 5f;
    }
}
