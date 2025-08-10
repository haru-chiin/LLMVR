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
    private string apiKey = "isi dengan api anda";

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
        request.Model = "isi dengan model anda / model gpt yang sudah ada";

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
