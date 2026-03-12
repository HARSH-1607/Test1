using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HeadIK : MonoBehaviour
{
    [Header("Targeting")]
    public Transform target; // This will be your Main Camera
    
    [Header("Settings")]
    public bool isTracking = true;
    [Range(0f, 1f)] public float maxLookWeight = 1.0f;
    public float smoothSpeed = 3.0f; // How fast the head turns

    private Animator animator;
    private float currentWeight = 0.0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        
        // Automatically find the Main Camera if you forget to assign it!
        if (target == null && Camera.main != null)
        {
            target = Camera.main.transform;
        }
    }

    // This is a special Unity function that runs after standard animations to adjust bones
    void OnAnimatorIK(int layerIndex)
    {
        if (animator != null)
        {
            // Smoothly calculate if the head should be turning or returning to center
            float targetWeight = isTracking ? maxLookWeight : 0.0f;
            currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * smoothSpeed);

            if (target != null)
            {
                // SetLookAtWeight parameters: (overall weight, body weight, head weight, eyes weight, clamp weight)
                animator.SetLookAtWeight(currentWeight, 0.2f, 0.8f, 1.0f, 0.5f);
                
                // Tell the head exactly where to point
                animator.SetLookAtPosition(target.position);
            }
        }
    }
}