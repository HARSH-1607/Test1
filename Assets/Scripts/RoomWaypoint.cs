using UnityEngine;

public class RoomWaypoint : MonoBehaviour
{
    public string roomName = "Kitchen";
    
    [Tooltip("The exact name of the Animator trigger/bool, e.g., isCooking or isLooking")]
    public string roomAnimation = "isCooking"; 
    
    [Tooltip("What we secretly tell Ollama to talk about when the AI arrives")]
    [TextArea]
    public string aiContextPrompt = "You just wandered into the kitchen. Say one short sentence out loud about being hungry or looking for food.";
}