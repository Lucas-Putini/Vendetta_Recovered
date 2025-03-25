using UnityEngine;
using System.Collections;

public class TrapDoor : MonoBehaviour
{
    public float openAngle = -90f;         // Negative to rotate downward
    public float speed = 200f;
    public float holdTime = 2f;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    private void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(0, 0, openAngle) * closedRotation;

        StartCoroutine(TrapLoop());
    }

    IEnumerator TrapLoop()
    {
        while (true)
        {
            Quaternion targetRotation = isOpen ? closedRotation : openRotation;

            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, speed * Time.deltaTime);
                yield return null;
            }

            transform.rotation = targetRotation;
            yield return new WaitForSeconds(holdTime);

            isOpen = !isOpen;
        }
    }
}
