using UnityEngine;

public class NPCPathfinding : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform[] waypoints; // Assign in Inspector
    private int currentWaypoint = 0;
    private Rigidbody2D rb;
    private Vector2 moveDir;
    private bool isStopped = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!isStopped && waypoints.Length > 0)
        {
            Vector2 direction = (waypoints[currentWaypoint].position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
            
            if (Vector2.Distance(transform.position, waypoints[currentWaypoint].position) < 0.1f)
            {
                currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            }
        }
    }

    public void MoveTo(Vector2 targetPosition)
    {
        moveDir = (targetPosition - rb.position).normalized;
        rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime));
    }

    public void StopMoving()
    {
        isStopped = true;
    if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    public void ResumeMoving()
    {
        isStopped = false;
    }
}
