using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Boss : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 500f;
    [SerializeField] private float attack = 30f;
    [SerializeField] private float defense = 20f;
    [SerializeField] private float moveSpeed = 0.3f;
    [SerializeField] private Slider healthBar;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 2.5f;

    [Header("Immune Chance")]
    [SerializeField] private float immuneChance = 0.25f;

    [Header("Immune Settings")]
    [SerializeField] private float immuneDuration = 1.2f;

    [Header("Prefabs")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPointRight;
    [SerializeField] private Transform projectileSpawnPointLeft;
    [SerializeField] private GameObject laserObject;
    [SerializeField] private float laserDuration = 1f;

    [Header("Posicionamento")]
    [SerializeField] private float desiredXDistance = 2f;
    [SerializeField] private float xAlignmentThreshold = 0.3f;
    [SerializeField] private float dodgeSpeed = 8f;

    [Header("Drops")]
    [SerializeField] private GameObject xpPrefab;
    [SerializeField] private List<GameObject> moneyPrefab;

    [Header("Audio")]
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip attackLaserClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip damageClip;

    private float currentHealth;
    private float attackTimer;

    private Rigidbody2D _rb;
    private Transform _player;
    private PlayerStats _playerStats;
    private PlayerController _playerController;
    private Animator _anim;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;

    private bool _isDead = false;
    private bool isKnockback = false;
    private bool isImmune = false;
    private bool isAttacking = false;
    private bool isDodging = false;
    private bool isAwake = false;
    private bool nextAttackIsLaser = true;

    private float knockbackDuration = 0.2f;
    private HashSet<Collider2D> alreadyHit = new HashSet<Collider2D>();

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _rb.freezeRotation = true;
        _rb.mass = 1f;
    }

    private void Start()
    {
        currentHealth = maxHealth;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;

            _playerStats = playerObj.GetComponent<PlayerStats>();

            _playerController = playerObj.GetComponent<PlayerController>();
        }


        // Começa dormindo com animação de imune e como Kinematic (não pode ser empurrado)
        isImmune = true;
        isAwake = false;
        _anim.SetBool("Immune", true);
        _rb.bodyType = RigidbodyType2D.Kinematic;
    }

    // Função para acordar o boss (chamada externamente)
    public void WakeUp()
    {
        if (isAwake) return;

        isAwake = true;
        StartCoroutine(WakeUpSequence());
        healthBar.gameObject.SetActive(true);
        healthBar.value = maxHealth;

    }

    private IEnumerator WakeUpSequence()
    {
        yield return new WaitForSeconds(0.5f);

        isImmune = false;
        _anim.SetBool("Immune", false);

        // Volta ao normal quando acordar
        _rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void Update()
    {
        if (_isDead || _player == null || !isAwake) return;

        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        // Se estiver atacando, não faz nada (fica parado)
        if (isAttacking)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        float distX = Mathf.Abs(_player.position.x - transform.position.x);

        // VERIFICA SE O PLAYER ESTÁ MUITO ALINHADO EM X
        if (distX <= xAlignmentThreshold && !isDodging)
        {
            StartCoroutine(DodgeToSafeDistance());
            return;
        }

        // MOVIMENTO NORMAL: Busca estar alinhado em Y e a uma distância em X
        if (!isDodging && !isKnockback && !isImmune)
        {
            MoveToIdealPosition();
        }

        // ATACA SE ESTIVER NO RANGE E O TIMER PERMITIR
        float distance = Vector2.Distance(_player.position, transform.position);
        if (distance <= attackRange && attackTimer <= 0 && !isImmune && !isDodging)
        {
            TryAttack();
        }
    }

    private void MoveToIdealPosition()
    {
        Vector2 currentPos = _rb.position;

        // SEMPRE ALINHA NO Y DO PLAYER
        float newY = Mathf.MoveTowards(
            currentPos.y,
            _player.position.y,
            moveSpeed * 2f * Time.deltaTime
        );

        // BUSCA FICAR A desiredXDistance DO PLAYER EM X
        float distX = _player.position.x - currentPos.x;
        float absDist = Mathf.Abs(distX);

        float newX = currentPos.x;

        // Se estiver mais longe que a distância desejada, se aproxima
        if (absDist > desiredXDistance + 0.1f)
        {
            float dirX = Mathf.Sign(distX);
            newX = currentPos.x + dirX * moveSpeed * Time.deltaTime;
        }
        // Se estiver mais perto que a distância desejada, se afasta
        else if (absDist < desiredXDistance - 0.1f)
        {
            float dirX = -Mathf.Sign(distX);
            newX = currentPos.x + dirX * moveSpeed * Time.deltaTime;
        }

        Vector2 newPos = new Vector2(newX, newY);
        _rb.position = newPos;

        // Flip baseado na direção do player
        Flipar(new Vector2(distX, 0));
    }

    private void Flipar(Vector2 direction)
    {
        if (direction.x != 0)
        {
            // Flip: se o player está à direita (direction.x > 0), NÃO flipa
            // se está à esquerda (direction.x < 0), flipa
            _spriteRenderer.flipX = direction.x < 0;

            // Flipa o laser também
            if (laserObject != null)
            {
                Vector3 laserScale = laserObject.transform.localScale;
                laserScale.x = Mathf.Abs(laserScale.x) * (direction.x < 0 ? -1 : 1);
                laserObject.transform.localScale = laserScale;
            }
        }
    }

    private IEnumerator DodgeToSafeDistance()
    {
        isDodging = true;
        isImmune = true;
        _anim.SetBool("Immune", true);

        // Trava o Rigidbody para não ser empurrado durante o dodge
        _rb.bodyType = RigidbodyType2D.Kinematic;

        // Determina para qual lado se afastar (2.5 de distância)
        float dodgeDirX = Mathf.Sign(transform.position.x - _player.position.x);

        // Se estiver exatamente em cima, escolhe um lado aleatório
        if (dodgeDirX == 0)
            dodgeDirX = Random.value < 0.5f ? -1f : 1f;

        float targetX = _player.position.x + (2.5f * dodgeDirX);
        float targetY = _player.position.y;

        Vector2 targetPos = new Vector2(targetX, targetY);

        // Move rapidamente para a posição alvo
        while (Vector2.Distance(_rb.position, targetPos) > 0.05f)
        {
            _rb.position = Vector2.MoveTowards(
                _rb.position,
                targetPos,
                dodgeSpeed * Time.deltaTime
            );

            // Atualiza o Y do alvo caso o player se mova
            targetY = _player.position.y;
            targetPos = new Vector2(targetX, targetY);

            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        isImmune = false;
        _anim.SetBool("Immune", false);
        isDodging = false;

        // Volta ao normal
        _rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void TryAttack()
    {
        attackTimer = attackCooldown;

        // Garante que está flipado na direção do player antes de atacar
        Vector2 dirToPlayer = new Vector2(_player.position.x - transform.position.x, 0);
        Flipar(dirToPlayer);

        isAttacking = true;
        _rb.linearVelocity = Vector2.zero;

        // Alterna entre laser e projétil
        if (nextAttackIsLaser)
        {
            StartCoroutine(LaserAttack());
        }
        else
        {
            StartCoroutine(ProjectileAttack());
        }

        nextAttackIsLaser = !nextAttackIsLaser;

        if (Random.value <= immuneChance)
            StartCoroutine(ActivateImmune());
    }

    private IEnumerator LaserAttack()
    {
        _anim.SetBool("AttackLaser", true);

        if (attackLaserClip)
            AudioSource.PlayClipAtPoint(attackLaserClip, transform.position);

        SpawnLaser();

        yield return new WaitForSeconds(laserDuration);

        laserObject.SetActive(false);
        _anim.SetBool("AttackLaser", false);
        isAttacking = false;
    }

    private IEnumerator ProjectileAttack()
    {
        _anim.SetBool("Attack1", true);

        if (attackClip)
            AudioSource.PlayClipAtPoint(attackClip, transform.position);

        // Espera um pouco antes de spawnar o projétil (meio da animação)
        yield return new WaitForSeconds(0.3f);

        SpawnProjectile();

        // Espera o resto da animação
        yield return new WaitForSeconds(0.5f);

        _anim.SetBool("Attack1", false);
        isAttacking = false;
    }

    private IEnumerator ActivateImmune()
    {
        isImmune = true;
        _anim.SetBool("Immune", true);

        // Trava o Rigidbody para não ser empurrado
        _rb.bodyType = RigidbodyType2D.Kinematic;

        yield return new WaitForSeconds(immuneDuration);

        if (!isDodging)
        {
            isImmune = false;
            _anim.SetBool("Immune", false);
            // Volta ao normal
            _rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    public void SpawnLaser()
    {
        laserObject.SetActive(true);
    }

    private void SpawnProjectile()
    {
        if (projectilePrefab == null) return;


        Vector2 dir = new Vector2(_player.position.x - transform.position.x, 0);

        if (dir.x >= 0)
        {
            Instantiate(
                projectilePrefab,
                projectileSpawnPointRight.position,
                Quaternion.Euler(0f, 0f, 0f) // direita
            );
        }
        else
        {
            Instantiate(
                projectilePrefab,
                projectileSpawnPointLeft.position,
                Quaternion.Euler(0f, 180f, 0f) // esquerda
            );
        }

    }

    public void TakeDamage(float dmg)
    {
        if (_isDead || isImmune) return;

        float finalDamage = Mathf.Max(1f, dmg - defense);
        currentHealth -= finalDamage;
        healthBar.value = currentHealth;

        if (damageClip)
            AudioSource.PlayClipAtPoint(damageClip, transform.position);

        if (currentHealth <= 0)
            Die();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Attack")) return;
        if (!alreadyHit.Add(collision)) return;

        float dmg = _playerStats.GetAttack();
        TakeDamage(dmg);
    }

    private void Die()
    {
        _isDead = true;
        _rb.linearVelocity = Vector2.zero;

        if (_anim) _anim.SetTrigger("Death");
        if (deathClip) AudioSource.PlayClipAtPoint(deathClip, transform.position);

        foreach (var c in GetComponents<Collider2D>())
            c.enabled = false;

        float chance = Random.value;

        if (chance < 0.7f)
            Instantiate(xpPrefab, transform.position, Quaternion.identity);
        else
            Instantiate(moneyPrefab[Random.Range(0, moneyPrefab.Count)], transform.position, Quaternion.identity);

        StartCoroutine(DestroyAfterDelay(1.5f));
    }

    private IEnumerator DestroyAfterDelay(float t)
    {
        yield return new WaitForSeconds(t);
        Destroy(gameObject);
        SceneManager.LoadScene("GameWin");     // carrega a cena
    }
}