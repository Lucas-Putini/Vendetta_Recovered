using UnityEngine;

public class HorizontalPlatform : MonoBehaviour
{
    public Transform pointA; // Left position
    public Transform pointB; // Right position
    public float speed = 2f; // Speed of movement
    public float waitTime = 1f; // Time to wait at each stop

    private Vector2 targetPosition;
    private bool movingRight = true;
    private bool isWaiting = false;

    void Start()
    {
        targetPosition = pointB.position; // Start moving right
    }

    void Update()
    {
        if (!isWaiting)
        {
            MovePlatform();
        }
    }

    void MovePlatform()
    {
        // Move towards the target position
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Check if the platform reached the target
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            StartCoroutine(WaitAndSwitchDirection());
        }
    }

    System.Collections.IEnumerator WaitAndSwitchDirection()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);

        // Switch direction
        movingRight = !movingRight;
        targetPosition = movingRight ? pointB.position : pointA.position;

        isWaiting = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Make player move with the platform
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Detach player from platform
            collision.transform.SetParent(null);
        }
    }
}
