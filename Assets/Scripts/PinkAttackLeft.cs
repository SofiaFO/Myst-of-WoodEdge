using UnityEngine;
using System.Collections;

public class PinkAttackLeft : MonoBehaviour
{
    [SerializeField] float xSpeed = -   5f;
    [SerializeField] float moveTime = 3.5f; // tempo que se move antes de parar

    Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        StartCoroutine(MoveForwardAndStop());
    }

    IEnumerator MoveForwardAndStop()
    {
        // aplica velocidade para frente
        _rb.linearVelocity = new Vector2(xSpeed, 0f); // 0f no Y se n�o quiser subir

        // espera o tempo definido
        yield return new WaitForSeconds(moveTime);

        // para o movimento
        _rb.linearVelocity = Vector2.zero;

        // destr�i o proj�til
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject);
    }
}
