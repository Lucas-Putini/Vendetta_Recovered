using UnityEngine;

public class SawTrap : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;
    

    private Vector3 startPos;
    private Vector3 endPos;

    void Start()
    {
        startPos = pointA.position;
        endPos = pointB.position;
    }

    void Update()
    {
        MoveSaw();
        
    }

    void MoveSaw()
    {
        // Smooth back and forth motion using Mathf.PingPong
        float t = Mathf.PingPong(Time.time * moveSpeed, 1);
        transform.position = Vector3.Lerp(startPos, endPos, t);
    }
}



