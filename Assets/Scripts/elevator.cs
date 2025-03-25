using UnityEngine;

public class ElevatorMovement : MonoBehaviour
{
    public Transform pointA; // Lower position
    public Transform pointB; // Upper position
    public float speed = 2f; // Speed of movement
    public float waitTime = 1f; // Time to wait at each stop

    private Vector3 targetPosition;
    private bool movingUp = true;
    private bool isWaiting = false;

    void Start()
    {
        targetPosition = pointB.position; // Start by moving up
    }

    void Update()
    {
        if (!isWaiting)
        {
            MoveElevator();
        }
    }

    void MoveElevator()
    {
        // Move towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Check if the elevator reached the target
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            StartCoroutine(WaitAndSwitchDirection());
        }
    }

    System.Collections.IEnumerator WaitAndSwitchDirection()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);

        // Switch direction
        movingUp = !movingUp;
        targetPosition = movingUp ? pointB.position : pointA.position;

        isWaiting = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Make player move with the elevator
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Detach player from elevator
            collision.transform.SetParent(null);
        }
    }
}
