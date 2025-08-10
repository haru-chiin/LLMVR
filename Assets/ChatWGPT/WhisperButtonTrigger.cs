using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WhisperButtonTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Samples.Whisper.Whisper whisper;

    public void OnPointerDown(PointerEventData eventData)
    {
        whisper.StartRecordingExtern();
        whisper.recordButton.interactable = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        whisper.StopRecordingExtern();
        whisper.recordButton.interactable = true;
    }
}

