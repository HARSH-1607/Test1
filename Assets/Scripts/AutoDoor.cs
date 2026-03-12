using UnityEngine;

public class AutoDoor : MonoBehaviour
{
    public float openAngle = 90f;
    public float smoothSpeed = 5f;
    
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isOpen = false;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = transform.rotation * Quaternion.Euler(0, openAngle, 0);
    }

    void Update()
    {
        // Smoothly swing the door open or closed
        transform.rotation = Quaternion.Lerp(transform.rotation, isOpen ? openRotation : closedRotation, Time.deltaTime * smoothSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        // Open if the AI (NavMeshAgent) or Player walks into the trigger zone
        if (other.GetComponent<UnityEngine.AI.NavMeshAgent>() != null || other.CompareTag("Player"))
        {
            isOpen = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<UnityEngine.AI.NavMeshAgent>() != null || other.CompareTag("Player"))
        {
            isOpen = false;
        }
    }
}