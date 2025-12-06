using UnityEngine;

public class BossActivate : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Boss bossScript;

    [Header("Configuração")]
    [SerializeField] private int requiredLevel = 10;
    [SerializeField] private bool activateOnTrigger = true;
    [SerializeField] private float delayBeforeActivation = 0.5f;
    [SerializeField] private GameObject bossFight;

    [Header("Mensagens (Opcional)")]
    [SerializeField] private bool showLevelWarning = true;
    [SerializeField] private string warningMessage = "Você precisa estar no nível {0} para despertar este chefe!";

    private bool hasActivated = false;
    private EnemySpawner enemySpawner;
    private GameObject _player;

    private void Awake()
    {
        // Encontra o spawner de inimigos
        GameObject spawnerObj = GameObject.FindGameObjectWithTag("Spawner");
        _player = GameObject.FindGameObjectWithTag("Player");
        if (spawnerObj != null)
        {
            enemySpawner = spawnerObj.GetComponent<EnemySpawner>();
        }
        else
        {
            Debug.LogWarning("Spawner não encontrado! Certifique-se de que existe um GameObject com a tag 'Spawner'.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activateOnTrigger || hasActivated) return;

        if (collision.CompareTag("Attack"))
        {
            PlayerStats playerStats = _player.GetComponent<PlayerStats>();

            if (playerStats == null)
            {
                Debug.LogWarning("PlayerStats não encontrado no player!");
                return;
            }

            print("Player Level: " + playerStats.Level);

            // Verifica se o player tem o level necessário
            if (playerStats.Level >= requiredLevel)
            {
                ActivateBoss();
                print("Boss Ativado!");
            }
            else if (showLevelWarning)
            {
                Debug.Log(string.Format(warningMessage, requiredLevel));
                // Aqui você pode adicionar uma UI mostrando a mensagem
            }
        }
    }

    public void ActivateBoss()
    {
        if (hasActivated || bossScript == null) return;

        hasActivated = true;

        // PARA O SPAWN DE INIMIGOS
        StopEnemySpawning();

        Invoke(nameof(WakeUpBoss), delayBeforeActivation);

        Instantiate(
            bossFight,
            new Vector3(-18f, 7f, 0f),
            Quaternion.identity
        );
    }

    private void StopEnemySpawning()
    {
        if (enemySpawner != null)
        {
            enemySpawner.StopSpawning();
            Debug.Log("Spawn de inimigos parado para a luta do boss!");
        }
        else
        {
            Debug.LogWarning("EnemySpawner não encontrado! Não foi possível parar o spawn.");
        }
    }

    private void WakeUpBoss()
    {
        if (bossScript != null)
        {
            bossScript.WakeUp();
        }
    }

    // Método público caso você queira ativar o boss manualmente de outro script
    public void ForceActivate()
    {
        ActivateBoss();
    }
}