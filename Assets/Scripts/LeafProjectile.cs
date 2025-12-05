using UnityEngine;

public class LeafProjectile : MonoBehaviour
{
    public float damage = 5f;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            // Se tiver script de inimigo, cause dano
            EnemyController enemy = col.GetComponent<EnemyController>();
            if (enemy != null)
                enemy.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}