using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public int damage = 10;

    [Header("Health")]
    public float maxHealth = 100;
    public float currentHealth;

    private float attackTimer = 0f;

    private PlayerController playerController;

    public bool canMove;

    public bool canPlay = false;

    private float flashTimer = 0f;
    public float flashDuration = 0.4f;

    public Color flashColor = Color.red;

    private void Start()
    {
        playerController = GameObject.Find("player").GetComponent<PlayerController>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (playerController == null) return;

        if (canMove && canPlay)
        {

            Vector3 playerPos = playerController.GetPosition();


            float distance = Vector2.Distance(transform.position, playerPos);

            if (distance > attackRange)
            {
                FollowPlayer();
            }
            else
            {
                AttackPlayer();
            }

            if (attackTimer > 0f)
                attackTimer -= Time.deltaTime;
            //RotateCharacter(playerPos);

            if (flashTimer > 0f)
            {
                gameObject.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, flashColor, flashTimer / flashDuration);
                flashTimer -= Time.deltaTime;
            }
        }
    }

    void FollowPlayer()
    {
        Vector2 direction = (playerController.GetPosition() - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    void AttackPlayer()
    {
        if (attackTimer <= 0f)
        {
            if (playerController != null)
            {
                playerController.TakeDamage(damage);
                //Debug.Log("Enemy hit the player!");
                attackTimer = attackCooldown;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0f)
        {
            //Die();
            Debug.Log("damaged " + damage);
        }
        flashTimer = flashDuration;

    }

    private void Die()
    {
        playerController.objectsInTrigger.Remove(gameObject);
        gameObject.SetActive(false);
    }
    void RotateCharacter(Vector3 targetPos)
    {
        Vector3 rotateDirection = (targetPos - transform.position).normalized;
        rotateDirection.z = 0;

        float angle = Mathf.Atan2(rotateDirection.y, rotateDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

    }
}
