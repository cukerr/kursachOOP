using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Префаб врага
    public Transform[] spawnPoints; // Точки спавна врагов
    public float spawnInterval = 5f; // Интервал спавна врагов
    private int enemyCount = 0; // Количество врагов
    private int baseDamage = 6; // Базовый урон врага
    private int baseHealth = 30; // Базовое здоровье врага

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
        // Выбираем случайную точку спавна
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Создаем врага
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // Настраиваем характеристики врага
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.maxHealth = baseHealth + enemyCount * 5; // Увеличиваем здоровье врага
        enemyScript.damageToPlayer = baseDamage + enemyCount; // Увеличиваем урон врага

        enemyCount++; // Увеличиваем количество врагов
    }

    void IncreaseDifficulty()
    {
        spawnInterval = Mathf.Max(1f, spawnInterval - 0.1f); // Уменьшаем интервал спавна врагов, но не меньше 1 секунды
    }
}
