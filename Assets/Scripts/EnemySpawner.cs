using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // ������ �����
    public Transform[] spawnPoints; // ����� ������ ������
    public float spawnInterval = 5f; // �������� ������ ������
    private int enemyCount = 0; // ���������� ������
    private int baseDamage = 6; // ������� ���� �����
    private int baseHealth = 30; // ������� �������� �����

    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnEnemy();
            IncreaseDifficulty();
        }
    }

    void SpawnEnemy()
    {
        // �������� ��������� ����� ������
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // ������� �����
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // ����������� �������������� �����
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.maxHealth = baseHealth + enemyCount * 5; // ����������� �������� �����
        enemyScript.damageToPlayer = baseDamage + enemyCount; // ����������� ���� �����

        enemyCount++; // ����������� ���������� ������
    }

    void IncreaseDifficulty()
    {
        spawnInterval = Mathf.Max(1f, spawnInterval - 0.1f); // ��������� �������� ������ ������, �� �� ������ 1 �������
    }
}
