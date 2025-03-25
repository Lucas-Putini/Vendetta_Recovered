using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Transform pointA, pointB; // Patrol points
    public float speed = 2f;
    private Transform targetPoint;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        targetPoint = pointA; // Start moving towards pointA
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Patrol();
    }

    private void Patrol()
    {
        // Move towards the target patrol point
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        // Switch direction when reaching a patrol point
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            targetPoint = (targetPoint == pointA) ? pointB : pointA;
            spriteRenderer.flipX = !spriteRenderer.flipX; // Flip sprite for correct facing direction
        }
    }
}

