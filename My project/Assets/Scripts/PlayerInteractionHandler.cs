using UnityEngine;
using UnityEngine.SceneManagement; // Оставляем, т.к. используется в Die() и LoadNextLevel()
using System.Collections;

public class PlayerInteractionHandler : MonoBehaviour
{
    [Header("Основные Настройки")]
    private Vector3 startPosition;
    private Rigidbody playerRigidbody;
    private Renderer playerRenderer; // Сюда будет назначен рендерер игрока

    [Header("Настройки Респауна")]
    public int flashCount = 4; // Количество вспышек при респауне
    public float flashDuration = 0.1f; // Длительность каждого этапа вспышки (скрыт/видим)
    private bool isRespawning = false; // Флаг, чтобы предотвратить повторный респаун во время респауна

    [Header("Эффекты")]
    [Tooltip("Префаб эффекта частиц при смерти (кровь)")]
    public GameObject deathEffectPrefab; // СЮДА НУЖНО ПЕРЕТАЩИТЬ ПРЕФАБ PlayerDeathEffect

    void Awake()
    {
        startPosition = transform.position; // Запоминаем стартовую позицию
        playerRigidbody = GetComponent<Rigidbody>();
        // Пытаемся найти рендерер, даже если он в дочернем объекте
        playerRenderer = GetComponent<Renderer>();
        if (playerRenderer == null)
        {
            playerRenderer = GetComponentInChildren<Renderer>();
        }

        // Проверки на наличие компонентов
        if (playerRigidbody == null) { Debug.LogError("Rigidbody не найден на игроке!", this); }
        if (playerRenderer == null) { Debug.LogError("Renderer не найден на игроке или его дочерних объектах!", this); }
        if (deathEffectPrefab == null) { Debug.LogError("Префаб 'Death Effect Prefab' не назначен в инспекторе!", this); }
    }

    // Этот метод оставим без изменений (для столкновений не-триггеров)
    void OnCollisionEnter(Collision collision)
    {
        

        // --- НАША МОДИФИЦИРОВАННАЯ ЛОГИКА ---
        // Логика для RespawnHazard (респаун на старте)
        if (!isRespawning && collision.gameObject.CompareTag("RespawnHazard"))
        {
            // Запускаем последовательность с частицами и респауном
            StartRespawnWithDeathEffect();
        }
        // ------------------------------------
    }

    

    // --- НОВЫЙ МЕТОД ДЛЯ ЗАПУСКА РЕСПАУНА С ЭФФЕКТОМ ---
    void StartRespawnWithDeathEffect()
    {
        if (isRespawning) return; // Если уже респаунимся, ничего не делаем

        Debug.Log("Игрок коснулся RespawnHazard! Запуск респауна...");
        isRespawning = true; // Ставим флаг, что процесс начался

        // 1. СОЗДАЕМ ЭФФЕКТ ЧАСТИЦ СМЕРТИ (КРОВЬ)
        //    в текущей позиции игрока, ПЕРЕД тем как его переместить
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            // Префаб сам себя уничтожит благодаря Stop Action = Destroy
        }
        else
        {
            Debug.LogWarning("Префаб эффекта смерти не назначен!");
        }

        // 2. НЕМЕДЛЕННО останавливаем движение игрока, чтобы он не улетел
        //    ПОСЛЕ создания эффекта частиц
        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
            playerRigidbody.isKinematic = true; // Делаем кинематическим на время респауна
        }

        // 3. ЗАПУСКАЕМ КОРУТИНУ для перемещения и мигания

        StartCoroutine(RespawnCoroutine());
    }
    // -----------------------------------------------

    IEnumerator RespawnCoroutine()
    {
        // Игрок уже остановлен и невидим (т.к. isKinematic=true и эффект создан)

        // 1. ПЕРЕМЕЩЕНИЕ ИГРОКА В СТАРТОВУЮ ТОЧКУ
        // Мы делаем это здесь, после создания частиц и остановки
        transform.position = startPosition;
        // Опционально: сбросить вращение
        // transform.rotation = Quaternion.identity;

        // Небольшая пауза перед миганием, чтобы игрок точно переместился
        yield return null; // Ждем один кадр

        // 2. ЭФФЕКТ ВСПЫШКИ (МИГАНИЕ РЕНДЕРЕРА)
        if (playerRenderer != null)
        {
            for (int i = 0; i < flashCount; i++)
            {
                playerRenderer.enabled = false;
                yield return new WaitForSeconds(flashDuration);
                playerRenderer.enabled = true;
                yield return new WaitForSeconds(flashDuration);
            }
            playerRenderer.enabled = true; // Убедимся, что игрок видим в конце
        }
        else
        {
            // Если рендерер не найден, просто ждем эквивалентное время
            Debug.LogWarning("Renderer не найден, мигание невозможно.");
            yield return new WaitForSeconds(flashCount * flashDuration * 2);
        }

        // 3. ВОССТАНОВЛЕНИЕ ФИЗИКИ И СБРОС ФЛАГА
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = false; // Возвращаем обычное физическое поведение
        }
        isRespawning = false; // Снимаем флаг, разрешаем новые столкновения
        Debug.Log("Респаун завершен.");
    }

    // Методы Die() и LoadNextLevel() остаются без изменений
   
}
