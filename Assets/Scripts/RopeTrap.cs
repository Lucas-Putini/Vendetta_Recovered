using UnityEngine;
using System.Collections;

public class RopeTrap : MonoBehaviour
{
    public Transform topPoint;     // Where it starts (up position)
    public Transform bottomPoint;  // Where it drops to
    public float dropSpeed = 10f;
    public float riseSpeed = 3f;
    public float pauseTime = 0.5f; // Optional: pause time at bottom

    private bool goingDown = true;

    private void Start()
    {
        StartCoroutine(TrapLoop());
    }

    IEnumerator TrapLoop()
    {
        while (true)
        {
            Vector3 target = goingDown ? bottomPoint.position : topPoint.position;
            float speed = goingDown ? dropSpeed : riseSpeed;

            while (Vector3.Distance(transform.position, target) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                yield return null;
            }

            transform.position = target;

            if (goingDown)
                yield return new WaitForSeconds(pauseTime);

            goingDown = !goingDown;
        }
    }
}
