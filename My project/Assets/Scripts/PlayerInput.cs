using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // Публичные свойства (read-only) для передачи значений ввода другим скриптам
    public float HorizontalInput { get; private set; }
    public float VerticalInput { get; private set; }
    // Можно добавить другие вводы, например, для прыжка, взаимодействия и т.д.
    // public bool JumpInputDown { get; private set; }
    // public bool InteractInputDown { get; private set; }

    void Update()
    {
        // Читаем оси движения
        HorizontalInput = Input.GetAxis("Horizontal"); // A/D или стрелки влево/вправо
        VerticalInput = Input.GetAxis("Vertical");     // W/S или стрелки вверх/вниз

        // Читаем другие кнопки (если нужно для будущих заданий)
        // JumpInputDown = Input.GetButtonDown("Jump"); // Обычно пробел
        // InteractInputDown = Input.GetKeyDown(KeyCode.E);
    }
}
