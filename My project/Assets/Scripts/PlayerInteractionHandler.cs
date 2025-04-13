using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class PlayerInteractionHandler : MonoBehaviour
{
    [Header("Основные Настройки")]
    private Vector3 startPosition;
    private Rigidbody playerRigidbody;
    private Renderer playerRenderer;
    private Collider playerCollider; // <--- ДОБАВЛЕНО: Ссылка на коллайдер игрока

    [Header("Настройки Респауна")]
    public int flashCount = 4;
    public float flashDuration = 0.1f;
    private bool isRespawning = false;

    [Header("Эффекты")]
    public GameObject deathEffectPrefab;

    [Header("Интерфейс")] // <--- ДОБАВЛЕНА СЕКЦИЯ
    [Tooltip("Ссылка на TextMeshPro Text для отображения счета монет")]
    public TextMeshProUGUI coinText; // <--- ДОБАВЛЕНО: Ссылка на UI текст
    private int coinCount = 0; // <--- ДОБАВЛЕНО: Счетчик монет

    void Awake()
    {
        startPosition = transform.position;
        playerRigidbody = GetComponent<Rigidbody>();
        playerRenderer = GetComponent<Renderer>();
        playerCollider = GetComponent<Collider>(); // <--- ДОБАВЛЕНО: Получаем коллайдер

        // Ищем рендерер в дочерних, если нет на основном
        if (playerRenderer == null) playerRenderer = GetComponentInChildren<Renderer>();

        // Проверки
        if (playerRigidbody == null) Debug.LogError("Rigidbody не найден!", this);
        if (playerRenderer == null) Debug.LogError("Renderer не найден!", this);
        if (playerCollider == null) Debug.LogError("Collider не найден!", this); // <--- ДОБАВЛЕНО: Проверка коллайдера
        if (deathEffectPrefab == null) Debug.LogError("Префаб 'Death Effect Prefab' не назначен!", this);
        if (coinText == null)
        {
            Debug.LogError("Ссылка на 'Coin Text' (TextMeshProUGUI) не назначена в инспекторе!", this);
        }
        UpdateCoinUI(); // Обновляем текст при старте (чтобы показать 0)
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isRespawning && collision.gameObject.CompareTag("RespawnHazard"))
        {
            StartRespawnWithDeathEffect();
        }
        // ...
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finish"))
        {
            LoadNextLevel();
        }

        if (other.gameObject.CompareTag("Bonus"))
        {
            AddCoins();
        }




    }

    public void AddCoins()
    {       

        coinCount += 1;
        Debug.Log($"Добавлено 1 монет. Всего: {coinCount}");
        UpdateCoinUI();
    }

   
    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = "Coins: " + coinCount; // Формируем текст
        }
    }

    void LoadNextLevel()
    {
        // ... (код без изменений) ...
        if (isRespawning) return;
        Debug.Log("Уровень пройден! Загрузка следующего...");
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings) SceneManager.LoadScene(nextSceneIndex);
        else { Debug.Log("Все уровни пройдены!"); SceneManager.LoadScene(0); }
    }

    void StartRespawnWithDeathEffect()
    {
        if (isRespawning) return;
        isRespawning = true;
        Debug.Log("Столкновение с RespawnHazard! Отключаем игрока, показываем эффект...");

        // --- ИЗМЕНЕННЫЙ ПОРЯДОК ---
        // 1. ОТКЛЮЧАЕМ Взаимодействие и Видимость ИГРОКА
        if (playerCollider != null)
        {
            playerCollider.enabled = false; // <--- ВАЖНО: Отключаем коллайдер ПЕРЕД эффектом
        }
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = true; // Делаем кинематическим
            playerRigidbody.velocity = Vector3.zero; // Обнуляем скорость на всякий случай
            playerRigidbody.angularVelocity = Vector3.zero;
        }
        if (playerRenderer != null)
        {
            playerRenderer.enabled = false; // Скрываем игрока
        }
        // --- КОНЕЦ ИЗМЕНЕНИЙ В ПОРЯДКЕ ---

        // 2. СОЗДАЕМ ЭФФЕКТ (теперь частицы не столкнутся с отключенным коллайдером игрока)
        ParticleSystem deathEffectInstance = null;
        if (deathEffectPrefab != null)
        {
            GameObject effectGO = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            deathEffectInstance = effectGO.GetComponent<ParticleSystem>();
            if (deathEffectInstance == null) Debug.LogError("Префаб эффекта не содержит ParticleSystem!", effectGO);
        }
        else Debug.LogWarning("Префаб эффекта смерти не назначен!");


        // 3. ЗАПУСКАЕМ ОЖИДАНИЕ завершения эффекта и последующий респаун
        StartCoroutine(WaitForEffectAndStartRespawn(deathEffectInstance));
    }

    // Корутина ожидания (код без изменений)
    IEnumerator WaitForEffectAndStartRespawn(ParticleSystem effectSystem)
    {
        if (effectSystem != null)
        {
            Debug.Log("Начинаем ожидание завершения эффекта...");
            while (effectSystem != null && effectSystem.IsAlive(true))
            {
                yield return null;
            }
            Debug.Log("Эффект смерти завершился или был уничтожен.");
        }
        else Debug.LogWarning("Эффект для ожидания не найден.");

        Debug.Log("Начинаем респаун (перемещение и мигание)...");
        StartCoroutine(RespawnCoroutine());
    }

    // Основная корутина респауна (нужно включить коллайдер обратно!)
    IEnumerator RespawnCoroutine()
    {
        // 1. Перемещение (игрок все еще невидим и коллайдер выключен)
        transform.position = startPosition;
        yield return null;

        // 2. Мигание (делаем игрока видимым)
        if (playerRenderer != null)
        {
            bool finalState = true; // Для чередования
            for (int i = 0; i < flashCount * 2; i++) // Умножаем на 2, т.к. вкл/выкл
            {
                playerRenderer.enabled = finalState;
                yield return new WaitForSeconds(flashDuration);
                finalState = !finalState; // Инвертируем
            }
            playerRenderer.enabled = true; // Убедимся, что включен в конце
        }
        else
        {
            // Если нет рендерера, просто ждем
            yield return new WaitForSeconds(flashCount * flashDuration * 2);
        }

        // 3. Восстановление взаимодействия ПЕРЕД возвратом управления физике
        if (playerCollider != null)
        {
            playerCollider.enabled = true; // <--- ВАЖНО: Включаем коллайдер обратно!
        }
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = false; // Возвращаем физику
        }

        isRespawning = false;
        Debug.Log("Респаун завершен.");
    }
}
