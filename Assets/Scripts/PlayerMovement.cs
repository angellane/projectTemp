using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    private float moveSpeed = 3f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    public LayerMask wallLayer;
    public static bool hasStock = false;
    private ReputationBar reputationBar;
    private bool isOnRestock = false;
    private GameObject restockObject;
    private List<GameObject> shelfObjects = new List<GameObject>();
    private GameObject currentShelf = null;
    private float holdTimer = 0f;
    private const float holdDuration = 2f;
    private Dictionary<GameObject, float> shelfCooldowns = new Dictionary<GameObject, float>();
    private const float shelfCooldownDuration = 20f;
    [Header("Coffee Speed Boost")]
    public float coffeeSpeedMultiplier = 2f;
    public float coffeeSpeedDuration = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        FindRestockAndShelfObjects();
        reputationBar = FindObjectOfType<ReputationBar>();
        if (reputationBar == null)
        {
            Debug.LogWarning("ReputationBar component not found in the scene.");
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindRestockAndShelfObjects();
        reputationBar = FindObjectOfType<ReputationBar>();
        if (reputationBar == null)
        {
            Debug.LogWarning("ReputationBar component not found in the scene.");
        }
    }

    void FindRestockAndShelfObjects()
    {
        restockObject = GameObject.Find("Restock");
        if (restockObject == null)
        {
            Debug.LogWarning("Restock object not found in the scene. Make sure the name is correct.");
        }
        shelfObjects.Clear();
        for (int i = 1; i <= 12; i++)
        {
            GameObject shelf = GameObject.Find("Shelf" + i);
            if (shelf != null)
            {
                shelfObjects.Add(shelf);
                shelfCooldowns[shelf] = -shelfCooldownDuration;
            }
            else
            {
                Debug.LogWarning($"Shelf{i} object not found in the scene. Make sure the name is correct.");
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 adjustedMovement = AdjustMovement(moveInput);
        rb.linearVelocity = adjustedMovement * moveSpeed;
        if (adjustedMovement != Vector2.zero)
        {
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);
    }

    private Vector2 AdjustMovement(Vector2 direction)
    {
        bool canMoveX = CanMove(new Vector2(direction.x, 0));
        bool canMoveY = CanMove(new Vector2(0, direction.y));
        return new Vector2(canMoveX ? direction.x : 0, canMoveY ? direction.y : 0);
    }

    private bool CanMove(Vector2 direction)
    {
        float checkDistance = 0.1f;
        Vector2 size = rb.GetComponent<Collider2D>().bounds.size * 0.9f;
        RaycastHit2D hit = Physics2D.BoxCast(rb.position, size, 0, direction, checkDistance, LayerMask.GetMask("Walls"));
        return hit.collider == null;
    }

    void Update()
    {
        Vector2 adjustedMovement = AdjustMovement(moveInput);
        if (moveInput != Vector2.zero)
        {
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
        }
        if (adjustedMovement != Vector2.zero && CanMove(adjustedMovement))
        {
            rb.linearVelocity = adjustedMovement * moveSpeed;
            animator.SetBool("isWalking", true);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);
        }
        if (isOnRestock && Keyboard.current.eKey.wasPressedThisFrame)
        {
            hasStock = true;
        }
        if (currentShelf != null)
        {
            if (shelfCooldowns[currentShelf] + shelfCooldownDuration > Time.time)
            {
                return;
            }
            if (Keyboard.current.eKey.isPressed)
            {
                holdTimer += Time.deltaTime;
                if (holdTimer >= holdDuration)
                {
                    if (hasStock)
                    {
                        hasStock = false;
                        if (reputationBar != null)
                        {
                            reputationBar.IncreaseReputation(10f);
                        }
                        else
                        {
                            Debug.LogWarning("ReputationBar component not found.");
                        }
                        shelfCooldowns[currentShelf] = Time.time;
                        holdTimer = 0f;
                    }
                    else
                    {
                        Debug.Log("Player does not have stock to interact with the shelf.");
                    }
                }
            }
            else
            {
                holdTimer = 0f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == restockObject)
        {
            isOnRestock = true;
        }
        else if (shelfObjects.Contains(collision.gameObject))
        {
            currentShelf = collision.gameObject;
        }
        else if (collision.CompareTag("Coffee"))
        {
            CoffeeSpawner spawner = FindObjectOfType<CoffeeSpawner>();
            if (spawner != null)
            {
                spawner.CoffeeCollected();
                ApplySpeedBoost(coffeeSpeedMultiplier, coffeeSpeedDuration);
            }
            else
            {
                Debug.LogWarning("CoffeeSpawner not found in scene");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == restockObject)
        {
            isOnRestock = false;
        }
        if (shelfObjects.Contains(collision.gameObject))
        {
            currentShelf = null;
            holdTimer = 0f;
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ApplySpeedBoost(float multiplier, float duration)
    {
        StartCoroutine(SpeedBoostCoroutine(multiplier, duration));
    }

    private IEnumerator SpeedBoostCoroutine(float multiplier, float duration)
    {
        float originalSpeed = moveSpeed;
        moveSpeed *= multiplier;
        yield return new WaitForSeconds(duration);
        moveSpeed = originalSpeed;
    }
}