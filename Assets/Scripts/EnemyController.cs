using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float attack = 10f;
    [SerializeField] private float defense = 2f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float attackRange = 1.0f;
    [SerializeField] private float attackRate = 1.5f;
    [SerializeField] private float xpDrop = 25f;
    [SerializeField] private int moneyDrop = 10;

    private float currentHealth;
    private float lastAttackTime = 0f;
    private Rigidbody2D _rb;
    private Transform _player;
    private PlayerStats _playerStats;
    private Animator _anim;

    private bool _isDead = false;


    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip deathClip;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
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

        float distance = Vector2.Distance(transform.position, _player.position);

        if (distance > attackRange)
        {
            FollowPlayer();
        }
        else
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        _rb.linearVelocity = Vector2.zero; // para ao atacar

        if (Time.time - lastAttackTime < attackRate) return;

        lastAttackTime = Time.time;
        if (attackClip != null) AudioSource.PlayClipAtPoint(attackClip, transform.position);

        // verifica se o player ainda está dentro do alcance
        float distance = Vector2.Distance(transform.position, _player.position);
        if (distance <= attackRange + 0.2f && _player != null)
        {
            _anim.SetBool("IsAttacking", true);
        }
    }

    private bool isKnockback = false;
    private float knockbackDuration = 0.2f; // quanto tempo dura o knockback

    private void FollowPlayer()
    {
        if (_isDead || isKnockback) return; // se estiver em knockback, não move

        Vector2 direction = (_player.position - transform.position).normalized;
        _rb.linearVelocity = direction * moveSpeed;

        if (direction.x != 0)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.flipX = direction.x < 0;
        }
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        float realDamage = Mathf.Max(1f, damage - defense);
        currentHealth -= realDamage;

        if (_anim != null)
            _anim.SetBool("IsDamage", true);

        // Aplica knockback
        Vector2 knockDir = (transform.position - _player.position).normalized;
        float knockForce = 5f;

        StartCoroutine(DoKnockback(knockDir, knockForce));

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator DoKnockback(Vector2 direction, float force)
    {
        isKnockback = true;
        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(direction * force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
        isKnockback = false;
    }



    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Attack") && _playerStats != null)
        {
            TakeDamage(_playerStats.GetAttack());
        }
    }


    private void Die()
    {
        _isDead = true;
        _rb.linearVelocity = Vector2.zero;

        if (_anim != null) _anim.SetTrigger("Destroy");
        if (deathClip != null) AudioSource.PlayClipAtPoint(deathClip, transform.position);

        // desativa colisores para não interferir após a morte
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var c in colliders) c.enabled = false;

        // drop de XP e dinheiro
        if (_playerStats != null)
        {
            _playerStats.GainXP(xpDrop);
            _playerStats.AddMoney(moneyDrop);
        }

        StartCoroutine(DestroyAfterDelay(1.5f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
