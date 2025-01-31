using Common.Timer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour, IObjectPoolable<SoundPlayer>
{
    [SerializeField] private AudioSource audioSource;       //오디오 소스 변수
    private float delay;                                    //클립 딜레이 저장 변수

    public event Action<SoundPlayer> ReturnEvent;

    private void OnEnable()
    {
        StartCoroutine(CoTimer.Start(delay, () => ReturnEvent.Invoke(this)));
    }

    /// <summary>
    /// 클립에 딜레이 설정 함수
    /// </summary>
    public void SetDelay(float delay)
    {
        this.delay = delay;
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void SetMaxDistance(float maxDistance)
    {
        audioSource.maxDistance = maxDistance;
    }

    public void SetRandomPitch()
    {
        float random = UnityEngine.Random.Range(-0.1f, 0.1f);
        audioSource.pitch = 1 + random;
    }

    public void SetPitch(float amount)
    {
        audioSource.pitch = amount;
    }

    /// <summary>
    /// 2D 사운드 설정 함수
    /// </summary>
    public void SetSound2D()
    {
        audioSource.spatialBlend = 0;
    }

    /// <summary>
    /// 3D 사운드 설정 함수
    /// </summary>
    public void SetSound3D(Transform playTr)
    {
        transform.position = playTr.position;
        audioSource.spatialBlend = 1;
    }

    /// <summary>
    /// 오디오 클립 플레이 함수
    /// </summary>
    public void Play(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
