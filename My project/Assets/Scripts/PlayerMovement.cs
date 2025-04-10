using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;

    private Rigidbody rb;
    private PlayerInput playerInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        if (rb == null) { Debug.LogError("Rigidbody не найден!"); enabled = false; }
        if (playerInput == null) { Debug.LogError("PlayerInput не найден!"); enabled = false; }
    }

    void FixedUpdate()
    {
        float horizontal = playerInput.HorizontalInput;
        float vertical = playerInput.VerticalInput;

        Vector3 movement = new Vector3(horizontal, 0.0f, vertical);
        rb.AddForce(movement * moveSpeed, ForceMode.Acceleration);
    }

    // ... Другие методы, СТРОГО связанные с движением (например, прыжок, если добавишь) ...
    // Например:
    // public void ApplyJumpForce(float force)
    // {
    //     rb.AddForce(Vector3.up * force, ForceMode.Impulse);
    // }
}
