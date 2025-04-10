using UnityEngine;

public class CameraFollow : MonoBehaviour // Этот скрипт нужно повесить на Main Camera
{
    public Transform target; // Сюда перетащишь объект игрока (BouncingBall)

    // Насколько плавно камера будет следовать за целью.
    // Меньшие значения = более плавное/медленное следование.
    public float smoothSpeed = 0.125f;

    // Смещение камеры относительно цели.
    // Это расстояние и угол будут автоматически вычислены при старте.
    public Vector3 offset;

    // --- Опционально: Ограничения движения камеры ---
    // public bool limitBounds = false;
    // public Vector3 minCameraPos;
    // public Vector3 maxCameraPos;
    // ---------------------------------------------

    void Start()
    {
        // Проверяем, назначена ли цель
        if (target == null)
        {
            Debug.LogError("Цель (target) для камеры не назначена!");
            enabled = false; // Отключаем скрипт, если цели нет
            return;
        }

        // Вычисляем начальное смещение камеры относительно цели.
        // Важно: Расположи камеру в редакторе так, как ты хочешь видеть игру
        // перед запуском, это смещение будет сохранено.
        offset = transform.position - target.position;
    }

    // LateUpdate вызывается после всех Update и FixedUpdate.
    // Это идеальное место для обновления камеры, так как игрок
    // уже должен был завершить свое движение в этом кадре.
    void LateUpdate()
    {
        // Если цель пропала (например, уничтожена), не выполняем код
        if (target == null) return;

        // Рассчитываем желаемую позицию камеры: позиция цели + смещение
        Vector3 desiredPosition = target.position + offset;

        // --- Опционально: Применение ограничений ---
        // if (limitBounds)
        // {
        //     desiredPosition.x = Mathf.Clamp(desiredPosition.x, minCameraPos.x, maxCameraPos.x);
        //     desiredPosition.y = Mathf.Clamp(desiredPosition.y, minCameraPos.y, maxCameraPos.y);
        //     desiredPosition.z = Mathf.Clamp(desiredPosition.z, minCameraPos.z, maxCameraPos.z);
        // }
        // -----------------------------------------

        // Плавно перемещаем камеру из текущей позиции в желаемую
        // Vector3.Lerp - Линейная интерполяция между двумя точками
        // Третий параметр (t) определяет, насколько близко к B мы переместимся от A.
        // Использование smoothSpeed * Time.deltaTime обеспечивает плавность,
        // не зависящую (или почти не зависящую) от частоты кадров.
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Применяем вычисленную позицию к камере
        transform.position = smoothedPosition;

        // Опционально: Всегда смотреть на цель (раскомментируй, если нужно)
        // Это может быть полезно, если цель вращается, но камера должна всегда
        // смотреть в её центр, игнорируя сохранение изначального угла обзора.
        // Для простого шара это обычно не нужно, если offset рассчитан правильно.
        // transform.LookAt(target);
    }
}
