using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    [Header("Status Base")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float attack = 20f;
    [SerializeField] private float defense;
    [SerializeField] private int level = 1;
    [SerializeField] private float currentXP = 0f;
    [SerializeField] private float xpToNextLevel = 10f;
    [SerializeField] private int money = 0;
    [SerializeField] private float moneyMultiplier;
    [SerializeField] private AudioClip bossClip;
    [SerializeField] private AudioClip battleClip;

    [Header("Sistema de Câmera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject cinemachineVirtualCamera; // Arraste aqui a CM vcam1
    [SerializeField] private Vector3 targetCameraPosition = new Vector3(-0.34f, 5.04f, -10f);
    [SerializeField] private float cameraTransitionDuration = 1.5f;
    [SerializeField] private GameObject objectsParent; // Prefab/objeto pai com vários filhos
    [SerializeField] private int objectsToDisable = 2;

    private XpBar _xpBar;
    ItemRandomScript1 itemRandom1;
    ItemRandomScript2 itemRandom2;
    ItemRandomScript3 itemRandom3;
    GameObject Card1;
    GameObject Card2;
    GameObject Card3;
    GameObject CardUI;

    private float currentHealth;
    private Vector3 originalCameraPosition;
    private bool isCameraTransitioning = false;

    [Header("Referências de UI (opcional)")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Text xpText;
    [SerializeField] private Text moneyText;

    public static PlayerStats Instance;

    private void Awake()
    {
        CardUI = GameObject.FindWithTag("CardUI");
        Card1 = GameObject.FindWithTag("Card1");
        Card2 = GameObject.FindWithTag("Card2");
        Card3 = GameObject.FindWithTag("Card3");
        if (CardUI != null)
            CardUI.SetActive(false);

        // ===== SINGLETON MAIS SEGURO =====
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[PlayerStats] JÁ EXISTE Instance! Destruindo {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // ===== REMOVIDO: DontDestroyOnLoad(gameObject); =====
        // O PinkMonster (pai) já usa DontDestroyOnLoad, então este persiste automaticamente!

        Debug.Log($"[PlayerStats] Instance criada: {gameObject.name}");

        _xpBar = FindObjectOfType<XpBar>();

        // Camera setup
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera != null)
            originalCameraPosition = mainCamera.transform.position;

        // Health
        if (!PlayerPrefs.HasKey("PlayerHealth"))
            PlayerPrefs.SetFloat("PlayerHealth", 100f);
        maxHealth = PlayerPrefs.GetFloat("PlayerHealth");

        // Defense
        if (!PlayerPrefs.HasKey("PlayerDefense"))
            PlayerPrefs.SetFloat("PlayerDefense", 5f);
        defense = PlayerPrefs.GetFloat("PlayerDefense");

        // Money Multiplier
        if (!PlayerPrefs.HasKey("PlayerMoneyMultiplier"))
            PlayerPrefs.SetFloat("PlayerMoneyMultiplier", 1f);
        moneyMultiplier = PlayerPrefs.GetFloat("PlayerMoneyMultiplier");

        if(!PlayerPrefs.HasKey("PlayerAttack"))
            PlayerPrefs.SetFloat("PlayerAttack", 20f);
        attack = PlayerPrefs.GetFloat("PlayerAttack");

        if (PlayerPrefs.HasKey("PlayerMoney"))
            money = PlayerPrefs.GetInt("PlayerMoney", 0);

        PlayerPrefs.Save();
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    // === VIDA ===
    public void TakeDamage(float damage)
    {
        float realDamage = damage - (defense * damage / 100); // reduz dano pela defesa
        currentHealth -= realDamage;
        UpdateUI();
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateUI();
    }

    public void SetMaxHealth(float amount)
    {
        maxHealth = amount;
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void SetDefense(float amount)
    {
        defense = amount;
    }

    public void SetMoneyMultiplier(float amount)
    {
        moneyMultiplier = amount;
    }

    // === ATAQUE / DEFESA ===
    public float GetAttack()
    {
        return attack;
    }

    public float GetDefense()
    {
        return defense;
    }

    public float GetMoneyMultiplier()
    {
        return moneyMultiplier;
    }

    public float GetHealth()
    {
        return maxHealth;
    }

    public float GetMoney()
    {
        return money;
    }

    public void IncreaseHealth(float amount)
    {
        maxHealth += amount;
        currentHealth = maxHealth;
    }

    public void IncreaseAttack(float amount)
    {
        attack += amount;
    }

    public void IncreaseDefense(float amount)
    {
        defense += amount;
    }

    public void IncreaseMoneyMultiplier(float amount)
    {
        moneyMultiplier += amount;
        Debug.Log($"Novo multiplicador de dinheiro: {moneyMultiplier}");
    }

    public void AddMoneyFromKill(int baseAmount)
    {
        int total = Mathf.RoundToInt(baseAmount * moneyMultiplier);
        AddMoney(total);
    }

    // === XP / LEVEL ===
    public void GainXP(float amount)
    {
        currentXP += amount;
        _xpBar.increaseXP(amount);

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }

        UpdateUI();
    }

    private void LevelUp()
    {
        level++;
        xpToNextLevel *= 1.25f; // aumenta XP necessária a cada nível
        _xpBar.levelUp(xpToNextLevel);
        maxHealth += 10f;
        attack += 2f;
        defense += 1f;
    
        // ===== CURA 5% AO SUBIR DE NÍVEL =====
        float healAmount = maxHealth * 0.05f; // 5% da vida máxima
        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        UpdateUI();
        // =====================================
    
        Time.timeScale = 0f;
        Physics2D.simulationMode = SimulationMode2D.Script;
        CardUI.SetActive(true);
        itemRandom1 = Card1.GetComponent<ItemRandomScript1>();
        itemRandom2 = Card2.GetComponent<ItemRandomScript2>();
        itemRandom3 = Card3.GetComponent<ItemRandomScript3>();
        itemRandom1.DrawRandomItem();
        itemRandom2.DrawRandomItem();
        itemRandom3.DrawRandomItem();

        Debug.Log($"Subiu para o nível {level}! +{healAmount:0.0} HP restaurado!");
    }

    // === SISTEMA DE CÂMERA E DESATIVAÇÃO DE OBJETOS ===
    public void StartCameraTransition()
    {
        if (level < 11)
            StartCoroutine(CameraTransitionSequence());
        else
            UnpauseGame();
    }

    private IEnumerator CameraTransitionSequence()
    {
        if (isCameraTransitioning)
        {
            UnpauseGame();
            yield break;
        }

        // Verifica e pega a câmera novamente se necessário
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("Câmera principal não encontrada!");
                UnpauseGame();
                yield break;
            }
        }

        isCameraTransitioning = true;

        // DESATIVA O CINEMACHINE TEMPORARIAMENTE
        bool cinemachineWasActive = false;
        if (cinemachineVirtualCamera != null)
        {
            cinemachineWasActive = cinemachineVirtualCamera.activeSelf;
            cinemachineVirtualCamera.SetActive(false);
            Debug.Log("Cinemachine desativado");
        }

        // Salva posição original da câmera
        originalCameraPosition = mainCamera.transform.position;

        Debug.Log($"Iniciando transição da câmera de {originalCameraPosition} para {targetCameraPosition}");

        // Move a câmera para a posição alvo
        float elapsed = 0f;
        Vector3 startPos = originalCameraPosition;

        while (elapsed < cameraTransitionDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Usa unscaled para funcionar com Time.timeScale = 0
            float t = elapsed / cameraTransitionDuration;

            // Suavização com ease in-out
            t = t * t * (3f - 2f * t);

            mainCamera.transform.position = Vector3.Lerp(startPos, targetCameraPosition, t);
            yield return null;
        }

        mainCamera.transform.position = targetCameraPosition;
        Debug.Log($"Câmera chegou em {mainCamera.transform.position}");

        // Desativa objetos aleatórios
        DisableRandomObjects();

        // Pequena pausa para mostrar o resultado
        yield return new WaitForSecondsRealtime(1.5f);

        // Volta a câmera para a posição original
        Debug.Log($"Voltando câmera para {originalCameraPosition}");
        elapsed = 0f;
        startPos = mainCamera.transform.position;

        while (elapsed < cameraTransitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / cameraTransitionDuration;
            t = t * t * (3f - 2f * t);

            mainCamera.transform.position = Vector3.Lerp(startPos, originalCameraPosition, t);
            yield return null;
        }

        mainCamera.transform.position = originalCameraPosition;
        Debug.Log($"Câmera voltou para {mainCamera.transform.position}");

        // REATIVA O CINEMACHINE
        if (cinemachineVirtualCamera != null && cinemachineWasActive)
        {
            cinemachineVirtualCamera.SetActive(true);
            Debug.Log("Cinemachine reativado");
        }

        // AGORA SIM despausa o jogo (só depois de tudo)
        UnpauseGame();

        isCameraTransitioning = false;
    }

    private void DisableRandomObjects()
    {
        if (objectsParent == null)
        {
            Debug.LogWarning("objectsParent não está configurado!");
            return;
        }

        // Pega todos os filhos ativos
        System.Collections.Generic.List<GameObject> activeChildren = new System.Collections.Generic.List<GameObject>();

        foreach (Transform child in objectsParent.transform)
        {
            if (child.gameObject.activeSelf)
            {
                activeChildren.Add(child.gameObject);
            }
        }

        if (activeChildren.Count == 0)
        {
            Debug.LogWarning("Nenhum objeto filho ativo encontrado!");
            return;
        }

        // Desativa objetos aleatórios
        int toDisable = Mathf.Min(objectsToDisable, activeChildren.Count);

        for (int i = 0; i < toDisable; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, activeChildren.Count);
            activeChildren[randomIndex].SetActive(false);
            activeChildren.RemoveAt(randomIndex);
            AudioSource.PlayClipAtPoint(bossClip, transform.position);
            if (level == 10)
            {
                AudioSource.PlayClipAtPoint(battleClip, transform.position);
            }
        }

        Debug.Log($"{toDisable} objetos foram desativados!");
    }

    private void UnpauseGame()
    {
        Time.timeScale = 1f;
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
    }

    public void AddMoney(int amount)
    {
        money += amount;
        SaveMoney(); // ← SALVA!
        UpdateUI();
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            SaveMoney(); // ← SALVA!
            UpdateUI();
            return true;
        }
        return false;
    }

    // ===== NOVO: Salvar dinheiro =====
    private void SaveMoney()
    {
        PlayerPrefs.SetInt("PlayerMoney", money);
        PlayerPrefs.Save();
    }

    // === UI ===
    private void UpdateUI()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (levelText != null)
            levelText.text = $"Lv. {level}";

        if (xpText != null)
            xpText.text = $"{currentXP:0}/{xpToNextLevel:0} XP";

        if (moneyText != null)
            moneyText.text = $"$ {money}";
    }

    // getters simples para outros scripts
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float defenseValue => defense;
    public int Level => level;
    public int Money => money;
    public float xpToNextLevelValue => xpToNextLevel;
    public float CurrentXP => currentXP;
}