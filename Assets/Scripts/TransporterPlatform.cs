using UnityEngine;

public class TransporterPlatform : MonoBehaviour
{
    public Transform pointA; // Start position
    public Transform pointB; // Target position
    public float moveDuration = 2f; // Time it takes to move
    public AnimationCurve easeCurve; // Optional, not used here

    private Vector3 startPos;
    private Vector3 endPos;
    private float moveTimer = 0f;
    private bool playerOn = false;
    private bool movingToTarget = false;
    private bool isMoving = false;
    private float t;

    void Start()
    {
        startPos = pointA.position;
        endPos = pointB.position;
        transform.position = startPos;
    }

    void Update()
    {
        if (isMoving)
        {
            moveTimer += Time.deltaTime;

            // Calculate progress
            t = Mathf.Clamp01(moveTimer / moveDuration);

            // Use custom easing function
            float easedT = EaseInOutQuad(t);

            Vector3 target = movingToTarget ? endPos : startPos;
            Vector3 origin = movingToTarget ? startPos : endPos;

            transform.position = Vector3.Lerp(origin, target, easedT);

            if (t >= 1f)
            {
                isMoving = false;
                moveTimer = 0f;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
            playerOn = true;
            StartMoving(toTarget: true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
            playerOn = false;
            StartMoving(toTarget: false);
        }
    }

    void StartMoving(bool toTarget)
    {
        movingToTarget = toTarget;
        isMoving = true;
        moveTimer = 0f;
    }

    // 💡 Easing function added here
    float EaseInOutQuad(float t)
    {
        return t < 0.5f
            ? 2f * t * t
            : -1f + (4f - 2f * t) * t;
    }
}
