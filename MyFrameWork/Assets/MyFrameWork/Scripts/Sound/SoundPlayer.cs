using Common.Timer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour, IObjectPoolable<SoundPlayer>
{
    [SerializeField] private AudioSource audioSource;
    private float delay;

    public event Action<SoundPlayer> ReturnEvent;

    private void OnEnable()
    {
        StartCoroutine(CoTimer.Start(delay, () => ReturnEvent.Invoke(this)));
    }

    public void SetDelay(float delay, bool is3D = true)
    {
        this.delay = delay;
    }

    public void SetSound2D()
    {
        audioSource.spatialBlend = 0;
    }

    public void SetSound3D()
    {
        audioSource.spatialBlend = 1;
    }

    public void Play(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
