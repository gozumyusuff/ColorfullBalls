using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;
    private bool isMuted = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (!audioSource.isPlaying && !isMuted)
        {
            audioSource.Play();
        }
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        audioSource.mute = !audioSource.mute;
    }
}
