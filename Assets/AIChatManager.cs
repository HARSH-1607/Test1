using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.AI; // NEEDED FOR NAVMESH
using TMPro;

public class AIChatManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField userInputField;
    public TextMeshProUGUI aiResponseText;
    public Button sendButton;

    [Header("System References")]
    public WindowsTTS tts;
    public EnvironmentSensor sensor;
    public Animator characterAnimator;
    public NavMeshAgent agent; // NEW: Drag your character here in the Inspector!

    [Header("Server Settings")]
    private string serverUrl = "http://localhost:8000/chat";

    [System.Serializable]
    public class ChatRequest
    {
        public string user_input;
        public string environment_context;
    }

    [System.Serializable]
    public class ChatResponse
    {
        public string response;
    }

    void Start()
    {
        sendButton.onClick.AddListener(OnSendClicked);
        // Ensure character starts sitting
        if (characterAnimator != null) characterAnimator.SetBool("isSitting", true);
    }

    void Update()
    {
        // Handle talking animation sync
        if (tts != null && tts.characterAudioSource != null && characterAnimator != null)
        {
            bool isAudioPlaying = tts.characterAudioSource.isPlaying;
            if (isAudioPlaying && !characterAnimator.GetBool("isTalking"))
            {
                characterAnimator.SetBool("isTalking", true);
            }
            else if (!isAudioPlaying && characterAnimator.GetBool("isTalking"))
            {
                characterAnimator.SetBool("isTalking", false);
            }
        }
    }

    public void OnSendClicked()
    {
        string message = userInputField.text;
        if (!string.IsNullOrEmpty(message))
        {
            StartCoroutine(SendMessageToAI(message));
            userInputField.text = "";
            aiResponseText.text = "AI is thinking...";
        }
    }

    IEnumerator SendMessageToAI(string userMsg)
    {
        ChatRequest requestData = new ChatRequest();
        requestData.user_input = userMsg;
        requestData.environment_context = sensor != null ? sensor.currentSurroundings : "sitting idle";

        string jsonPayload = JsonUtility.ToJson(requestData);

        using (UnityWebRequest request = new UnityWebRequest(serverUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                ChatResponse responseData = JsonUtility.FromJson<ChatResponse>(request.downloadHandler.text);
                string rawText = responseData.response;
                string spokenText = rawText;

                // 1. Check if the AI decided to move!
                if (rawText.Contains("[MOVE:"))
                {
                    int startIndex = rawText.IndexOf("[MOVE:") + 6;
                    int endIndex = rawText.IndexOf("]", startIndex);
                    
                    if (endIndex > startIndex)
                    {
                        string targetObjectName = rawText.Substring(startIndex, endIndex - startIndex);
                        // Remove the tag so the AI doesn't say "[MOVE:Computer]" out loud
                        spokenText = rawText.Substring(endIndex + 1).Trim(); 
                        
                        // Start the movement sequence
                        StartCoroutine(HandleMovement(targetObjectName));
                    }
                }

                aiResponseText.text = spokenText;

                if (tts != null)
                {
                    tts.Speak(spokenText);
                }
            }
            else
            {
                aiResponseText.text = "Connection Error.";
                Debug.LogError("Server Error: " + request.downloadHandler.text);
            }
        }
    }

    // NEW: The Sequence to Stand up, Walk, and Stop
    IEnumerator HandleMovement(string targetName)
    {
        GameObject target = GameObject.Find(targetName);
        
        if (target != null && agent != null)
        {
            // 1. If currently sitting, trigger the stand up animation
            if (characterAnimator.GetBool("isSitting"))
            {
                characterAnimator.SetBool("isSitting", false);
                // Wait for the Sit-To-Stand animation to finish (adjust the 2.0f based on your fbx length)
                yield return new WaitForSeconds(2.0f); 
            }

            // 2. Start walking
            characterAnimator.SetBool("isWalking", true);
            agent.SetDestination(target.transform.position);

            // 3. Wait until the character arrives at the destination
            // (We check if path is pending or distance is greater than the stopping distance)
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                // Ensure they look where they are going
                if (agent.velocity.sqrMagnitude > 0.1f)
                {
                    agent.transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);
                }
                yield return null;
            }

            // 4. Arrived! Stop walking.
            characterAnimator.SetBool("isWalking", false);
        }
        else
        {
            Debug.LogWarning("AI tried to move to " + targetName + ", but that object doesn't exist in the scene!");
        }
    }
}