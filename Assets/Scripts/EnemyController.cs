using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float attack = 10f;
    [SerializeField] private float defense = 2f;
    [SerializeField] private float moveSpeed = 2f;
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
        _anim = GetComponent<Animator>();
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

        UpdateAnimation();
    }

    private void FollowPlayer()
    {
        Vector2 direction = (_player.position - transform.position).normalized;
        _rb.linearVelocity = direction * moveSpeed;
    }

    private void TryAttack()
    {
        _rb.linearVelocity = Vector2.zero; // para ao atacar

        if (Time.time - lastAttackTime < attackRate) return;

        lastAttackTime = Time.time;
        if (_anim != null) _anim.SetTrigger("Attack");
        if (attackClip != null) AudioSource.PlayClipAtPoint(attackClip, transform.position);

        // verifica se o player ainda está dentro do alcance
        float distance = Vector2.Distance(transform.position, _player.position);
        if (distance <= attackRange + 0.2f && _playerStats != null)
        {
            _playerStats.TakeDamage(attack);
        }
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        float realDamage = Mathf.Max(1f, damage - defense);
        currentHealth -= realDamage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _isDead = true;
        _rb.linearVelocity = Vector2.zero;

        if (_anim != null) _anim.SetTrigger("Die");
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

    private void UpdateAnimation()
    {
        if (_anim == null) return;

        Vector2 vel = _rb.linearVelocity;
        _anim.SetFloat("Speed", vel.magnitude);
        _anim.SetFloat("MoveX", vel.x);
        _anim.SetFloat("MoveY", vel.y);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    // --- Getters para diferenciação e debug ---
    public float GetAttack() => attack;
    public float GetDefense() => defense;
    public float GetHealth() => currentHealth;
}
