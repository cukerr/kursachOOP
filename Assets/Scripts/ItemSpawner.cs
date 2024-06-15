using System.Collections;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] pickupPrefabs; // Массив префабов предметов
    public float spawnInterval = 5f; // Интервал спавна предметов
    public Vector2 spawnAreaMin; // Минимальные координаты области спавна
    public Vector2 spawnAreaMax; // Максимальные координаты области спавна

    void Start()
    {
        StartCoroutine(SpawnItems());
    }

    IEnumerator SpawnItems()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnItem();
        }
    }

    void SpawnItem()
    {
        // Случайный выбор префаба предмета
        GameObject pickupPrefab = pickupPrefabs[Random.Range(0, pickupPrefabs.Length)];

        // Случайное определение позиции спавна внутри заданной области
        Vector2 spawnPosition = new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        // Создание предмета в указанной позиции
        Instantiate(pickupPrefab, spawnPosition, Quaternion.identity);
    }
}
