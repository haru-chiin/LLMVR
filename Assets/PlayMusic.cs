using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    // Start is called before the first frame update
    AudioSource audio;
    void Start()
    {
        audio = GetComponent<AudioSource>();
        Invoke("PlaySound", 1.0f);
    }

    void PlaySound()
    {
        audio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
