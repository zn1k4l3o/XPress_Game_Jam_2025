using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public bool isRanged = false;
    public bool isFinal = false;

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

    public Animator animEnm;
    public GameObject projectile; 

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        playerController = GameObject.Find("player").GetComponent<PlayerController>();
        currentHealth = maxHealth;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        isFinal = false;
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

                if (isRanged && attackTimer <= 0)
                {
                    Shoot();
                    attackTimer = attackCooldown;
                }
                else
                {
                StartCoroutine(AttackPlayer());
                }
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
        if (direction.x < 0 && spriteRenderer.flipX == false)
        {
            spriteRenderer.flipX = true;
        }
        else if (direction.x > 0 && spriteRenderer.flipX == true)
        {
            spriteRenderer.flipX = false;

        }
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    System.Collections.IEnumerator AttackPlayer()
    {   
        
        if (attackTimer <= 0f)
        {
            animEnm.SetBool("isAttacking", true);
            if (playerController != null)
            {
                playerController.TakeDamage(damage);
                //Debug.Log("Enemy hit the player!");
                attackTimer = attackCooldown;
            }
            yield return new WaitForSeconds(0.5f);
            animEnm.SetBool("isAttacking", false);
        }
        
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (isFinal && currentHealth <= 0f)
        {
            GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>().FinalWin();
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

    void Shoot()
    {
        GameObject newProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
        newProjectile.GetComponent<Projectile>().targetPos = playerController.transform.position;
    }
}
