using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Permissions")]
    public bool canRun = false;
public bool canDash = false;
    public bool canHit = false;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;

    [Header("Dash")]
    public float dashForce = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public float dashStaminaCost = 20f;

    [Header("Health")]
    public float maxHealth = 100;
    public float currentHealth;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 15f;
    public float runStaminaDrainRate = 25f;

    private Vector2 moveInput;
    private Rigidbody2D rb;
    public Camera mainCamera;
    private bool isDashing = false;
    private float dashCooldownTimer = 0f;
    private bool isRunning = false;
    public GameObject attackArea;
    public LayerMask targetLayer;
    public GameObject rotateArea;

    public List<GameObject> objectsInTrigger;
    public float attackCooldown = 0.8f;
    private float attackTimer = 0f;
    public float hitDamage = 50f;
    private bool isAttacking = false;
    public Slider healthSlider;
    public Slider staminaSlider;
    public bool canPlay = false;
    public LevelManager levelManager;
    public int staminaRefreshFrame = 0;

    public Animator anim;
    private SpriteRenderer spriteRenderer;
    public float attackDuration = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        healthSlider.value = maxHealth;
        healthSlider.maxValue = maxHealth;
        staminaSlider.value = maxStamina;
        staminaSlider.maxValue = maxStamina;
        Physics2D.IgnoreLayerCollision(6,7);
        levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (canPlay)
        {
            HandleInput();
            HandleDash();
            HandleMouseRotate();
            RegenerateStamina();
        }
        else
        {
            moveInput = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
        }

    }

    void HandleMouseRotate()
    {
        Vector3 pos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.nearClipPlane));

        Vector3 rotateDirection = (pos- transform.position).normalized;
        rotateDirection.z = 0;

        float  angle = Mathf.Atan2(rotateDirection.y, rotateDirection.x) * Mathf.Rad2Deg;
        rotateArea.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

    }


    void FixedUpdate()
    {
        if (canPlay)
        {
            if (!isDashing)
            {
                Move();
            }
            attackTimer -= Time.fixedDeltaTime;
            if (staminaRefreshFrame >= 10)
            {
                UpdateStamina();
                staminaRefreshFrame = 0;
            }
            else staminaRefreshFrame++;
        }
        else
        {
            rb.linearVelocity = Vector2.zero; 
        }

    }

    void HandleInput()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
       if (moveInput == Vector2.zero && anim.GetBool("isRunning") == true)
        {
            anim.SetBool("isRunning", false);
        }
       else if (moveInput != Vector2.zero && anim.GetBool("isRunning") == false)
        {
            anim.SetBool("isRunning", true);
        }
       if (moveInput.x < 0 && spriteRenderer.flipX == false)
        {
            spriteRenderer.flipX = true;
        }
       else if (moveInput.x > 0 && spriteRenderer.flipX == true)
        {
            spriteRenderer.flipX = false;

        }

        if (Input.GetKeyDown(KeyCode.Space) && dashCooldownTimer <= 0f && moveInput != Vector2.zero && canDash && currentStamina >= dashStaminaCost)
        {
            StartCoroutine(Dash());
        }
        if (Input.GetKey(KeyCode.Mouse0) && canHit && attackTimer <= 0 && !isAttacking)
        {
            StartCoroutine(Hit());
        }
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && canRun && currentStamina > 1)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }
    }

    void Move()
    {
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        rb.linearVelocity = moveInput * currentSpeed;
        if (isRunning)
        {
            currentStamina -= runStaminaDrainRate * Time.fixedDeltaTime;
            currentStamina = Mathf.Max(0f, currentStamina);
        }
        //anim.Play("Dino-run");
    }

    System.Collections.IEnumerator Dash()
    {
        isDashing = true;
        rb.linearVelocity = moveInput * dashForce;
        dashCooldownTimer = dashCooldown;
        currentStamina -= dashStaminaCost;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
    }

    void HandleDash()
    {
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    void RegenerateStamina()
    {
        if (!isRunning && !isDashing && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0f)
        {
            Die();
        }
        UpdateHealth();
    }

    void Die()
    {
        int deaths = PlayerPrefs.GetInt("Deaths", 0);
        deaths += 1;
        PlayerPrefs.SetInt("Deaths", deaths);
        levelManager.ShowDeathScreen(deaths);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    System.Collections.IEnumerator Hit()
    {

        isAttacking = true;
        anim.SetBool("isAttacking", true);
        //attackArea.SetActive(true);
        try
        {
            foreach (GameObject hit in objectsInTrigger)
            {
                
                if (hit != null)
                {
                    
                    hit.GetComponent<EnemyController>().TakeDamage(hitDamage);
                    if (hit.GetComponent<EnemyController>().currentHealth <= 0)
                    {
                        objectsInTrigger.Remove(hit);
                        if (levelManager.isKillEveryone)
                        {
                            levelManager.AddItem();
                        }
                        Destroy(hit);
                    }
                }
            }
            attackTimer = attackCooldown;

        }
        catch
        {
            //Debug.Log("Ups");
        }

        yield return new WaitForSeconds(attackDuration);


        isAttacking = false;
        anim.SetBool("isAttacking", false);

    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (!isAttacking)
        {
            objectsInTrigger.Remove(enemy);

        }
    }

    public void AddEnemy(GameObject enemy)
    {
        if (!isAttacking)
        {
            objectsInTrigger.Add(enemy);

        }
    }
    
    public void AddHealth(float addHealth)
    {
        currentHealth += addHealth;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        UpdateHealth();
    }
    public void AddStamina(float addStamina)
    {
        currentStamina += addStamina;
        if (currentStamina > maxStamina)
        {
            currentStamina = maxStamina;
        }
        UpdateStamina();
    }


    private void UpdateHealth()
    {
        healthSlider.value = currentHealth;
    }
    private void UpdateStamina()
    {
        staminaSlider.value = currentStamina;
    }
}