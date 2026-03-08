using UnityEngine;

public class EnvironmentSensor : MonoBehaviour
{
    [Header("Current Status")]
    // The default context. I've set this to fit a dungeon exploration theme!
    public string currentSurroundings = "standing in a dark, quiet dungeon room"; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            // Updates the AI's "vision" when it walks near an object
            currentSurroundings = "standing right next to a " + other.gameObject.name;
            Debug.Log("AI Sensor Update: " + currentSurroundings);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            // Reverts to the default state when walking away
            currentSurroundings = "standing in a dark, quiet dungeon room";
            Debug.Log("AI Sensor Update: " + currentSurroundings);
        }
    }
}