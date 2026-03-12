using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;
using Cinemachine;

public class AIChatManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField userInputField;
    public TextMeshProUGUI aiResponseText;
    public Button sendButton;

    [Header("System References")]
    public WindowsTTS tts;
    public Animator characterAnimator;
    public NavMeshAgent agent;

    [Header("Camera & Polish")]
    public CinemachineVirtualCamera vCam;
    private CinemachineBasicMultiChannelPerlin cameraNoise;
    private CinemachineTransposer cameraTransposer; // Controls the offset
    
    public float idleCameraZ = 1.0f; // Close up
    public float walkCameraZ = 2.0f; // Zoomed out
    public float cameraZoomSpeed = 3.0f; // How fast it zooms
    private Coroutine zoomCoroutine;

    [Header("AFK Settings")]
    public float afkTimeLimit = 20f;
    private float currentAfkTimer = 0f;
    public RoomWaypoint[] roomWaypoints;

    private string serverUrl = "http://localhost:8000/chat";

    [System.Serializable]
    public class ChatRequest { public string user_input; public string environment_context; }
    [System.Serializable]
    public class ChatResponse { public string response; }

    void Start()
    {
        sendButton.onClick.AddListener(OnSendClicked);
        StartCoroutine(InitialGreeting());

        if (vCam != null)
        {
            cameraNoise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (cameraNoise != null) cameraNoise.m_AmplitudeGain = 0f;
            
            // Get the component that controls the Z offset
            cameraTransposer = vCam.GetCinemachineComponent<CinemachineTransposer>();
            if (cameraTransposer != null) 
            {
                cameraTransposer.m_FollowOffset.z = idleCameraZ; // Start zoomed in
            }
        }
    }

    IEnumerator InitialGreeting()
    {
        yield return new WaitForSeconds(1.0f);
        characterAnimator.SetTrigger("isGreeting");
        string greetingText = "Hello there! Welcome to the apartment.";
        aiResponseText.text = greetingText;
        if (tts != null) tts.Speak(greetingText);
    }

    void Update()
    {
        if (tts != null && tts.characterAudioSource != null && characterAnimator != null)
        {
            bool isAudioPlaying = tts.characterAudioSource.isPlaying;
            if (isAudioPlaying && !characterAnimator.GetBool("isTalking"))
                characterAnimator.SetBool("isTalking", true);
            else if (!isAudioPlaying && characterAnimator.GetBool("isTalking"))
                characterAnimator.SetBool("isTalking", false);
        }

        if (Input.anyKey || userInputField.isFocused)
        {
            currentAfkTimer = 0f;
        }
        else
        {
            currentAfkTimer += Time.deltaTime;
            if (currentAfkTimer >= afkTimeLimit)
            {
                TriggerAFKAction();
            }
        }
    }

    public void OnSendClicked()
    {
        string message = userInputField.text;
        if (!string.IsNullOrEmpty(message))
        {
            StopActions(); 
            StartCoroutine(SendMessageToAI(message, "Normal conversation in the apartment."));
            userInputField.text = "";
            aiResponseText.text = "AI is thinking...";
        }
    }

    private void TriggerAFKAction()
    {
        currentAfkTimer = 0f; 
        StopActions(); 
        
        int randomAction = Random.Range(0, 3); 

        if (randomAction == 0)
        {
            characterAnimator.SetTrigger("isLookingAround");
            string text = "It's a bit quiet in here...";
            aiResponseText.text = text;
            tts.Speak(text);
        }
        else if (randomAction == 1)
        {
            characterAnimator.SetTrigger("isStretching");
            string text = "Ah, my back is stiff. Needed a good stretch.";
            aiResponseText.text = text;
            tts.Speak(text);
        }
        else if (randomAction == 2 && roomWaypoints.Length > 0)
        {
            int randomIndex = Random.Range(0, roomWaypoints.Length);
            RoomWaypoint dest = roomWaypoints[randomIndex];
            StartCoroutine(WanderSequence(dest));
        }
    }

    IEnumerator WanderSequence(RoomWaypoint waypoint)
    {
        characterAnimator.SetBool("isWalking", true);
        
        // 1. Zoom out smoothly when starting to walk!
        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        zoomCoroutine = StartCoroutine(SmoothCameraZoom(walkCameraZ));

        agent.SetDestination(waypoint.transform.position);

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            if (agent.velocity.sqrMagnitude > 0.1f)
                agent.transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);
            yield return null;
        }

        characterAnimator.SetBool("isWalking", false);
        
        // 2. Zoom back in smoothly when arrived!
        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        zoomCoroutine = StartCoroutine(SmoothCameraZoom(idleCameraZ));

        StartCoroutine(SendMessageToAI("Make a short comment about wandering into this room.", waypoint.aiContextPrompt));
    }

    // NEW: Handles the smooth math for zooming in and out
    IEnumerator SmoothCameraZoom(float targetZ)
    {
        if (cameraTransposer == null) yield break;

        while (Mathf.Abs(cameraTransposer.m_FollowOffset.z - targetZ) > 0.01f)
        {
            cameraTransposer.m_FollowOffset.z = Mathf.Lerp(cameraTransposer.m_FollowOffset.z, targetZ, Time.deltaTime * cameraZoomSpeed);
            yield return null;
        }
        
        cameraTransposer.m_FollowOffset.z = targetZ; // Snap to exact value at the end
    }

    IEnumerator SendMessageToAI(string userMsg, string context)
    {
        ChatRequest req = new ChatRequest { user_input = userMsg, environment_context = context };
        string json = JsonUtility.ToJson(req);

        using (UnityWebRequest request = new UnityWebRequest(serverUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                ChatResponse res = JsonUtility.FromJson<ChatResponse>(request.downloadHandler.text);
                if (res == null || string.IsNullOrEmpty(res.response)) yield break;

                string rawText = res.response;
                string spokenText = rawText;

                if (rawText.Contains("[DANCE]"))
                {
                    spokenText = rawText.Replace("[DANCE]", "").Trim();
                    characterAnimator.SetBool("isDancing", true);
                    if (cameraNoise != null) cameraNoise.m_AmplitudeGain = 1.2f; 
                }

                if (rawText.Contains("[SING]"))
                {
                    spokenText = rawText.Replace("[SING]", "").Trim();
                    characterAnimator.SetBool("isSinging", true);
                    if (cameraNoise != null) cameraNoise.m_AmplitudeGain = 0.5f; 
                }

                if (rawText.Contains("[MOVE:"))
                {
                    int startIndex = rawText.IndexOf("[MOVE:") + 6;
                    int endIndex = rawText.IndexOf("]", startIndex);
                    if (endIndex > startIndex)
                    {
                        string targetRoom = rawText.Substring(startIndex, endIndex - startIndex);
                        spokenText = rawText.Substring(endIndex + 1).Trim();
                        
                        foreach (RoomWaypoint wp in roomWaypoints)
                        {
                            if (wp.roomName.ToLower() == targetRoom.ToLower())
                            {
                                StartCoroutine(WanderSequence(wp));
                                break;
                            }
                        }
                    }
                }

                aiResponseText.text = spokenText;
                if (tts != null) tts.Speak(spokenText);
            }
        }
    }

    private void StopActions()
    {
        characterAnimator.SetBool("isDancing", false);
        characterAnimator.SetBool("isSinging", false);
        if (cameraNoise != null) cameraNoise.m_AmplitudeGain = 0f;
    }
}