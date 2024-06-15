using System.Collections;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] pickupPrefabs; // ������ �������� ���������
    public float spawnInterval = 5f; // �������� ������ ���������
    public Vector2 spawnAreaMin; // ����������� ���������� ������� ������
    public Vector2 spawnAreaMax; // ������������ ���������� ������� ������

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
        // ��������� ����� ������� ��������
        GameObject pickupPrefab = pickupPrefabs[Random.Range(0, pickupPrefabs.Length)];

        // ��������� ����������� ������� ������ ������ �������� �������
        Vector2 spawnPosition = new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        // �������� �������� � ��������� �������
        Instantiate(pickupPrefab, spawnPosition, Quaternion.identity);
    }
}
