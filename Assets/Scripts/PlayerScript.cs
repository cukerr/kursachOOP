using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float runSpeed = 10f; // Скорость бега
    public float squatSpeed = 2f;
    public float jumpForce = 5f; // Сила прыжка
    public float slideSpeed = 2f; // Скорость скольжения по стене

    public int maxHealth = 100;
    private int currentHealth;

    private Rigidbody2D rb;
    public Animator animator;
    private bool isGrounded; // Проверка, на земле ли персонаж
    private bool isSliding; // Проверка, на стене ли персонаж
    private bool isDead = false; // Проверка, жив ли персонаж

    private int jumpCount = 0; // Счетчик прыжков
    private bool canDoubleJump = true; // Возможность выполнить двойной прыжок

    private Coroutine healCoroutine; // Ссылка на корутину для автолевинга

    // Переменные для системы уровней и опыта
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;

    // Ссылки на UI элементы
    public Text levelText;
    public Text xpText;
    public GameObject deathPanel; // Панель окна смерти

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        LoadPlayerData(); // Загружаем данные игрока при старте
        currentHealth = maxHealth;

        // Обновляем UI
        UpdateUI();

        // Убедитесь, что панель смерти отключена при старте
        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    void Update()
    {
        if (isDead)
        {
            HandleDeathScreenInput();
            return; // Прекращение выполнения, если персонаж мертв
        }

        float moveInput = Input.GetAxis("Horizontal");
        animator.SetFloat("horizontalMove", Mathf.Abs(moveInput)); // Обновить анимацию движения

        // Проверка нажатия клавиши Shift для бега и движение
        bool running = Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(moveInput) > 0.01f;
        float currentMoveSpeed = running ? runSpeed : moveSpeed;
        rb.velocity = new Vector2(moveInput * currentMoveSpeed, rb.velocity.y);

        // Устанавливаем параметр анимации для бега
        animator.SetBool("running", running);

        // Прыжок по пробелу
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        // Проверка скольжения по стене
        if (isSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);
        }

        if (moveInput > 0) // Если двигаемся вправо, поворачиваем персонажа вправо
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput < 0) // Если двигаемся влево, поворачиваем персонажа влево
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // Атака по левой кнопке мыши
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        // Атака по правой кнопке мыши
        if (Input.GetMouseButtonDown(1))
        {
            SpecialAttack();
        }

        // Проверка на падение ниже определенной высоты
        if (transform.position.y < -10 && !isDead)
        {
            TakeDamage(maxHealth); // Наносим урон равный максимальному здоровью
        }
    }

    void Jump()
    {
        if (isGrounded || (jumpCount < 2 && canDoubleJump))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
            animator.SetBool("jumping", true);

            if (jumpCount == 2)
            {
                canDoubleJump = false;
            }
        }
    }

    void HandleDeathScreenInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            ReturnToMenu();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            QuitGame();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("jumping", false);
            jumpCount = 0; // Сбрасываем счетчик прыжков при касании земли
            canDoubleJump = true; // Восстанавливаем возможность двойного прыжка
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            isSliding = true;
            animator.SetBool("sliding", true);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            isSliding = false;
            animator.SetBool("sliding", false);
        }
    }

    // Функция для обычной атаки
    void Attack()
    {
        animator.SetBool("lkmAttacking", true);
        // Логика для атаки (напр., обнаружение врагов и нанесение урона)
        DealDamage(4); // Наносим 4 урона
        StartCoroutine(ResetAttack("lkmAttacking"));
    }

    // Функция для специальной атаки
    void SpecialAttack()
    {
        animator.SetBool("pkmAttacking", true);
        // Логика для специальной атаки (напр., обнаружение врагов и нанесение большого урона)
        DealDamage(11); // Наносим 11 урона
        StartCoroutine(ResetAttack("pkmAttacking"));
    }

    // Функция для нанесения урона врагам
    void DealDamage(int damage)
    {
        // Здесь предполагается, что враги находятся в пределах определенного радиуса вокруг игрока
        float attackRange = 1.0f; // Радиус атаки
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<Enemy>().TakeDamage(damage);
            }
        }
    }

    // Функция для получения урона
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log("Player took damage: " + damage + ", current health: " + currentHealth);

        if (healCoroutine != null)
        {
            StopCoroutine(healCoroutine); // Останавливаем текущую корутину лечения, если она существует
        }
        healCoroutine = StartCoroutine(AutoHeal()); // Запускаем новую корутину лечения

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Корутина для автолевинга
    IEnumerator AutoHeal()
    {
        yield return new WaitForSeconds(2f); // Ждем 2 секунды

        while (currentHealth < maxHealth && !isDead)
        {
            currentHealth += 1; // Восстанавливаем 1 HP
            Debug.Log("Player healed: " + currentHealth);
            yield return new WaitForSeconds(0.1f); // Восстанавливаем здоровье каждые 0.1 секунды
        }
    }

    // Функция для смерти персонажа
    void Die()
    {
        isDead = true;
        animator.SetBool("isDead", true);
        Debug.Log("Player is dead, playing death animation");
        rb.velocity = Vector2.zero; // Останавливаем персонажа
        rb.constraints = RigidbodyConstraints2D.FreezeAll; // Замораживаем физику персонажа
        Time.timeScale = 0f; // Останавливаем время
        // Логика для смерти персонажа (напр., отключение управления)
        ShowDeathPanel(); // Показ окна смерти
    }

    // Coroutine для сброса атакующих флагов
    IEnumerator ResetAttack(string attackType)
    {
        yield return new WaitForSeconds(0.5f); // Задержка для завершения анимации атаки
        animator.SetBool(attackType, false);
    }

    // Публичный метод для получения текущего здоровья
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    // Применение бонусов от предметов
    public void ApplyPickup(PickupItem.PickupType type, float duration)
    {
        switch (type)
        {
            case PickupItem.PickupType.Speed:
                StartCoroutine(TemporarySpeedBoost(duration));
                break;
            case PickupItem.PickupType.Health:
                StartCoroutine(TemporaryHealthBoost(duration));
                break;
            case PickupItem.PickupType.Damage:
                StartCoroutine(TemporaryDamageBoost(duration));
                break;
        }
    }

    // Корутина для временного ускорения
    IEnumerator TemporarySpeedBoost(float duration)
    {
        moveSpeed *= 1.5f;
        runSpeed *= 1.5f;
        yield return new WaitForSeconds(duration);
        moveSpeed /= 1.5f;
        runSpeed /= 1.5f;
    }

    // Корутина для временного увеличения здоровья
    IEnumerator TemporaryHealthBoost(float duration)
    {
        int originalHealth = currentHealth;
        currentHealth = Mathf.Min(maxHealth, currentHealth + 50);
        yield return new WaitForSeconds(duration);
        currentHealth = originalHealth;
    }

    // Корутина для временного увеличения урона
    IEnumerator TemporaryDamageBoost(float duration)
    {
        int originalDamage = 4; // базовый урон для обычной атаки
        int originalSpecialDamage = 11; // базовый урон для специальной атаки
        int boostedDamage = originalDamage + 10;
        int boostedSpecialDamage = originalSpecialDamage + 10;

        DealDamage(boostedDamage);
        DealDamage(boostedSpecialDamage);

        yield return new WaitForSeconds(duration);

        DealDamage(originalDamage);
        DealDamage(originalSpecialDamage);
    }

    // Метод для получения опыта
    public void GainXP(int amount)
    {
        currentXP += amount;
        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }

        // Обновляем UI
        UpdateUI();
        SavePlayerData(); // Сохраняем данные игрока после получения опыта
    }

    // Метод для повышения уровня
    void LevelUp()
    {
        currentXP -= xpToNextLevel;
        currentLevel++;
        xpToNextLevel += 100; // Увеличение требований к опыту для следующего уровня
        maxHealth += 10; // Увеличение максимального здоровья при повышении уровня
        currentHealth = maxHealth; // Полное восстановление здоровья
        Debug.Log("Level Up! Current Level: " + currentLevel);

        // Обновляем UI
        UpdateUI();

        SavePlayerData(); // Сохраняем данные игрока
    }

    // Обновление UI элементов
    void UpdateUI()
    {
        levelText.text = "Level: " + currentLevel;
        xpText.text = "XP: " + currentXP + " / " + xpToNextLevel;
    }

    // Сохранение данных игрока
    void SavePlayerData()
    {
        PlayerPrefs.SetInt("PlayerLevel", currentLevel);
        PlayerPrefs.SetInt("PlayerXP", currentXP);
        PlayerPrefs.SetInt("PlayerXPToNextLevel", xpToNextLevel);
        PlayerPrefs.SetInt("PlayerHealth", currentHealth);
        PlayerPrefs.SetInt("PlayerMaxHealth", maxHealth);
        PlayerPrefs.Save(); // Явное сохранение данных
    }

    // Загрузка данных игрока
    void LoadPlayerData()
    {
        currentLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
        currentXP = PlayerPrefs.GetInt("PlayerXP", 0);
        xpToNextLevel = PlayerPrefs.GetInt("PlayerXPToNextLevel", 100);
        currentHealth = PlayerPrefs.GetInt("PlayerHealth", maxHealth);
        maxHealth = PlayerPrefs.GetInt("PlayerMaxHealth", maxHealth);

        // Обновляем UI
        UpdateUI();
    }

    // Метод для отображения окна смерти
    void ShowDeathPanel()
    {
        if (deathPanel != null)
        {
            deathPanel.SetActive(true); // Отображаем окно смерти
        }
    }

    // Метод для перезапуска игры
    public void RestartGame()
    {
        Debug.Log("Restarting game...");
        Time.timeScale = 1f; // Восстанавливаем время
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Перезагрузка текущей сцены
    }

    // Метод для выхода в меню
    public void ReturnToMenu()
    {
        Debug.Log("Returning to menu...");
        Time.timeScale = 1f; // Восстанавливаем время
        SceneManager.LoadScene("MainMenuScene"); // Загрузка сцены меню (укажите правильное имя сцены)
    }

    // Метод для выхода из игры
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit(); // Выход из игры
    }
}
