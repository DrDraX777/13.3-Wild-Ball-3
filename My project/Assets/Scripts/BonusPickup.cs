using UnityEngine;

public class BonusPickup : MonoBehaviour
{
    [Tooltip("Ссылка на дочерний объект с системой частиц")]
    public ParticleSystem pickupEffectPrefab; // Сюда перетащим наш BonusPickupEffect

    [Tooltip("Тег объекта, который может подобрать бонус (игрок)")]
    public string playerTag = "Player";

    // Флаг, чтобы бонус подбирался только один раз
    private bool isPickedUp = false;

    void Start()
    {
        // Небольшая проверка при старте
        if (pickupEffectPrefab == null)
        {
            Debug.LogError("Система частиц (pickupEffectPrefab) не назначена в скрипте BonusPickup на объекте " + gameObject.name);
        }
        // Убедимся, что система частиц выключена изначально
        // (хотя мы это сделали в редакторе, дублирующая проверка не помешает)
        else if (pickupEffectPrefab.gameObject.activeSelf)
        {
            pickupEffectPrefab.gameObject.SetActive(false);
        }
    }

    // Вызывается, когда другой Collider входит в наш триггер
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, не подобран ли бонус уже И вошел ли объект с нужным тегом
        if (!isPickedUp && other.CompareTag(playerTag))
        {
            Pickup();
        }
    }

    private void Pickup()
    {
        Debug.Log("Бонус " + gameObject.name + " подобран!");
        isPickedUp = true;

        // 1. Показываем и запускаем эффект частиц
        if (pickupEffectPrefab != null)
        {
            // Важно: Перед активацией отсоединяем систему частиц от бонуса,
            // чтобы она не уничтожилась вместе с бонусом (если мы будем его удалять)
            // и чтобы stopAction = Destroy сработал на самой системе частиц.
            pickupEffectPrefab.transform.SetParent(null); // Отсоединяем

            pickupEffectPrefab.gameObject.SetActive(true); // Активируем GameObject системы частиц
            pickupEffectPrefab.Play(); // Явно запускаем воспроизведение (хотя SetActive(true) часто тоже запускает)
            // Теперь система частиц сама себя уничтожит после завершения благодаря Stop Action = Destroy
        }

        // 2. Скрываем или уничтожаем сам объект бонуса
        // Можно просто скрыть MeshRenderer и отключить Collider:
        //GetComponent<MeshRenderer>().enabled = false;
        //GetComponent<Collider>().enabled = false;
        // Или можно уничтожить весь объект бонуса:
        Destroy(gameObject);
        // (Если выберешь Destroy, убедись, что частицы отсоединены, как сделано выше!)
    }
}
