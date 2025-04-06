using System.Collections;
using UnityEngine;


[System.Serializable]
public class SpikeUnit
{
    public Transform spike;
    public Transform retractPoint; // Where it goes when down
    public Transform extendPoint;  // Where it returns when up
}


public class SpikeTrapTrigger : MonoBehaviour
{
    [Header("Spike Units (each with its own move points)")]
    [SerializeField] private SpikeUnit[] spikeUnits;

    [Header("Timing")]
    [SerializeField] private float triggerDelay = 1f;
    [SerializeField] private float stayRetractedTime = 2f;
    [SerializeField] private float moveSpeed = 3f;

    private bool isPlayerOnTile = false;
    private bool isRunning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isRunning)
        {
            isPlayerOnTile = true;
            StartCoroutine(SpikeRoutine(other.GetComponent<Player>()));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnTile = false;
        }
    }

    private IEnumerator SpikeRoutine(Player player)
    {
        isRunning = true;

        yield return new WaitForSeconds(triggerDelay);

        // Move all spikes down
        yield return MoveAllSpikes(retract: true);

        yield return new WaitForSeconds(stayRetractedTime);

        // Move all spikes back up
        yield return MoveAllSpikes(retract: false);

        // Damage if still on
        if (isPlayerOnTile && player != null)
        {
            player.TakeDamage(25f);
            Debug.Log("Spike hit the player!");
        }

        isRunning = false;
    }

    private IEnumerator MoveAllSpikes(bool retract)
    {
        bool anyStillMoving;

        do
        {
            anyStillMoving = false;

            foreach (var unit in spikeUnits)
            {
                Vector3 target = retract ? unit.retractPoint.position : unit.extendPoint.position;

                if (Vector3.Distance(unit.spike.position, target) > 0.01f)
                {
                    unit.spike.position = Vector3.MoveTowards(unit.spike.position, target, moveSpeed * Time.deltaTime);
                    anyStillMoving = true;
                }
            }

            yield return null;
        }
        while (anyStillMoving);

        // Snap all into final positions
        foreach (var unit in spikeUnits)
        {
            Vector3 target = retract ? unit.retractPoint.position : unit.extendPoint.position;
            unit.spike.position = target;
        }
    }
}
