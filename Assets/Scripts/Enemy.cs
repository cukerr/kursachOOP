using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 30;
    private int currentHealth;
    public float speed = 2f; // Скорость движения врага
    public int damageToPlayer = 6; // Урон, наносимый игроку при столкновении
    private Transform playerTransform; // Ссылка на трансформ игрока
    private bool isTouchingPlayer = false; // Флаг для проверки контакта с игроком
    private Animator animator; // Ссылка на компонент Animator
    private SpriteRenderer spriteRenderer; // Ссылка на компонент SpriteRenderer
    public int xpReward = 10; // Опыт, который получит игрок за убийство этого врага

    void Start()
    {
        currentHealth = maxHealth;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // Предполагается, что у игрока тег "Player"
        animator = GetComponent<Animator>(); // Получаем компонент Animator
        spriteRenderer = GetComponent<SpriteRenderer>(); // Получаем компонент SpriteRenderer
        animator.SetBool("isMoving", false); // Устанавливаем начальное состояние анимации
    }

    void Update()
    {
        // Логика следования за игроком
        if (playerTransform != null)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Управление анимацией
            bool isMoving = direction.magnitude > 0.01f;
            animator.SetBool("isMoving", isMoving);
            animator.SetFloat("horizontalMove", Mathf.Abs(direction.x));

            // Поворот спрайта в зависимости от направления движения
            if (direction.x > 0)
            {
                spriteRenderer.flipX = false; // Повернуть вправо
            }
            else if (direction.x < 0)
            {
                spriteRenderer.flipX = true; // Повернуть влево
            }
        }
        else
        {
            animator.SetBool("isMoving", false);
            animator.SetFloat("horizontalMove", 0f);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Enemy took damage: " + damage + ", current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Логика для смерти врага
        Debug.Log("Enemy died");
        PlayerScript playerScript = playerTransform.GetComponent<PlayerScript>();
        if (playerScript != null)
        {
            playerScript.GainXP(xpReward); // Даем игроку опыт за убийство врага
        }
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true;
            StartCoroutine(DamagePlayerOverTime(collision.gameObject));
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = false;
        }
    }

    IEnumerator DamagePlayerOverTime(GameObject player)
    {
        PlayerScript playerScript = player.GetComponent<PlayerScript>();
        while (isTouchingPlayer)
        {
            if (playerScript != null)
            {
                animator.SetTrigger("attack"); // Вызов анимации атаки
                playerScript.TakeDamage(damageToPlayer);
            }
            yield return new WaitForSeconds(1f); // Наносим урон каждые 1 секунду
        }
    }
}
