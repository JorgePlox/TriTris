using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;
    private AudioSource audioSource;

    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start() {
        audioSource = this.GetComponent<AudioSource>();

        Playfield.Instance.OnFlashAnimationStarted += Playfield_OnFlashAnimationStarted;
        Playfield.Instance.OnFlashAnimationEnded += Playfield_OnFlashAnimationEnded;

    }


    private void Playfield_OnFlashAnimationStarted(object sender, EventArgs e)
    {
        audioSource.Pause();
    }

    private void Playfield_OnFlashAnimationEnded(object sender, EventArgs e)
    {
        audioSource.UnPause();
    }


}
