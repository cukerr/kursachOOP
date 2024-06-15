using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 30;
    private int currentHealth;
    public float speed = 2f; // �������� �������� �����
    public int damageToPlayer = 6; // ����, ��������� ������ ��� ������������
    private Transform playerTransform; // ������ �� ��������� ������
    private bool isTouchingPlayer = false; // ���� ��� �������� �������� � �������
    private Animator animator; // ������ �� ��������� Animator
    private SpriteRenderer spriteRenderer; // ������ �� ��������� SpriteRenderer
    public int xpReward = 10; // ����, ������� ������� ����� �� �������� ����� �����

    void Start()
    {
        currentHealth = maxHealth;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // ��������������, ��� � ������ ��� "Player"
        animator = GetComponent<Animator>(); // �������� ��������� Animator
        spriteRenderer = GetComponent<SpriteRenderer>(); // �������� ��������� SpriteRenderer
        animator.SetBool("isMoving", false); // ������������� ��������� ��������� ��������
    }

    void Update()
    {
        // ������ ���������� �� �������
        if (playerTransform != null)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // ���������� ���������
            bool isMoving = direction.magnitude > 0.01f;
            animator.SetBool("isMoving", isMoving);
            animator.SetFloat("horizontalMove", Mathf.Abs(direction.x));

            // ������� ������� � ����������� �� ����������� ��������
            if (direction.x > 0)
            {
                spriteRenderer.flipX = false; // ��������� ������
            }
            else if (direction.x < 0)
            {
                spriteRenderer.flipX = true; // ��������� �����
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
        // ������ ��� ������ �����
        Debug.Log("Enemy died");
        PlayerScript playerScript = playerTransform.GetComponent<PlayerScript>();
        if (playerScript != null)
        {
            playerScript.GainXP(xpReward); // ���� ������ ���� �� �������� �����
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
                animator.SetTrigger("attack"); // ����� �������� �����
                playerScript.TakeDamage(damageToPlayer);
            }
            yield return new WaitForSeconds(1f); // ������� ���� ������ 1 �������
        }
    }
}
