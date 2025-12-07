using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    private static GameObject _playerPrefab; // Guardar referência do prefab

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
    private float _damageSoundCooldown = 0.25f;
    private float _lastDamageSoundTime = -999f;
    public bool upgrade = false;

    [SerializeField] float attackRate;
    [SerializeField] public float attackDamage;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] Transform attackPoint;
    [SerializeField] AudioClip attackClip;
    [SerializeField] AudioClip hurtClip;
    private PlayerStats _playerStats;
    float _lastAttackTime = -99f;

    bool _isDead = false;

    float baseMoveSpeed = 3f;
    float baseAttackDamage = 15f;
    float baseAttackRate = 1f;

    void Awake()
    {
        Debug.Log($"[PlayerController] Awake na cena: {SceneManager.GetActiveScene().name}, Objeto: {gameObject.name}");

        // ===== SALVAR PREFAB NA PRIMEIRA VEZ =====
        if (_playerPrefab == null)
        {
            _playerPrefab = gameObject;
            Debug.Log("[PlayerController] Prefab original salvo!");
        }

        // ===== SINGLETON =====
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[PlayerController] JÁ EXISTE Instance ({Instance.gameObject.name})! Destruindo {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log($"[PlayerController] Instance criada e marcada como DontDestroyOnLoad");

        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
        _stats = GetComponentInChildren<PlayerStats>();
        _playerStats = GetComponent<PlayerStats>();
        _gameManager = FindObjectOfType<GameManager>();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("[PlayerController] Evento sceneLoaded registrado!");
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("[PlayerController] Evento sceneLoaded removido!");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[PlayerController] ===== CENA CARREGADA: {scene.name} =====");

        if (scene.name == "PrefabScene")
        {
            // Não fazer nada aqui - o novo player será criado por RecreatePlayer()
        }
    }

    void Start()
    {
        _anim.SetBool("isWalking", false);

        if (SceneManager.GetActiveScene().name == "PrefabScene")
        {
            LoadUpgradesFromShop();
        }
    }

    void LoadUpgradesFromShop()
    {
        // Velocidade
        float moveBonus = PlayerPrefs.GetFloat("PlayerMoveSpeedBonus", 0f);
        moveSpeed = baseMoveSpeed + moveBonus;

        // Dano
        float damageBonus = PlayerPrefs.GetFloat("PlayerDamageBonus", 0f);
        attackDamage = baseAttackDamage + damageBonus;

        // Attack Speed
        float atkSpeedBonus = PlayerPrefs.GetFloat("PlayerAttackSpeedBonus", 0f);
        attackRate = Mathf.Max(0.15f, baseAttackRate - atkSpeedBonus);

        Debug.Log($"[PlayerController] Upgrades carregados - Speed: {moveSpeed}, Damage: {attackDamage}, AttackRate: {attackRate}");
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
        if (_isDead) return;
        damaging = true;
        _playerStats.TakeDamage(damage);

        if (_playerStats.CurrentHealth <= 0)
        {
            Death();
            return;
        }

        _anim.SetBool("Damage", true);
        if (Time.time - _lastDamageSoundTime >= _damageSoundCooldown)
        {
            AudioSource.PlayClipAtPoint(hurtClip, transform.position);
            _lastDamageSoundTime = Time.time;
        }

        if (_gameManager != null)
            _gameManager.AddCoins(0);

        StartCoroutine(EndDamageAfterDelay(0.6f));
    }

    public void Death()
    {
        if (_isDead) return;
        _isDead = true;

        xDir = 0;
        yDir = 0;
        moveSpeed = 0;
        _rb.linearVelocity = Vector2.zero;
        _rb.mass = float.MaxValue;

        Collider2D col = GetComponentInChildren<Collider2D>();
        if (col != null) col.enabled = false;

        AudioSource.PlayClipAtPoint(deathClip, transform.position);

        // Deletar itens temporários
        PlayerPrefs.DeleteKey("ITEM_Machado Giratório");
        PlayerPrefs.DeleteKey("ITEM_Varinha Mágica");
        PlayerPrefs.DeleteKey("ITEM_Colar Estelar");
        PlayerPrefs.DeleteKey("ITEM_Botas Chamariz");
        PlayerPrefs.Save();

        _anim.SetTrigger("Destroy");
        StartCoroutine(LoadSceneAfterDelay());
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
        yield return new WaitForSeconds(2f);

        Debug.Log("[PlayerController] Destruindo player antigo...");

        // ===== DESTRUIR COMPLETAMENTE =====
        Instance = null; // Liberar singleton
        Destroy(gameObject); // Destruir o GameObject

        SceneManager.LoadScene("GameOver");
    }

    // ===== MÉTODO ESTÁTICO PARA RECRIAR PLAYER =====
    public static void RecreatePlayer()
    {
        if (_playerPrefab == null)
        {
            Debug.LogError("[PlayerController] Prefab não encontrado! Não é possível recriar player.");
            return;
        }

        Debug.Log("[PlayerController] Recriando player do zero...");

        // Instanciar novo player na posição inicial
        GameObject newPlayer = Instantiate(_playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        newPlayer.name = "Player"; // Remover "(Clone)" do nome

        // O Awake do novo player vai configurar o singleton automaticamente

        Debug.Log("[PlayerController] Player recriado com sucesso!");
    }

    void Movimentar()
    {
        _rb.linearVelocityX = xDir * moveSpeed;
        _rb.linearVelocityY = yDir * moveSpeed;
        isRunning = Mathf.Max(Mathf.Abs(_rb.linearVelocityX), Mathf.Abs(_rb.linearVelocityY)) > Mathf.Epsilon;
        _anim.SetBool("isWalking", isRunning);
        if (isRunning)
            Flipar();
    }

    void Flipar()
    {
        transform.localScale = new Vector3(Mathf.Sign(_rb.linearVelocityX) * 4, 4, 4);
    }

    void OnMove(InputValue input)
    {
        xDir = input.Get<Vector2>().x;
        yDir = input.Get<Vector2>().y;
    }

    void OnAttack(InputValue value)
    {
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

        _anim.SetBool("isAttacking", true);
        StartCoroutine(SpawnAttackAfterDelay(0.5f));

        if (attackClip != null)
            AudioSource.PlayClipAtPoint(attackClip, transform.position);
    }

    IEnumerator SpawnAttackAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);

    if (upgrade)
    {
        if (Mathf.Approximately(transform.localScale.x, -4f))
        {
            // Projétil 1 vai para a ESQUERDA
            GameObject attack1 = Instantiate(attackPrefab1, attackPoint.position, Quaternion.identity);
            Attack attackScript1 = attack1.GetComponent<Attack>();
            if (attackScript1 != null) attackScript1.SetDirection(-1f); // ESQUERDA

            // Projétil 2 vai para a DIREITA
            Vector3 oppositePosition = new Vector3(attackPoint.position.x + 3f, attackPoint.position.y, attackPoint.position.z);
            GameObject attack2 = Instantiate(attackPrefab2, oppositePosition, Quaternion.identity);
            Attack attackScript2 = attack2.GetComponent<Attack>();
            if (attackScript2 != null) attackScript2.SetDirection(1f); // DIREITA
        }
        else
        {
            // Projétil 2 vai para a DIREITA
            GameObject attack2 = Instantiate(attackPrefab2, attackPoint.position, Quaternion.identity);
            Attack attackScript2 = attack2.GetComponent<Attack>();
            if (attackScript2 != null) attackScript2.SetDirection(1f); // DIREITA

            // Projétil 1 vai para a ESQUERDA
            Vector3 oppositePosition = new Vector3(attackPoint.position.x - 3f, attackPoint.position.y, attackPoint.position.z);
            GameObject attack1 = Instantiate(attackPrefab1, oppositePosition, Quaternion.identity);
            Attack attackScript1 = attack1.GetComponent<Attack>();
            if (attackScript1 != null) attackScript1.SetDirection(-1f); // ESQUERDA
        }
    }
    else
    {
        // Ataque normal - segue a direção do player
        float direction = Mathf.Approximately(transform.localScale.x, -4f) ? -1f : 1f;
        
        if (Mathf.Approximately(transform.localScale.x, -4f))
        {
            GameObject attack = Instantiate(attackPrefab1, attackPoint.position, Quaternion.identity);
            Attack attackScript = attack.GetComponent<Attack>();
            if (attackScript != null) attackScript.SetDirection(direction);
        }
        else
        {
            GameObject attack = Instantiate(attackPrefab2, attackPoint.position, Quaternion.identity);
            Attack attackScript = attack.GetComponent<Attack>();
            if (attackScript != null) attackScript.SetDirection(direction);
        }
    }

    if (attackClip != null)
        AudioSource.PlayClipAtPoint(attackClip, transform.position);
}

    public void UpgradeAttack()
    {
        upgrade = true;
        attackDamage += 5f;
    }
}