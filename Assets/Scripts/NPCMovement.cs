using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    private Rigidbody2D myRigidBody;
    private Animator anim;

    private Vector2[] waypoints; // Stores the waypoints for the chosen path
    private int waypointIndex = 0;
    private bool isWaiting = false;

    // Lists for Path1
    [SerializeField] private List<int> skipListPath1; // Waypoints to skip in Path1
    [SerializeField] private List<int> lookDownListPath1; // Waypoints to look down in Path1

    // Lists for Path2
    [SerializeField] private List<int> skipListPath2; // Waypoints to skip in Path2
    [SerializeField] private List<int> lookDownListPath2; // Waypoints to look down in Path2

    private List<int> currentSkipList; // Skip list for the chosen path
    private List<int> currentLookDownList; // Look down list for the chosen path

    private float waypointThreshold = 0.2f; // Stopping threshold

    [SerializeField] private Transform npcSpawner; // Reference to the NPC spawner

    // Customer Line Logic
    private Vector2[] customerLineSpots; // Array of Vector2 positions for the customer line
    private int currentSpotIndex = 8; // Start at the last spot (Spot8)
    private bool isInLine = false; // Whether the NPC is in the customer line

    // Interaction Logic
    private bool isAtSpot0 = false; // Whether the NPC is at Spot0
    private GameObject checkout; // Reference to the Checkout object

    // Exit Path Logic
    private Vector2[] exitPathWaypoints; // Stores the waypoints for the ExitPath
    private bool isExiting = false; // Whether the NPC is following the ExitPath

    // Path Selection
    public enum PathChoice { Path1, Path2, Random }
    [SerializeField] private PathChoice pathChoice = PathChoice.Random; // Default to Random

    // Reputation System
    private ReputationBar reputationBar; // Reference to the ReputationBar component

    [SerializeField] private float reputationIncreaseAmount = 50f; // Customizable reputation increase value

    void Start()
    {
        anim = GetComponent<Animator>();
        myRigidBody = GetComponent<Rigidbody2D>();

        // Find the Checkout object by name
        checkout = GameObject.Find("Checkout");
        if (checkout == null)
        {
            Debug.LogError("Checkout object not found! Make sure it exists in the scene.");
        }

        // Initialize customer line spots by name
        customerLineSpots = new Vector2[9]; // 9 spots (Spot0 to Spot8)
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

        // Choose the path based on the selected option
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

        // Set the current skip and look down lists based on the chosen path
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

        // Store waypoint positions as Vector2
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

        // Log waypoints for debugging
        for (int i = 0; i < waypoints.Length; i++)
        {
            Debug.Log($"Waypoint {i}: {waypoints[i]}");
        }

        // Set NPC to start at the spawner's position
        if (npcSpawner != null)
        {
            transform.position = npcSpawner.position;
        }
        else
        {
            Debug.LogWarning("NPC Spawner not assigned! Using default position.");
        }

        // Find the ReputationBar component in the scene
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
            // Follow the ExitPath
            FollowExitPath();
        }
        else if (!isInLine)
        {
            // Existing movement logic for paths
            if (!isWaiting)
            {
                Vector2 target = waypoints[waypointIndex];
                Vector2 direction = (target - (Vector2)transform.position).normalized;

                // Move the NPC
                myRigidBody.linearVelocity = direction * speed;

                anim.SetBool("isWalking", true);
                anim.SetFloat("MoveX", direction.x);
                anim.SetFloat("MoveY", direction.y);

                // Check if NPC reached the waypoint
                if (Vector2.Distance(transform.position, target) < waypointThreshold)
                {
                    Debug.Log($"Reached Waypoint {waypointIndex} in {(currentSkipList == skipListPath1 ? "Path1" : "Path2")}");

                    // Check if this is the last waypoint
                    if (waypointIndex == waypoints.Length - 1)
                    {
                        Debug.Log("Reached the last waypoint. Walking to Spot8...");
                        StartWalkingToSpot8();
                        return; // Exit to avoid further movement logic
                    }

                    // Check if the current waypoint is in the skip list
                    if (currentSkipList.Contains(waypointIndex))
                    {
                        // Skip this waypoint and move to the next one
                        waypointIndex = (waypointIndex + 1) % waypoints.Length;
                        Debug.Log($"Skipping Waypoint {waypointIndex}");
                    }
                    else
                    {
                        // 50% chance to stop or skip
                        if (Random.Range(0, 2) == 0) // Random.Range(0, 2) returns 0 or 1
                        {
                            // Stop at this waypoint
                            StartCoroutine(WaitAtWaypoint());
                        }
                        else
                        {
                            // Skip this waypoint and move to the next one
                            waypointIndex = (waypointIndex + 1) % waypoints.Length;
                            Debug.Log($"Randomly Skipping Waypoint {waypointIndex}");
                        }
                    }
                }
            }
        }
        else
        {
            // Customer line logic
            MoveInLine();
        }
    }

    private IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        myRigidBody.linearVelocity = Vector2.zero; // Stop movement
        anim.SetBool("isWalking", false);

        // Check if the current waypoint is in the look down list
        if (currentLookDownList.Contains(waypointIndex))
        {
            Debug.Log($"Looking down at Waypoint {waypointIndex}");
            anim.SetFloat("MoveX", 0); // Stop horizontal movement
            anim.SetFloat("MoveY", -1); // Set to look down
        }
        else
        {
            Debug.Log($"Pausing at Waypoint {waypointIndex} without looking down");
            anim.SetFloat("MoveX", 0); // Stop horizontal movement
            anim.SetFloat("MoveY", 1); // Set to look up (or default idle)
        }

        yield return new WaitForSeconds(2f); // Pause at waypoint

        // Move to the next waypoint
        waypointIndex = (waypointIndex + 1) % waypoints.Length;
        Debug.Log($"Next Waypoint: {waypointIndex}");

        isWaiting = false;
    }

    private void MoveInLine()
    {
        if (currentSpotIndex > 0)
        {
            // Check if the spot below is unoccupied
            Vector2 nextSpot = customerLineSpots[currentSpotIndex - 1];
            if (IsSpotEmpty(nextSpot))
            {
                // Move to the next spot
                Vector2 direction = (nextSpot - (Vector2)transform.position).normalized;
                myRigidBody.linearVelocity = direction * speed;

                anim.SetBool("isWalking", true);
                anim.SetFloat("MoveX", direction.x);
                anim.SetFloat("MoveY", direction.y);

                // Check if NPC reached the next spot
                if (Vector2.Distance(transform.position, nextSpot) < waypointThreshold)
                {
                    currentSpotIndex--; // Move to the next spot
                    Debug.Log($"Moved to Spot {currentSpotIndex}");
                }
            }
            else
            {
                // Stop moving if the next spot is occupied
                myRigidBody.linearVelocity = Vector2.zero;
                anim.SetBool("isWalking", false);
                Debug.Log($"Spot {currentSpotIndex - 1} is occupied. Waiting...");
            }
        }
        else
        {
            // NPC has reached Spot0 (front of the line)
            myRigidBody.linearVelocity = Vector2.zero;
            anim.SetBool("isWalking", false);
            Debug.Log("Reached the front of the line!");
            isAtSpot0 = true; // NPC is now at Spot0
        }
    }

    private bool IsSpotEmpty(Vector2 spot)
    {
        // Check if the spot is unoccupied
        Collider2D[] colliders = Physics2D.OverlapCircleAll(spot, 0.1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject) // Ignore self
            {
                Debug.Log($"Spot at {spot} is occupied by {collider.gameObject.name}");
                return false; // Spot is occupied
            }
        }
        return true; // Spot is empty
    }

    // Call this method to add the NPC to the customer line
    private void JoinCustomerLine()
    {
        if (customerLineSpots != null && customerLineSpots.Length > 0)
        {
            transform.position = customerLineSpots[currentSpotIndex]; // Move to Spot8
            isInLine = true; // Enable customer line logic
            Debug.Log("Joined the customer line at Spot8!");
        }
        else
        {
            Debug.LogError("Customer line spots are not assigned!");
        }
    }

    // Interaction Logic
    void Update()
    {
        // Only process interaction if this NPC is at Spot0 and not exiting
        if (isAtSpot0 && !isExiting && Input.GetKeyDown(KeyCode.E))
        {
            // Check if the player is standing on the Checkout object
            if (IsPlayerOnCheckout())
            {
                Debug.Log("Player pressed E near NPC at Spot0 and is on Checkout. NPC will follow ExitPath.");

                // Increase reputation by the specified amount
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
        // Check if the player is colliding with the Checkout object
        if (checkout != null)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(checkout.transform.position, 0.1f);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Player")) // Ensure the player has the "Player" tag
                {
                    return true; // Player is on the Checkout object
                }
            }
        }
        return false; // Player is not on the Checkout object
    }

    // Exit Path Logic
    private void StartExitPath()
    {
        // Find the ExitPath object
        GameObject exitPath = GameObject.Find("ExitPath");
        if (exitPath == null)
        {
            Debug.LogError("ExitPath not found! Make sure it exists in the scene.");
            return;
        }

        // Store ExitPath waypoints as Vector2
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

        // Start following the ExitPath
        isExiting = true;
        waypointIndex = 0; // Reset waypoint index
        Debug.Log("Started following ExitPath.");
    }

    private void FollowExitPath()
    {
        if (waypointIndex < exitPathWaypoints.Length)
        {
            Vector2 target = exitPathWaypoints[waypointIndex];
            Vector2 direction = (target - (Vector2)transform.position).normalized;

            // Move the NPC
            myRigidBody.linearVelocity = direction * speed;

            anim.SetBool("isWalking", true);
            anim.SetFloat("MoveX", direction.x);
            anim.SetFloat("MoveY", direction.y);

            // Check if NPC reached the waypoint
            if (Vector2.Distance(transform.position, target) < waypointThreshold)
            {
                Debug.Log($"Reached ExitPath Waypoint {waypointIndex}");

                // Move to the next waypoint
                waypointIndex++;

                // Check if the NPC reached the last waypoint
                if (waypointIndex == exitPathWaypoints.Length)
                {
                    Debug.Log("Reached the end of ExitPath. NPC will disappear.");
                    Destroy(gameObject); // Destroy the NPC
                }
            }
        }
    }

    // Walk to Spot8 Logic
    private void StartWalkingToSpot8()
    {
        isInLine = true; // Enable customer line logic
        currentSpotIndex = 8; // Start at Spot8
        Debug.Log("Walking to Spot8...");
    }
}