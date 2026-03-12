using UnityEngine;

public class CharacterDoorRaycast : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float rayDistance = 3.0f;
    public LayerMask doorLayer;
    public Transform rayOrigin;

    private Animator lastDoorAnimator;

    void Update()
    {
        // 1. Safety Check
        if (rayOrigin == null)
        {
            Debug.LogError("Ray Origin is MISSING on " + gameObject.name + "! Please drag a bone (like the Head) into the slot.");
            return;
        }

        // 2. Define the Ray
        Vector3 direction = rayOrigin.forward;
        Ray ray = new Ray(rayOrigin.position, direction);
        RaycastHit hit;

        // ALWAYS draw the line in the Scene View
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.yellow);

        // 3. Shoot the Ray
        if (Physics.Raycast(ray, out hit, rayDistance, doorLayer))
        {
            Debug.Log("Ray HIT something: " + hit.collider.name);

            // Look for Animator in the object hit OR its parents (Apartment Kit doors have many layers)
            Animator doorAnim = hit.collider.GetComponentInParent<Animator>();

            if (doorAnim != null)
            {
                // Verify the parameter exists to avoid errors
                doorAnim.SetBool("isOpen", true);
                lastDoorAnimator = doorAnim;
                
                // Change line to Green when it actually finds a door
                Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green);
            }
            else
            {
                Debug.LogWarning("Hit " + hit.collider.name + " but found no Animator on it or its parents!");
            }
        }
        else
        {
            // 4. Auto-Close Logic
            if (lastDoorAnimator != null)
            {
                float dist = Vector3.Distance(transform.position, lastDoorAnimator.transform.position);
                if (dist > rayDistance + 1.5f)
                {
                    lastDoorAnimator.SetBool("isOpen", false);
                    lastDoorAnimator = null;
                }
            }
        }
    }
    // This draws the ray in the editor window 24/7, even when not playing!
    void OnDrawGizmos()
    {
        if (rayOrigin != null)
        {
            Gizmos.color = Color.magenta; // Bright magenta so it's impossible to miss
            Gizmos.DrawRay(rayOrigin.position, rayOrigin.forward * rayDistance);
        }
    }
}