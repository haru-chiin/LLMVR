using OpenAI;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.Whisper
{
    public class Whisper : MonoBehaviour
    {
        [SerializeField] public Button recordButton;
        [SerializeField] private Image progressBar;
        [SerializeField] private Text message;
        [SerializeField] private Dropdown dropdown;
        [SerializeField] private GPTManager gpt;

        private readonly string fileName = "output.wav";
        private readonly int duration = 30;

        private AudioClip clip;
        private bool isRecording;
        private float time;
        private OpenAIApi openai;
        private string apiKey = "isi dengan API yang anda punya";

        private void Awake()
        {
            openai = new OpenAIApi(apiKey);
        }

        private void Start()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            // WebGL mic init if needed
#else
            foreach (var device in Microphone.devices)
            {
                dropdown.options.Add(new Dropdown.OptionData(device));
            }

            var index = PlayerPrefs.GetInt("user-mic-device-index", 0);
            dropdown.SetValueWithoutNotify(index);

#endif
        }

        public void StartRecordingExtern()
        {
            isRecording = true;
            time = 0;
            recordButton.interactable = false;

            var index = PlayerPrefs.GetInt("user-mic-device-index", 0);

#if !UNITY_WEBGL
            string deviceName = dropdown.options[index].text;
            clip = Microphone.Start(deviceName, false, duration, 44100);
            Debug.Log("Started recording with device: " + deviceName);
#endif
        }

        public void StopRecordingExtern()
        {
            isRecording = false;
            EndRecording();
        }

        private async void EndRecording()
        {
            message.text = "Transcribing...";

#if !UNITY_WEBGL
            while (Microphone.IsRecording(null))
            {
                await Task.Delay(100);
            }
            Microphone.End(null);
#endif

            if (clip == null || clip.samples <= 0)
            {
                Debug.LogError("Clip is empty.");
                message.text = "Gagal merekam suara.";
                recordButton.interactable = true;
                return;
            }

            byte[] data = SaveWav.Save(fileName, clip);
            Debug.Log("Data saved: " + (data != null ? data.Length.ToString() : "null"));

            if (data == null || data.Length < 1000)
            {
                message.text = "File audio terlalu kecil atau kosong.";
                recordButton.interactable = true;
                return;
            }

            try
            {
                var req = new CreateAudioTranscriptionsRequest
                {
                    FileData = new FileData() { Data = data, Name = "audio.wav" },
                    Model = "whisper-1",
                    Language = "id"
                };

                var res = await openai.CreateAudioTranscription(req);

                if (string.IsNullOrWhiteSpace(res.Text) || res.Text == "." || res.Text.ToLower().Contains("hello"))
                {
                    message.text = "Suara tidak dikenali. Silakan ulangi.";
                    Debug.LogWarning("Transkripsi mencurigakan: " + res.Text);
                }
                else
                {
                    message.text = res.Text;
                    gpt.AskChatGPT(res.Text);
                    Debug.Log("Transkripsi akhir: " + res.Text);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Transkripsi GAGAL: " + ex.Message);
                message.text = "Gagal mentranskripsi suara.";
            }

            progressBar.fillAmount = 0;
            recordButton.interactable = true;
        }

        private void Update()
        {
            if (isRecording)
            {
                time += Time.deltaTime;
                progressBar.fillAmount = time / duration;

                if (time >= duration)
                {
                    isRecording = false;
                    EndRecording();
                }
            }
        }
    }
}
