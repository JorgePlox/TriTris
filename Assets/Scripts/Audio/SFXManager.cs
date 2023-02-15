using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;
    private AudioSource audioSource;
    [SerializeField] AudioClip clearLine; 

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
    }

    private void Playfield_OnFlashAnimationStarted(object sender, EventArgs e)
    {
        ClearLine();
    }

    private void ClearLine()
    {
        audioSource.clip = clearLine;
        audioSource.Play();
    }
}
