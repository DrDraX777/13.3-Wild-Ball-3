using UnityEngine;
using UnityEngine.SceneManagement; // ���������, �.�. ������������ � Die() � LoadNextLevel()
using System.Collections;

public class PlayerInteractionHandler : MonoBehaviour
{
    [Header("�������� ���������")]
    private Vector3 startPosition;
    private Rigidbody playerRigidbody;
    private Renderer playerRenderer; // ���� ����� �������� �������� ������

    [Header("��������� ��������")]
    public int flashCount = 4; // ���������� ������� ��� ��������
    public float flashDuration = 0.1f; // ������������ ������� ����� ������� (�����/�����)
    private bool isRespawning = false; // ����, ����� ������������� ��������� ������� �� ����� ��������

    [Header("�������")]
    [Tooltip("������ ������� ������ ��� ������ (�����)")]
    public GameObject deathEffectPrefab; // ���� ����� ���������� ������ PlayerDeathEffect

    void Awake()
    {
        startPosition = transform.position; // ���������� ��������� �������
        playerRigidbody = GetComponent<Rigidbody>();
        // �������� ����� ��������, ���� ���� �� � �������� �������
        playerRenderer = GetComponent<Renderer>();
        if (playerRenderer == null)
        {
            playerRenderer = GetComponentInChildren<Renderer>();
        }

        // �������� �� ������� �����������
        if (playerRigidbody == null) { Debug.LogError("Rigidbody �� ������ �� ������!", this); }
        if (playerRenderer == null) { Debug.LogError("Renderer �� ������ �� ������ ��� ��� �������� ��������!", this); }
        if (deathEffectPrefab == null) { Debug.LogError("������ 'Death Effect Prefab' �� �������� � ����������!", this); }
    }

    // ���� ����� ������� ��� ��������� (��� ������������ ��-���������)
    void OnCollisionEnter(Collision collision)
    {
        

        // --- ���� ���������������� ������ ---
        // ������ ��� RespawnHazard (������� �� ������)
        if (!isRespawning && collision.gameObject.CompareTag("RespawnHazard"))
        {
            // ��������� ������������������ � ��������� � ���������
            StartRespawnWithDeathEffect();
        }
        // ------------------------------------
    }

    

    // --- ����� ����� ��� ������� �������� � �������� ---
    void StartRespawnWithDeathEffect()
    {
        if (isRespawning) return; // ���� ��� �����������, ������ �� ������

        Debug.Log("����� �������� RespawnHazard! ������ ��������...");
        isRespawning = true; // ������ ����, ��� ������� �������

        // 1. ������� ������ ������ ������ (�����)
        //    � ������� ������� ������, ����� ��� ��� ��� �����������
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            // ������ ��� ���� ��������� ��������� Stop Action = Destroy
        }
        else
        {
            Debug.LogWarning("������ ������� ������ �� ��������!");
        }

        // 2. ���������� ������������� �������� ������, ����� �� �� ������
        //    ����� �������� ������� ������
        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
            playerRigidbody.isKinematic = true; // ������ �������������� �� ����� ��������
        }

        // 3. ��������� �������� ��� ����������� � �������

        StartCoroutine(RespawnCoroutine());
    }
    // -----------------------------------------------

    IEnumerator RespawnCoroutine()
    {
        // ����� ��� ���������� � ������� (�.�. isKinematic=true � ������ ������)

        // 1. ����������� ������ � ��������� �����
        // �� ������ ��� �����, ����� �������� ������ � ���������
        transform.position = startPosition;
        // �����������: �������� ��������
        // transform.rotation = Quaternion.identity;

        // ��������� ����� ����� ��������, ����� ����� ����� ������������
        yield return null; // ���� ���� ����

        // 2. ������ ������� (������� ���������)
        if (playerRenderer != null)
        {
            for (int i = 0; i < flashCount; i++)
            {
                playerRenderer.enabled = false;
                yield return new WaitForSeconds(flashDuration);
                playerRenderer.enabled = true;
                yield return new WaitForSeconds(flashDuration);
            }
            playerRenderer.enabled = true; // ��������, ��� ����� ����� � �����
        }
        else
        {
            // ���� �������� �� ������, ������ ���� ������������� �����
            Debug.LogWarning("Renderer �� ������, ������� ����������.");
            yield return new WaitForSeconds(flashCount * flashDuration * 2);
        }

        // 3. �������������� ������ � ����� �����
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = false; // ���������� ������� ���������� ���������
        }
        isRespawning = false; // ������� ����, ��������� ����� ������������
        Debug.Log("������� ��������.");
    }

    // ������ Die() � LoadNextLevel() �������� ��� ���������
   
}
