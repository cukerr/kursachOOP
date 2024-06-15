using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float runSpeed = 10f; // �������� ����
    public float squatSpeed = 2f;
    public float jumpForce = 5f; // ���� ������
    public float slideSpeed = 2f; // �������� ���������� �� �����

    public int maxHealth = 100;
    private int currentHealth;

    private Rigidbody2D rb;
    public Animator animator;
    private bool isGrounded; // ��������, �� ����� �� ��������
    private bool isSliding; // ��������, �� ����� �� ��������
    private bool isDead = false; // ��������, ��� �� ��������

    private int jumpCount = 0; // ������� �������
    private bool canDoubleJump = true; // ����������� ��������� ������� ������

    private Coroutine healCoroutine; // ������ �� �������� ��� �����������

    // ���������� ��� ������� ������� � �����
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;

    // ������ �� UI ��������
    public Text levelText;
    public Text xpText;
    public GameObject deathPanel; // ������ ���� ������

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        LoadPlayerData(); // ��������� ������ ������ ��� ������
        currentHealth = maxHealth;

        // ��������� UI
        UpdateUI();

        // ���������, ��� ������ ������ ��������� ��� ������
        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    void Update()
    {
        if (isDead)
        {
            HandleDeathScreenInput();
            return; // ����������� ����������, ���� �������� �����
        }

        float moveInput = Input.GetAxis("Horizontal");
        animator.SetFloat("horizontalMove", Mathf.Abs(moveInput)); // �������� �������� ��������

        // �������� ������� ������� Shift ��� ���� � ��������
        bool running = Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(moveInput) > 0.01f;
        float currentMoveSpeed = running ? runSpeed : moveSpeed;
        rb.velocity = new Vector2(moveInput * currentMoveSpeed, rb.velocity.y);

        // ������������� �������� �������� ��� ����
        animator.SetBool("running", running);

        // ������ �� �������
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        // �������� ���������� �� �����
        if (isSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);
        }

        if (moveInput > 0) // ���� ��������� ������, ������������ ��������� ������
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput < 0) // ���� ��������� �����, ������������ ��������� �����
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // ����� �� ����� ������ ����
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        // ����� �� ������ ������ ����
        if (Input.GetMouseButtonDown(1))
        {
            SpecialAttack();
        }

        // �������� �� ������� ���� ������������ ������
        if (transform.position.y < -10 && !isDead)
        {
            TakeDamage(maxHealth); // ������� ���� ������ ������������� ��������
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
            jumpCount = 0; // ���������� ������� ������� ��� ������� �����
            canDoubleJump = true; // ��������������� ����������� �������� ������
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

    // ������� ��� ������� �����
    void Attack()
    {
        animator.SetBool("lkmAttacking", true);
        // ������ ��� ����� (����., ����������� ������ � ��������� �����)
        DealDamage(4); // ������� 4 �����
        StartCoroutine(ResetAttack("lkmAttacking"));
    }

    // ������� ��� ����������� �����
    void SpecialAttack()
    {
        animator.SetBool("pkmAttacking", true);
        // ������ ��� ����������� ����� (����., ����������� ������ � ��������� �������� �����)
        DealDamage(11); // ������� 11 �����
        StartCoroutine(ResetAttack("pkmAttacking"));
    }

    // ������� ��� ��������� ����� ������
    void DealDamage(int damage)
    {
        // ����� ��������������, ��� ����� ��������� � �������� ������������� ������� ������ ������
        float attackRange = 1.0f; // ������ �����
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<Enemy>().TakeDamage(damage);
            }
        }
    }

    // ������� ��� ��������� �����
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log("Player took damage: " + damage + ", current health: " + currentHealth);

        if (healCoroutine != null)
        {
            StopCoroutine(healCoroutine); // ������������� ������� �������� �������, ���� ��� ����������
        }
        healCoroutine = StartCoroutine(AutoHeal()); // ��������� ����� �������� �������

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // �������� ��� �����������
    IEnumerator AutoHeal()
    {
        yield return new WaitForSeconds(2f); // ���� 2 �������

        while (currentHealth < maxHealth && !isDead)
        {
            currentHealth += 1; // ��������������� 1 HP
            Debug.Log("Player healed: " + currentHealth);
            yield return new WaitForSeconds(0.1f); // ��������������� �������� ������ 0.1 �������
        }
    }

    // ������� ��� ������ ���������
    void Die()
    {
        isDead = true;
        animator.SetBool("isDead", true);
        Debug.Log("Player is dead, playing death animation");
        rb.velocity = Vector2.zero; // ������������� ���������
        rb.constraints = RigidbodyConstraints2D.FreezeAll; // ������������ ������ ���������
        Time.timeScale = 0f; // ������������� �����
        // ������ ��� ������ ��������� (����., ���������� ����������)
        ShowDeathPanel(); // ����� ���� ������
    }

    // Coroutine ��� ������ ��������� ������
    IEnumerator ResetAttack(string attackType)
    {
        yield return new WaitForSeconds(0.5f); // �������� ��� ���������� �������� �����
        animator.SetBool(attackType, false);
    }

    // ��������� ����� ��� ��������� �������� ��������
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    // ���������� ������� �� ���������
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

    // �������� ��� ���������� ���������
    IEnumerator TemporarySpeedBoost(float duration)
    {
        moveSpeed *= 1.5f;
        runSpeed *= 1.5f;
        yield return new WaitForSeconds(duration);
        moveSpeed /= 1.5f;
        runSpeed /= 1.5f;
    }

    // �������� ��� ���������� ���������� ��������
    IEnumerator TemporaryHealthBoost(float duration)
    {
        int originalHealth = currentHealth;
        currentHealth = Mathf.Min(maxHealth, currentHealth + 50);
        yield return new WaitForSeconds(duration);
        currentHealth = originalHealth;
    }

    // �������� ��� ���������� ���������� �����
    IEnumerator TemporaryDamageBoost(float duration)
    {
        int originalDamage = 4; // ������� ���� ��� ������� �����
        int originalSpecialDamage = 11; // ������� ���� ��� ����������� �����
        int boostedDamage = originalDamage + 10;
        int boostedSpecialDamage = originalSpecialDamage + 10;

        DealDamage(boostedDamage);
        DealDamage(boostedSpecialDamage);

        yield return new WaitForSeconds(duration);

        DealDamage(originalDamage);
        DealDamage(originalSpecialDamage);
    }

    // ����� ��� ��������� �����
    public void GainXP(int amount)
    {
        currentXP += amount;
        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }

        // ��������� UI
        UpdateUI();
        SavePlayerData(); // ��������� ������ ������ ����� ��������� �����
    }

    // ����� ��� ��������� ������
    void LevelUp()
    {
        currentXP -= xpToNextLevel;
        currentLevel++;
        xpToNextLevel += 100; // ���������� ���������� � ����� ��� ���������� ������
        maxHealth += 10; // ���������� ������������� �������� ��� ��������� ������
        currentHealth = maxHealth; // ������ �������������� ��������
        Debug.Log("Level Up! Current Level: " + currentLevel);

        // ��������� UI
        UpdateUI();

        SavePlayerData(); // ��������� ������ ������
    }

    // ���������� UI ���������
    void UpdateUI()
    {
        levelText.text = "Level: " + currentLevel;
        xpText.text = "XP: " + currentXP + " / " + xpToNextLevel;
    }

    // ���������� ������ ������
    void SavePlayerData()
    {
        PlayerPrefs.SetInt("PlayerLevel", currentLevel);
        PlayerPrefs.SetInt("PlayerXP", currentXP);
        PlayerPrefs.SetInt("PlayerXPToNextLevel", xpToNextLevel);
        PlayerPrefs.SetInt("PlayerHealth", currentHealth);
        PlayerPrefs.SetInt("PlayerMaxHealth", maxHealth);
        PlayerPrefs.Save(); // ����� ���������� ������
    }

    // �������� ������ ������
    void LoadPlayerData()
    {
        currentLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
        currentXP = PlayerPrefs.GetInt("PlayerXP", 0);
        xpToNextLevel = PlayerPrefs.GetInt("PlayerXPToNextLevel", 100);
        currentHealth = PlayerPrefs.GetInt("PlayerHealth", maxHealth);
        maxHealth = PlayerPrefs.GetInt("PlayerMaxHealth", maxHealth);

        // ��������� UI
        UpdateUI();
    }

    // ����� ��� ����������� ���� ������
    void ShowDeathPanel()
    {
        if (deathPanel != null)
        {
            deathPanel.SetActive(true); // ���������� ���� ������
        }
    }

    // ����� ��� ����������� ����
    public void RestartGame()
    {
        Debug.Log("Restarting game...");
        Time.timeScale = 1f; // ��������������� �����
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // ������������ ������� �����
    }

    // ����� ��� ������ � ����
    public void ReturnToMenu()
    {
        Debug.Log("Returning to menu...");
        Time.timeScale = 1f; // ��������������� �����
        SceneManager.LoadScene("MainMenuScene"); // �������� ����� ���� (������� ���������� ��� �����)
    }

    // ����� ��� ������ �� ����
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit(); // ����� �� ����
    }
}
