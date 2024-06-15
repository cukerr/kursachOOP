using UnityEngine;
using UnityEngine.UI;

public class HPDisplay : MonoBehaviour
{
    public Text hpText; // Ссылка на текстовый элемент UI
    public Transform playerTransform; // Ссылка на трансформ персонажа
    public PlayerScript playerScript; // Ссылка на скрипт PlayerScript
    public Canvas canvas; // Ссылка на Canvas
    public Vector3 offset; // Смещение текста относительно персонажа

    void Start()
    {
        // Устанавливаем начальную позицию текста под игроком
        UpdateTextPosition();
    }

    void Update()
    {
        if (playerScript != null && hpText != null && playerTransform != null)
        {
            // Обновляем текст здоровья
            hpText.text = "HP: " + playerScript.GetCurrentHealth().ToString();

            // Обновляем позицию текста
            UpdateTextPosition();
        }
    }

    void UpdateTextPosition()
    {
        // Получаем позицию игрока в мировых координатах с учетом смещения
        Vector3 worldPosition = playerTransform.position + offset;

        // Преобразуем мировую позицию в экранные координаты
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        // Преобразуем экранные координаты в локальные координаты Canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosition, canvas.worldCamera, out Vector2 localPoint);

        // Устанавливаем локальную позицию текста
        hpText.rectTransform.localPosition = localPoint;
    }
}
