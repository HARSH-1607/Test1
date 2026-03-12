using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class WindowsTTS : MonoBehaviour
{
    [Header("Character Audio Reference")]
    public AudioSource characterAudioSource;

    public void Speak(string text)
    {
        StartCoroutine(GenerateAndPlayAudio(text));
    }

    private IEnumerator GenerateAndPlayAudio(string text)
    {
        // Clean text to avoid breaking PowerShell commands
        string safeText = text.Replace("'", "");
        string filePath = Path.Combine(Application.temporaryCachePath, "tts_audio.wav").Replace("\\", "/");

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        // PowerShell command updated to force a Female voice
        string command = $"Add-Type -AssemblyName System.Speech; " +
                         $"$speak = New-Object System.Speech.Synthesis.SpeechSynthesizer; " +
                         $"$speak.SelectVoiceByHints([System.Speech.Synthesis.VoiceGender]::Female); " + // <--- THIS IS THE NEW LINE
                         $"$speak.SetOutputToWaveFile('{filePath}'); " +
                         $"$speak.Speak('{safeText}'); " +
                         $"$speak.Dispose()";

        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.FileName = "powershell.exe";
        process.StartInfo.Arguments = $"-Command \"{command}\"";
        process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        process.StartInfo.CreateNoWindow = true;
        process.Start();

        while (!process.HasExited)
        {
            yield return null;
        }

        string fileUrl = "file:///" + filePath;
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(fileUrl, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                characterAudioSource.clip = clip;
                characterAudioSource.Play(); 
            }
            else
            {
                Debug.LogError("Failed to load TTS audio: " + www.error);
            }
        }
    }
}