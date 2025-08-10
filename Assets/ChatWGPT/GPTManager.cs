using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using Samples.Whisper;

public class GPTManager : MonoBehaviour
{
    private OpenAIApi openAI;
    private List<ChatMessage> messages = new List<ChatMessage>();
    [SerializeField] private Whisper whisper;
    [SerializeField] private OpenAITextToSpeech toSpeech;
    private string apiKey = "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";

    private void Awake() {
        openAI = new OpenAIApi(apiKey);
    }
    public async void AskChatGPT(string newText)
    {
        ChatMessage newMessage = new ChatMessage();
        newMessage.Content = newText;
        newMessage.Role = "user";

        messages.Add(newMessage);
        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = new List<ChatMessage>
            {
             new ChatMessage { Role = "system", Content = "Jawablah dengan singkat, tidak lebih dari 500 karakter." },
             newMessage
            };
        request.Model = "ft:gpt-4o-mini-2024-07-18:galih-wasis-wicaksono::BbPIxjOv";

        var response = await openAI.CreateChatCompletion(request);

        if(response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            messages.Add(chatResponse);

            Debug.Log(chatResponse.Content);
            toSpeech.GenerateSpeech(chatResponse.Content);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
