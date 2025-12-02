using UnityEngine;
using System.Collections;

public class MachadoGir : MonoBehaviour
{
    public Transform player;
    public Transform axeSprite;

    public float radius = 0.4f;     // bem perto do player
    public float orbitSpeed = 4f;   // velocidade da órbita (radianos/sec)
    public float spinSpeed = 720f;  // machado girando no próprio eixo
    public float damage = 10f;
    public float activeDuration = 4f;
    public float inactiveDuration = 10f;

    private bool isActive = false;
    private float orbitAngle;

    private void Awake()
    {
        if (!isActive)
            gameObject.SetActive(false);
        player = GameObject.FindWithTag("Player").transform;
    }

    private void Start()
    {
        StartCoroutine(Cycle());
    }

    private void FixedUpdate()
    {
        if (!isActive) return;

        // incrementa ângulo (controla a órbita)
        orbitAngle += orbitSpeed * Time.deltaTime;

        // posição orbital — SEM depender de RotateAround
        Vector3 offset = new Vector3(
            Mathf.Cos(orbitAngle) * radius,
            Mathf.Sin(orbitAngle) * radius,
            0f
        );

        // coloca o machado exatamente próximo do player, sempre correto
        transform.position = player.position + offset;

        // gira no próprio eixo (efeito de serra)
        axeSprite.Rotate(0, 0, spinSpeed * Time.deltaTime);
    }

    private IEnumerator Cycle()
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        Collider2D col = GetComponent<Collider2D>();

        while (true)
        {
            // ATIVAR SOMENTE SPRITE E COLISOR
            isActive = true;
            if (sr) sr.enabled = true;
            if (col) col.enabled = true;

            orbitAngle = 0;

            yield return new WaitForSeconds(activeDuration);

            // DESATIVAR SOMENTE SPRITE E COLISOR
            isActive = false;
            if (sr) sr.enabled = false;
            if (col) col.enabled = false;

            yield return new WaitForSeconds(inactiveDuration);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    public void Upgrade()
    {
        damage += 3f;
        orbitSpeed+= 0.3f;
    }
}
