
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    private Rigidbody2D myRigidBody;
    private Animator anim;

    private Vector2[] waypoints;
    private int waypointIndex = 0;
    private bool isWaiting = false;

    [SerializeField] private List<int> skipListPath1;
    [SerializeField] private List<int> lookDownListPath1;

    [SerializeField] private List<int> skipListPath2;
    [SerializeField] private List<int> lookDownListPath2;

    private List<int> currentSkipList;
    private List<int> currentLookDownList;

    private float waypointThreshold = 0.2f;

    [SerializeField] private Transform npcSpawner;

    private Vector2[] customerLineSpots;
    private int currentSpotIndex = 8;
    private bool isInLine = false;

    private bool isAtSpot0 = false;
    private GameObject checkout;

    private Vector2[] exitPathWaypoints;
    private bool isExiting = false;

    public enum PathChoice { Path1, Path2, Random }
    [SerializeField] private PathChoice pathChoice = PathChoice.Random;

    private ReputationBar reputationBar;

    [SerializeField] private float reputationIncreaseAmount = 50f;

    void Start()
    {
        anim = GetComponent<Animator>();
        myRigidBody = GetComponent<Rigidbody2D>();

        checkout = GameObject.Find("Checkout");
        if (checkout == null)
        {
            Debug.LogError("Checkout object not found! Make sure it exists in the scene.");
        }

        customerLineSpots = new Vector2[9];
        for (int i = 0; i <= 8; i++)
        {
            GameObject spot = GameObject.Find($"Spot{i}");
            if (spot != null)
            {
                customerLineSpots[i] = spot.transform.position;
                Debug.Log($"Spot {i} found at {spot.transform.position}");
            }
            else
            {
                Debug.LogError($"Spot{i} not found in the scene!");
            }
        }

        GameObject chosenPath = null;
        switch (pathChoice)
        {
            case PathChoice.Path1:
                chosenPath = GameObject.Find("Path1");
                break;
            case PathChoice.Path2:
                chosenPath = GameObject.Find("Path2");
                break;
            case PathChoice.Random:
                chosenPath = Random.Range(0, 2) == 0 ? GameObject.Find("Path1") : GameObject.Find("Path2");
                break;
        }

        if (chosenPath == null)
        {
            Debug.LogError("Path not found! Make sure Path1 and Path2 exist in the scene.");
            return;
        }

        if (chosenPath.name == "Path1")
        {
            currentSkipList = skipListPath1;
            currentLookDownList = lookDownListPath1;
        }
        else
        {
            currentSkipList = skipListPath2;
            currentLookDownList = lookDownListPath2;
        }

        List<Vector2> waypointList = new List<Vector2>();
        foreach (Transform child in chosenPath.transform)
        {
            waypointList.Add(child.position);
        }
        waypoints = waypointList.ToArray();

        if (waypoints.Length == 0)
        {
            Debug.LogError("No waypoints found in the chosen path!");
            return;
        }

        for (int i = 0; i < waypoints.Length; i++)
        {
            Debug.Log($"Waypoint {i}: {waypoints[i]}");
        }

        if (npcSpawner != null)
        {
            transform.position = npcSpawner.position;
        }
        else
        {
            Debug.LogWarning("NPC Spawner not assigned! Using default position.");
        }

        reputationBar = FindFirstObjectByType<ReputationBar>();
        if (reputationBar == null)
        {
            Debug.LogError("ReputationBar component not found in the scene!");
        }
    }

    void FixedUpdate()
    {
        if (isExiting)
        {
            FollowExitPath();
        }
        else if (!isInLine)
        {
            if (!isWaiting)
            {
                Vector2 target = waypoints[waypointIndex];
                Vector2 direction = (target - (Vector2)transform.position).normalized;

                myRigidBody.linearVelocity = direction * speed;

                anim.SetBool("isWalking", true);
                anim.SetFloat("MoveX", direction.x);
                anim.SetFloat("MoveY", direction.y);

                if (Vector2.Distance(transform.position, target) < waypointThreshold)
                {
                    Debug.Log($"Reached Waypoint {waypointIndex} in {(currentSkipList == skipListPath1 ? "Path1" : "Path2")}");

                    if (waypointIndex == waypoints.Length - 1)
                    {
                        Debug.Log("Reached the last waypoint. Walking to Spot8...");
                        StartWalkingToSpot8();
                        return;
                    }

                    if (currentSkipList.Contains(waypointIndex))
                    {
                        waypointIndex = (waypointIndex + 1) % waypoints.Length;
                        Debug.Log($"Skipping Waypoint {waypointIndex}");
                    }
                    else
                    {
                        if (Random.Range(0, 2) == 0)
                        {
                            StartCoroutine(WaitAtWaypoint());
                        }
                        else
                        {
                            waypointIndex = (waypointIndex + 1) % waypoints.Length;
                            Debug.Log($"Randomly Skipping Waypoint {waypointIndex}");
                        }
                    }
                }
            }
        }
        else
        {
            MoveInLine();
        }
    }

    private IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        myRigidBody.linearVelocity = Vector2.zero;
        anim.SetBool("isWalking", false);

        if (currentLookDownList.Contains(waypointIndex))
        {
            Debug.Log($"Looking down at Waypoint {waypointIndex}");
            anim.SetFloat("MoveX", 0);
            anim.SetFloat("MoveY", -1);
        }
        else
        {
            Debug.Log($"Pausing at Waypoint {waypointIndex} without looking down");
            anim.SetFloat("MoveX", 0);
            anim.SetFloat("MoveY", 1);
        }

        yield return new WaitForSeconds(2f);

        waypointIndex = (waypointIndex + 1) % waypoints.Length;
        Debug.Log($"Next Waypoint: {waypointIndex}");

        isWaiting = false;
    }

    private void MoveInLine()
    {
        if (currentSpotIndex > 0)
        {
            Vector2 nextSpot = customerLineSpots[currentSpotIndex - 1];
            if (IsSpotEmpty(nextSpot))
            {
                Vector2 direction = (nextSpot - (Vector2)transform.position).normalized;
                myRigidBody.linearVelocity = direction * speed;

                anim.SetBool("isWalking", true);
                anim.SetFloat("MoveX", direction.x);
                anim.SetFloat("MoveY", direction.y);

                if (Vector2.Distance(transform.position, nextSpot) < waypointThreshold)
                {
                    currentSpotIndex--;
                    Debug.Log($"Moved to Spot {currentSpotIndex}");
                }
            }
            else
            {
                myRigidBody.linearVelocity = Vector2.zero;
                anim.SetBool("isWalking", false);
                Debug.Log($"Spot {currentSpotIndex - 1} is occupied. Waiting...");
            }
        }
        else
        {
            myRigidBody.linearVelocity = Vector2.zero;
            anim.SetBool("isWalking", false);
            Debug.Log("Reached the front of the line!");
            isAtSpot0 = true;
        }
    }

    private bool IsSpotEmpty(Vector2 spot)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(spot, 0.1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                Debug.Log($"Spot at {spot} is occupied by {collider.gameObject.name}");
                return false;
            }
        }
        return true;
    }

    private void JoinCustomerLine()
    {
        if (customerLineSpots != null && customerLineSpots.Length > 0)
        {
            transform.position = customerLineSpots[currentSpotIndex];
            isInLine = true;
            Debug.Log("Joined the customer line at Spot8!");
        }
        else
        {
            Debug.LogError("Customer line spots are not assigned!");
        }
    }

    void Update()
    {
        if (isAtSpot0 && !isExiting && Input.GetKeyDown(KeyCode.E))
        {
            if (IsPlayerOnCheckout())
            {
                Debug.Log("Player pressed E near NPC at Spot0 and is on Checkout. NPC will follow ExitPath.");

                if (reputationBar != null)
                {
                    reputationBar.IncreaseReputation(reputationIncreaseAmount);
                }
                else
                {
                    Debug.LogError("ReputationBar reference is not assigned!");
                }

                StartExitPath();
            }
        }
    }

    private bool IsPlayerOnCheckout()
    {
        if (checkout != null)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(checkout.transform.position, 0.1f);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void StartExitPath()
    {
        GameObject exitPath = GameObject.Find("ExitPath");
        if (exitPath == null)
        {
            Debug.LogError("ExitPath not found! Make sure it exists in the scene.");
            return;
        }

        List<Vector2> exitWaypointList = new List<Vector2>();
        foreach (Transform child in exitPath.transform)
        {
            exitWaypointList.Add(child.position);
        }
        exitPathWaypoints = exitWaypointList.ToArray();

        if (exitPathWaypoints.Length == 0)
        {
            Debug.LogError("No waypoints found in the ExitPath!");
            return;
        }

        isExiting = true;
        waypointIndex = 0;
        Debug.Log("Started following ExitPath.");
    }

    private void FollowExitPath()
    {
        if (waypointIndex < exitPathWaypoints.Length)
        {
            Vector2 target = exitPathWaypoints[waypointIndex];
            Vector2 direction = (target - (Vector2)transform.position).normalized;

            myRigidBody.linearVelocity = direction * speed;

            anim.SetBool("isWalking", true);
            anim.SetFloat("MoveX", direction.x);
            anim.SetFloat("MoveY", direction.y);

            if (Vector2.Distance(transform.position, target) < waypointThreshold)
            {
                Debug.Log($"Reached ExitPath Waypoint {waypointIndex}");

                waypointIndex++;

                if (waypointIndex == exitPathWaypoints.Length)
                {
                    Debug.Log("Reached the end of ExitPath. NPC will disappear.");
                    Destroy(gameObject);
                }
            }
        }
    }

    private void StartWalkingToSpot8()
    {
        isInLine = true;
        currentSpotIndex = 8;
        Debug.Log("Walking to Spot8...");
    }
}