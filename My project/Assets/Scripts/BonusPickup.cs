using UnityEngine;

public class BonusPickup : MonoBehaviour
{
    [Tooltip("������ �� �������� ������ � �������� ������")]
    public ParticleSystem pickupEffectPrefab; // ���� ��������� ��� BonusPickupEffect

    [Tooltip("��� �������, ������� ����� ��������� ����� (�����)")]
    public string playerTag = "Player";

    // ����, ����� ����� ���������� ������ ���� ���
    private bool isPickedUp = false;

    void Start()
    {
        // ��������� �������� ��� ������
        if (pickupEffectPrefab == null)
        {
            Debug.LogError("������� ������ (pickupEffectPrefab) �� ��������� � ������� BonusPickup �� ������� " + gameObject.name);
        }
        // ��������, ��� ������� ������ ��������� ����������
        // (���� �� ��� ������� � ���������, ����������� �������� �� ��������)
        else if (pickupEffectPrefab.gameObject.activeSelf)
        {
            pickupEffectPrefab.gameObject.SetActive(false);
        }
    }

    // ����������, ����� ������ Collider ������ � ��� �������
    private void OnTriggerEnter(Collider other)
    {
        // ���������, �� �������� �� ����� ��� � ����� �� ������ � ������ �����
        if (!isPickedUp && other.CompareTag(playerTag))
        {
            Pickup();
        }
    }

    private void Pickup()
    {
        Debug.Log("����� " + gameObject.name + " ��������!");
        isPickedUp = true;

        // 1. ���������� � ��������� ������ ������
        if (pickupEffectPrefab != null)
        {
            // �����: ����� ���������� ����������� ������� ������ �� ������,
            // ����� ��� �� ������������ ������ � ������� (���� �� ����� ��� �������)
            // � ����� stopAction = Destroy �������� �� ����� ������� ������.
            pickupEffectPrefab.transform.SetParent(null); // �����������

            pickupEffectPrefab.gameObject.SetActive(true); // ���������� GameObject ������� ������
            pickupEffectPrefab.Play(); // ���� ��������� ��������������� (���� SetActive(true) ����� ���� ���������)
            // ������ ������� ������ ���� ���� ��������� ����� ���������� ��������� Stop Action = Destroy
        }

        // 2. �������� ��� ���������� ��� ������ ������
        // ����� ������ ������ MeshRenderer � ��������� Collider:
        //GetComponent<MeshRenderer>().enabled = false;
        //GetComponent<Collider>().enabled = false;
        // ��� ����� ���������� ���� ������ ������:
        Destroy(gameObject);
        // (���� �������� Destroy, �������, ��� ������� �����������, ��� ������� ����!)
    }
}
