using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using OpenAI;

public class OpenAITextToSpeech : MonoBehaviour
{
    public AudioSource audioSource;
    private OpenAIApi openAI = new OpenAIApi();
    private string apiKey = "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"; // Ganti dengan API key OpenAI Anda
    private string openAiUrl = "https://api.openai.com/v1/audio/speech"; // Ganti dengan URL endpoint text-to-speech dari OpenAI

    [System.Serializable]
    public class RequestData
    {
        public string model;
        public string voice;
        public string input;
    }

    public void GenerateSpeech(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        StartCoroutine(SendTextToOpenAI(text));
    }

    private IEnumerator SendTextToOpenAI(string inputText)
    {
        var requestData = new RequestData
        {
            model = "tts-1",  // Bisa diganti "tts-1-hd"
            voice = "alloy",
            input = inputText
        };

        string jsonData = JsonUtility.ToJson(requestData);
        byte[] postData = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(openAiUrl, AudioType.MPEG))
        {
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.downloadHandler = new DownloadHandlerAudioClip(openAiUrl, AudioType.MPEG);
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("TTS Request: " + jsonData);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);
                audioSource.clip = audioClip;
                audioSource.Play();
                Debug.Log("TTS success.");
            }
            else
            {
                Debug.LogError("TTS Error: " + request.error);
            }
        }
    }
}