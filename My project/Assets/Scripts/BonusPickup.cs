using UnityEngine;

public class BonusPickup : MonoBehaviour
{
    [Tooltip("Префаб эффекта, который будет создан при подборе")]
    public GameObject pickupEffectPrefab;

    [Tooltip("Тег объекта, который может подобрать бонус (игрок)")]
    public string playerTag = "Player";

    private bool isPickedUp = false;

    void Start()
    {
        if (pickupEffectPrefab == null)
        {
            Debug.LogError("Префаб эффекта (pickupEffectPrefab) не назначен!", this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPickedUp && other.CompareTag(playerTag))
        {
            // Используем позицию самого бонуса, а не игрока, для эффекта
            Pickup(transform.position);
        }
    }

    // Принимаем позицию, где нужно создать эффект
    private void Pickup(Vector3 pickupPosition)
    {
        Debug.Log("Бонус " + gameObject.name + " подобран! Уничтожаем родителя...");
        isPickedUp = true;

        // 1. Создаем эффект ИЗ Префаба в месте бонуса
        if (pickupEffectPrefab != null)
        {
            Instantiate(pickupEffectPrefab, pickupPosition, Quaternion.identity);
            // Убедись, что у префаба есть ParticleSystem с PlayOnAwake=true и StopAction=Destroy
        }

        // 2. Уничтожаем РОДИТЕЛЬСКИЙ объект бонуса
        if (transform.parent != null) // Проверяем, есть ли родитель
        {
            Destroy(transform.parent.gameObject); // Уничтожаем GameObject родителя
            // Важно: При уничтожении родителя, все его дочерние объекты (включая этот)
            // также будут уничтожены автоматически.
        }
        else
        {
            // Если родителя нет, уничтожаем сам бонусный объект
            Debug.LogWarning("У объекта " + gameObject.name + " нет родителя. Уничтожаем сам объект.", this);
            Destroy(gameObject);
        }
    }
}
