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

    private bool isSoftPushed = false;
    private float softPushDuration = 0.15f;

    private bool touchingPlayer = false;
    [SerializeField] private float softPushForce = 4f;
    [SerializeField] private float autoNudgeForce = 1.5f;

    private float currentHealth;
    private float lastAttackTime = 0f;

    private Rigidbody2D _rb;
    private Transform _player;
    [SerializeField] private GameObject playerSerie;
    private PlayerStats _playerStats;

    private Animator _anim;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;

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

    // ============================================================
    //          SISTEMA DE DESVIO DE OBSTÁCULOS
    // ============================================================
    [Header("Sistema de Desvio")]
    [SerializeField] private float obstacleDetectionDistance = 1.5f;
    [SerializeField] private float sideRayDistance = 1.0f;
    [SerializeField] private float stuckCheckTime = 0.5f;
    [SerializeField] private float minMovementThreshold = 0.1f;
    
    private Vector2 lastPosition;
    private float stuckTimer = 0f;
    private bool isAvoiding = false;
    private Vector2 avoidanceDirection;
    private float avoidanceTimer = 0f;
    private float avoidanceDuration = 1.0f;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _rb.mass = 0.1f;
        
        lastPosition = transform.position;
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
        else if (playerSerie != null)
        {
            _player = playerSerie.transform;
            _playerStats = playerObj.GetComponent<PlayerStats>();
            if (_playerStats == null)
                _playerStats = playerObj.GetComponentInChildren<PlayerStats>();

        }
        else
        {
            Debug.LogError("Player não atribuído no Inspector!");
        }
    }


    private void Update()
    {
        if (_isDead) return;

        if (_player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                _player = playerObj.transform;
                _playerStats = playerObj.GetComponent<PlayerStats>();
            }
            else
            {
                _rb.linearVelocity = Vector2.zero;
                return;
            }
        }

        if (_collider == null)
            _collider = GetComponent<Collider2D>();

        float distance = Vector2.Distance(_player.position, _collider.bounds.center);

        // Verifica se está preso
        CheckIfStuck();

        if (!touchingPlayer)
        {
            if (distance > attackRange)
                FollowPlayer();
            else
                TryAttack();
        }
        else if (!isSoftPushed)
        {
            _rb.linearVelocity = Vector2.zero;
        }
    }


    private void Flipar(Vector2 direction)
    {
        if (direction.x != 0)
            _spriteRenderer.flipX = direction.x > 0;
    }


    private void CheckIfStuck()
    {
        if (isKnockback || _isDead) return;

        float distanceMoved = Vector2.Distance(transform.position, lastPosition);
        
        if (distanceMoved < minMovementThreshold && _rb.linearVelocity.magnitude > 0.1f)
        {
            stuckTimer += Time.deltaTime;
            
            if (stuckTimer >= stuckCheckTime)
            {
                // Está preso! Força uma nova direção de desvio
                ForceAvoidance();
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }
        
        lastPosition = transform.position;
    }


    private void ForceAvoidance()
    {
        Vector2 toPlayer = ((Vector2)_player.position - (Vector2)transform.position).normalized;
        Vector2 perpendicular = Vector2.Perpendicular(toPlayer);
        
        // Escolhe aleatoriamente esquerda ou direita
        if (Random.value > 0.5f)
            perpendicular = -perpendicular;
            
        isAvoiding = true;
        avoidanceDirection = perpendicular;
        avoidanceTimer = avoidanceDuration;
        
        Debug.Log($"{gameObject.name} está preso! Forçando desvio.");
    }


    private void FollowPlayer()
    {
        if (_isDead || isKnockback) return;

        Vector2 origin = (Vector2)_collider.bounds.center;
        Vector2 targetDir = ((Vector2)_player.position - origin).normalized;

        // Se está em modo de desvio, continua desviando
        if (isAvoiding)
        {
            avoidanceTimer -= Time.deltaTime;
            
            if (avoidanceTimer <= 0f)
            {
                isAvoiding = false;
            }
            else
            {
                // Mistura a direção de desvio com a direção do player
                Vector2 blendedDirection = (avoidanceDirection * 0.7f + targetDir * 0.3f).normalized;
                _rb.linearVelocity = blendedDirection * moveSpeed;
                Flipar(blendedDirection);
                return;
            }
        }

        // Empurra o raio um pouco para frente
        Vector2 rayOrigin = origin + targetDir * 0.3f;
        
        // Raycast central
        RaycastHit2D centerHit = Physics2D.Raycast(rayOrigin, targetDir, obstacleDetectionDistance, LayerMask.GetMask("Default"));
        
        // Raycasts laterais
        Vector2 perpLeft = Vector2.Perpendicular(targetDir);
        Vector2 perpRight = -perpLeft;
        
        RaycastHit2D leftHit = Physics2D.Raycast(rayOrigin, (targetDir + perpLeft).normalized, sideRayDistance, LayerMask.GetMask("Default"));
        RaycastHit2D rightHit = Physics2D.Raycast(rayOrigin, (targetDir + perpRight).normalized, sideRayDistance, LayerMask.GetMask("Default"));

        // Visualização dos raycasts
        Debug.DrawRay(rayOrigin, targetDir * obstacleDetectionDistance, Color.red);
        Debug.DrawRay(rayOrigin, (targetDir + perpLeft).normalized * sideRayDistance, Color.blue);
        Debug.DrawRay(rayOrigin, (targetDir + perpRight).normalized * sideRayDistance, Color.green);

        Vector2 finalDirection = targetDir;

        // Detectou obstáculo à frente
        if (centerHit.collider != null && centerHit.collider.CompareTag("Tronco"))
        {
            bool leftBlocked = leftHit.collider != null && leftHit.collider.CompareTag("Tronco");
            bool rightBlocked = rightHit.collider != null && rightHit.collider.CompareTag("Tronco");

            if (!leftBlocked && !rightBlocked)
            {
                // Ambos os lados livres - escolhe o lado mais próximo do player
                Vector2 toPlayer = (Vector2)_player.position - origin;
                float dotLeft = Vector2.Dot(toPlayer, perpLeft);
                
                if (dotLeft > 0)
                    finalDirection = perpLeft;
                else
                    finalDirection = perpRight;
                    
                // Inicia modo de desvio
                isAvoiding = true;
                avoidanceDirection = finalDirection;
                avoidanceTimer = avoidanceDuration * 0.5f;
            }
            else if (!leftBlocked)
            {
                finalDirection = perpLeft;
                isAvoiding = true;
                avoidanceDirection = finalDirection;
                avoidanceTimer = avoidanceDuration * 0.5f;
            }
            else if (!rightBlocked)
            {
                finalDirection = perpRight;
                isAvoiding = true;
                avoidanceDirection = finalDirection;
                avoidanceTimer = avoidanceDuration * 0.5f;
            }
            else
            {
                // Ambos bloqueados - tenta voltar
                finalDirection = -targetDir;
                ForceAvoidance();
            }
        }
        // Obstáculo só na esquerda
        else if (leftHit.collider != null && leftHit.collider.CompareTag("Tronco"))
        {
            finalDirection = (targetDir + perpRight * 0.5f).normalized;
        }
        // Obstáculo só na direita
        else if (rightHit.collider != null && rightHit.collider.CompareTag("Tronco"))
        {
            finalDirection = (targetDir + perpLeft * 0.5f).normalized;
        }

        _rb.linearVelocity = finalDirection * moveSpeed;
        Flipar(finalDirection);
    }


    private void TryAttack()
    {
        _rb.linearVelocity = Vector2.zero;
        if (Time.time - lastAttackTime < attackRate) return;

        lastAttackTime = Time.time;
    }


    // ============================================================
    //                TAKE DAMAGE + KNOCKBACK SUAVE
    // ============================================================

    public void TakeDamage(float dmg)
    {
        if (_isDead) return;

        float finalDamage = Mathf.Max(1f, dmg - defense);
        currentHealth -= finalDamage;

        damaging = true;
        if (_anim != null) _anim.SetBool("isDamage", true);
        if (damageClip != null) AudioSource.PlayClipAtPoint(damageClip, transform.position);

        // calcula direção do knockback (inimigo ← ataque)
        Vector2 knockDir = ((Vector2)transform.position - (Vector2)_player.position).normalized;

        // Inicia knockback leve
        StartCoroutine(DoKnockback(knockDir, 0.8f));

        if (currentHealth <= 0)
            Die();
    }


    private IEnumerator DoKnockback(Vector2 dir, float force)
    {
        if (_isDead) yield break;

        isKnockback = true;
        isAvoiding = false; // Cancela desvio durante knockback
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

        if (_playerStats == null)
        {
            Debug.LogError("❌ _playerStats é NULL! O inimigo tomou dano mas não sabe quem é o player!");
            return;
        }

        if (alreadyHit.Contains(collision)) return;
        alreadyHit.Add(collision);
        if (_playerStats == null) {
            GameObject playerObjx = GameObject.FindGameObjectWithTag("Player");
            if (playerObjx != null)
            {
                _playerStats = playerObjx.GetComponent<PlayerStats>();
            }
        }
        float dmg = _playerStats.GetAttack();
        TakeDamage(dmg);
    }




    // ============================================================
    //                 COLISÃO PLAYER → SOFT PUSH SUAVE
    // ============================================================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Detecta colisão com obstáculos
        if (collision.collider.CompareTag("Tronco"))
        {
            // Calcula direção de repulsão do obstáculo
            Vector2 awayFromObstacle = ((Vector2)transform.position - (Vector2)collision.transform.position).normalized;
            Vector2 toPlayer = ((Vector2)_player.position - (Vector2)transform.position).normalized;
            
            // Combina as direções para desviar do obstáculo na direção geral do player
            Vector2 avoidDir = (awayFromObstacle * 0.6f + Vector2.Perpendicular(toPlayer) * 0.4f).normalized;
            
            isAvoiding = true;
            avoidanceDirection = avoidDir;
            avoidanceTimer = avoidanceDuration;
            
            // Aplica um pequeno impulso para sair do obstáculo
            _rb.AddForce(awayFromObstacle * 2f, ForceMode2D.Impulse);
            
            return;
        }

        if (!collision.collider.CompareTag("Player")) return;

        if (_rb.linearVelocity.sqrMagnitude < 0.01f) return;

        touchingPlayer = true;

        Vector2 enemyC = _collider.bounds.center;
        Vector2 playerC = collision.collider.bounds.center;

        Vector2 delta = enemyC - playerC;
        float dx = Mathf.Abs(delta.x);
        float dy = Mathf.Abs(delta.y);

        Vector2 pushDir;

        if (dx > dy)
            pushDir = new Vector2(0, Mathf.Sign(delta.y));
        else
            pushDir = new Vector2(Mathf.Sign(delta.x), 0);

        StartCoroutine(SoftPush(pushDir));
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        // Continua tentando sair se ainda estiver colidindo com obstáculo
        if (collision.collider.CompareTag("Tronco") && !isKnockback)
        {
            Vector2 awayFromObstacle = ((Vector2)transform.position - (Vector2)collision.transform.position).normalized;
            _rb.AddForce(awayFromObstacle * 1.5f, ForceMode2D.Force);
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            touchingPlayer = false;
    }


    private IEnumerator SoftPush(Vector2 dir)
    {
        isSoftPushed = true;

        float f = 0.4f;
        _rb.AddForce(dir.normalized * f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(softPushDuration);
        isSoftPushed = false;
    }


    // ============================================================

    private void Die()
    {
        _isDead = true;
        _rb.linearVelocity = Vector2.zero;

        if (_anim != null) _anim.SetTrigger("Destroy");
        if (deathClip != null) AudioSource.PlayClipAtPoint(deathClip, transform.position);

        foreach (var c in GetComponents<Collider2D>())
            c.enabled = false;

        float chance = Random.value;

        if (chance < 0.7f)
            Instantiate(xpPrefab, transform.position, Quaternion.identity);
        else
            Instantiate(moneyPrefab[Random.Range(0, moneyPrefab.Count)], transform.position, Quaternion.identity);

        StartCoroutine(DestroyAfterDelay(0.5f));
    }


    private IEnumerator DestroyAfterDelay(float d)
    {
        yield return new WaitForSeconds(d);
        Destroy(gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Visualiza a área de detecção de obstáculos
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, obstacleDetectionDistance);
    }
}