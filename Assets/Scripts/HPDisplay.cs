using UnityEngine;
using UnityEngine.UI;

public class HPDisplay : MonoBehaviour
{
    public Text hpText; // ������ �� ��������� ������� UI
    public Transform playerTransform; // ������ �� ��������� ���������
    public PlayerScript playerScript; // ������ �� ������ PlayerScript
    public Canvas canvas; // ������ �� Canvas
    public Vector3 offset; // �������� ������ ������������ ���������

    void Start()
    {
        // ������������� ��������� ������� ������ ��� �������
        UpdateTextPosition();
    }

    void Update()
    {
        if (playerScript != null && hpText != null && playerTransform != null)
        {
            // ��������� ����� ��������
            hpText.text = "HP: " + playerScript.GetCurrentHealth().ToString();

            // ��������� ������� ������
            UpdateTextPosition();
        }
    }

    void UpdateTextPosition()
    {
        // �������� ������� ������ � ������� ����������� � ������ ��������
        Vector3 worldPosition = playerTransform.position + offset;

        // ����������� ������� ������� � �������� ����������
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        // ����������� �������� ���������� � ��������� ���������� Canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosition, canvas.worldCamera, out Vector2 localPoint);

        // ������������� ��������� ������� ������
        hpText.rectTransform.localPosition = localPoint;
    }
}
