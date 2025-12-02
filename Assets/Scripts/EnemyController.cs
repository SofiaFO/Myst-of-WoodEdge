using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float attack = 10f;
    [SerializeField] private float defense = 2f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private float attackRate = 1.5f;

    private float currentHealth;
    private float lastAttackTime = 0f;

    private Rigidbody2D _rb;
    private Transform _player;
    private PlayerStats _playerStats;

    private Animator _anim;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;

    private bool touchingPlayer = false;
    private bool damaging = false;
    private bool _isDead = false;

    private bool isKnockback = false;
    private float knockbackDuration = 0.2f;

    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip damageClip;

    [SerializeField] private GameObject xpPrefab;
    [SerializeField] private List<GameObject> moneyPrefab;

    private HashSet<Collider2D> alreadyHit = new HashSet<Collider2D>();

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        _rb.freezeRotation = true;
        _rb.mass = 0.1f;
    }

    private void Start()
    {
        currentHealth = maxHealth;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
            _playerStats = playerObj.GetComponent<PlayerStats>();
        }
    }

    private void Update()
    {
        if (_isDead || _player == null) return;

        float distance = Vector2.Distance(_player.position, transform.position);

        if (!touchingPlayer)
        {
            if (distance > attackRange)
                FollowPlayer();
            else
                TryAttack();
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
        }
    }

    private void Flipar(Vector2 direction)
    {
        if (direction.x != 0)
            _spriteRenderer.flipX = direction.x > 0;
    }

    // ============================================================
    //                INIMIGO SEGUE O PLAYER
    // ============================================================

    private void FollowPlayer()
    {
        if (_isDead || isKnockback) return;

        Vector2 dir = (_player.position - transform.position).normalized;

        _rb.linearVelocity = dir * moveSpeed;
        Flipar(dir);
    }

    // ============================================================
    //                   ATAQUE AO PLAYER
    // ============================================================

    private void TryAttack()
    {
        _rb.linearVelocity = Vector2.zero;

        if (Time.time - lastAttackTime < attackRate) return;
        lastAttackTime = Time.time;

        if (_anim) _anim.SetTrigger("Attack");

        if (attackClip != null)
            AudioSource.PlayClipAtPoint(attackClip, transform.position);

        if (_playerStats != null)
            _playerStats.TakeDamage(attack);
    }

    // ============================================================
    //                    TOMAR DANO + KNOCKBACK
    // ============================================================

    public void TakeDamage(float dmg)
    {
        if (_isDead) return;

        // Fórmula melhorada: defesa tem retorno decrescente, evita redução excessiva
        float damageReduction = defense / (defense + 50f); // máximo de ~50% de redução com defesa muito alta
        float realDamage = damage * (1 - damageReduction);
        currentHealth -= Mathf.Max(1f, realDamage); // sempre causa pelo menos 1 de dano
        damaging = true;

        damaging = true;
        if (_anim != null) _anim.SetBool("isDamage", true);
        if (damageClip) AudioSource.PlayClipAtPoint(damageClip, transform.position);

        Vector2 knockDir = ((Vector2)transform.position - (Vector2)_player.position).normalized;
        StartCoroutine(DoKnockback(knockDir, 0.8f));

        if (currentHealth <= 0)
            Die();
    }

    private IEnumerator DoKnockback(Vector2 dir, float force)
    {
        isKnockback = true;
        _rb.linearVelocity = Vector2.zero;

        _rb.AddForce(dir * force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        isKnockback = false;
        damaging = false;

        if (_anim != null) _anim.SetBool("isDamage", false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Attack")) return;

        if (!alreadyHit.Add(collision)) return;

        float dmg = _playerStats.GetAttack();
        TakeDamage(dmg);
    }

    // ============================================================
    //                        COLISÃO PLAYER
    // ============================================================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            touchingPlayer = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            touchingPlayer = false;
    }

    // ============================================================
    //                           MORTE
    // ============================================================

    private void Die()
    {
        _isDead = true;
        _rb.linearVelocity = Vector2.zero;

        if (_anim) _anim.SetTrigger("Destroy");
        if (deathClip) AudioSource.PlayClipAtPoint(deathClip, transform.position);

        foreach (var c in GetComponents<Collider2D>())
            c.enabled = false;

        // Sistema de drop melhorado: pode dropar ambos XP e moeda
        float xpChance = 0.8f;   // 80% de chance de dropar XP
        float coinChance = 0.4f; // 40% de chance de dropar moeda

        // Dropar XP
        if (Random.value < xpChance)
        {
            Instantiate(xpPrefab, transform.position, Quaternion.identity);
        }

        // Dropar moeda (independente do XP)
        if (Random.value < coinChance)
        {
            int index = Random.Range(0, moneyPrefab.Count);
            GameObject prefabToSpawn = moneyPrefab[index];
            Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
        }

        StartCoroutine(DestroyAfterDelay(0.5f));
    }

    private IEnumerator DestroyAfterDelay(float t)
    {
        yield return new WaitForSeconds(t);
        Destroy(gameObject);
    }
}
