using Common.EnumExtensions;
using Common.Objects;
using Common.Path;
using Common.Pool;
using Common.SceneEx;
using System;
using UnityEngine;
using UnityEngine.Audio;

public sealed class SoundManager : MonoBehaviour, IInit
{
    private ObjectPool<SoundPlayer> soundPool;

    private AudioMixer audioMixer;
    private AudioSource bgmSource;

    private const int SoundPlayerCount = 20;

    private AudioClip curSceneClip;
    private float curSceneVolume;

    public void Init()
    {
        audioMixer = Resources.Load<AudioMixer>("Sound/AudioMixer");

        CreateAudioSource(SoundType.BGM.EnumToString());
        CreateSoundPool();

        SceneJobLoader.Add(LoadPriorityType.Sound, OnSceneLoaded);
    }

    public void OnStart()
    {
        InitPlayerPrefsVolume();
    }

    /// <summary>
    /// 씬 로드시 bgm깔아주는 이벤트 함수
    /// </summary>
    private void OnSceneLoaded(string sceneName)
    {
        bgmSource.Stop();
    }

    /// <summary>
    /// 저장된 사운드 크기로 초기화 함수
    /// </summary>
    private void InitPlayerPrefsVolume()
    {
        foreach (SoundType type in Enum.GetValues(typeof(SoundType)))
        {
            string name = type.EnumToString();
            if (!GetAudioMixerGroup(name, out var group))
            {
                Debug.LogError($"Is Not Found Group : {name}");
                return;
            }

            SetVolume(type, PlayerPrefs.GetFloat(name, 1));
        }
    }

    /// <summary>
    /// BGMSource 제작 함수
    /// </summary>
    private void CreateAudioSource(string GroupName)
    {
        GameObject bgmGo = new GameObject(GroupName);
        bgmGo.transform.SetParent(transform);

        bgmSource = bgmGo.AddComponent<AudioSource>();

        if (!GetAudioMixerGroup(GroupName, out var bgmGroup))
        {
            Debug.LogError($"Is Not Found AudioMixerGroup : {GroupName}");
        }

        bgmSource.outputAudioMixerGroup = bgmGroup;

        bgmSource.playOnAwake = false;
        bgmSource.loop = true;

    }

    /// <summary>
    /// SoundPool 제작 함수
    /// </summary>
    private void CreateSoundPool()
    {
        GameObject prefab = Resources.Load<GameObject>("Sound/SoundPlayer");
        soundPool = new ObjectPool<SoundPlayer>(prefab.name, prefab, transform, SoundPlayerCount);
    }

    /// <summary>
    /// AudioMixerGroup 반환 함수
    /// </summary>
    private bool GetAudioMixerGroup(string name, out AudioMixerGroup audioMixerGroup)
    {
        var audioMixerGroupArr = audioMixer.FindMatchingGroups(name);

        if (audioMixerGroupArr.Length == 0)
        {
            audioMixerGroup = null;
            return false;
        }

        audioMixerGroup = audioMixerGroupArr[0];
        return true;
    }

    /// <summary>
    /// BGM 플레이 함수
    /// </summary>
    public void BGMPlay(AudioClip clip, float volume)
    {
        bgmSource.volume = volume;

        bgmSource.clip = clip;
        bgmSource.Play();
    }

    /// <summary>
    /// 처음 SceneBGM 플레이 함수
    /// </summary>
    public void FirstSceneBGMPlay(SceneType type, float volume)
    {
        bgmSource.volume = volume;
        curSceneVolume = volume;
        bgmSource.clip = ObjectManager.Return<AudioClip>(AddressablePath.BGMPath(type.EnumToString()));
        curSceneClip = bgmSource.clip;

        bgmSource.Play();
    }

    /// <summary>
    /// 다시 SceneBGM 플레이 함수
    /// </summary>
    public void SceneBGMRePlay()
    {
        bgmSource.volume = curSceneVolume;
        bgmSource.clip = curSceneClip;

        bgmSource.Play();
    }

    /// <summary>
    /// BGM 멈추는 함수
    /// </summary>
    public void BGMStop()
    {
        bgmSource.Stop();
    }

    /// <summary>
    /// 2D 플레이 함수(일반 플레이)
    /// </summary>
    public void SFX2DPlay(AudioClip clip)
    {
        SoundPlayer soundPlayer = soundPool.GetObject();
        soundPlayer.SetDelay(clip.length);
        soundPlayer.SetSound2D();
        soundPlayer.SetPitch(1);
        soundPlayer.SetVolume(1);
        soundPlayer.gameObject.SetActive(true);

        soundPlayer.Play(clip);
    }

    /// <summary>
    /// 2D 플레이 함수(일반 플레이)
    /// </summary>
    public void SFX2DPlay(AudioClip clip, float pitch)
    {
        SoundPlayer soundPlayer = soundPool.GetObject();
        soundPlayer.SetDelay(clip.length);
        soundPlayer.SetSound2D();
        soundPlayer.SetPitch(pitch);
        soundPlayer.SetVolume(1);
        soundPlayer.gameObject.SetActive(true);

        soundPlayer.Play(clip);
    }

    /// <summary>
    /// 3D 플레이 함수(원근감 사운드 : 일반)
    /// </summary>
    public void SFX3DPlay(AudioClip clip, Transform playTr)
    {
        SoundPlayer soundPlayer = soundPool.GetObject();
        soundPlayer.SetDelay(clip.length);
        soundPlayer.SetMaxDistance(15);
        soundPlayer.SetPitch(1);
        soundPlayer.SetVolume(1);
        soundPlayer.SetSound3D(playTr);
        soundPlayer.gameObject.SetActive(true);

        soundPlayer.Play(clip);
    }

    /// <summary>
    /// 3D 플레이 함수(원근감 사운드 : 볼륨 설정)
    /// </summary>
    public void SFX3DPlay(AudioClip clip, Transform playTr, float volume)
    {
        SoundPlayer soundPlayer = soundPool.GetObject();
        soundPlayer.SetDelay(clip.length);
        soundPlayer.SetMaxDistance(15);
        soundPlayer.SetPitch(1);
        soundPlayer.SetVolume(volume);
        soundPlayer.SetSound3D(playTr);
        soundPlayer.gameObject.SetActive(true);

        soundPlayer.Play(clip);
    }

    /// <summary>
    /// 3D 플레이 함수(원근감 사운드 : 피치 사용여부, 사운드 최대거리 설정 가능)
    /// </summary>
    public void SFX3DPlay(AudioClip clip, Transform playTr, bool usePitch, float maxDistance)
    {
        SoundPlayer soundPlayer = soundPool.GetObject();
        soundPlayer.SetDelay(clip.length);
        soundPlayer.SetMaxDistance(maxDistance);
        soundPlayer.SetSound3D(playTr);

        if (usePitch)
            soundPlayer.SetRandomPitch();

        soundPlayer.SetVolume(1);
        soundPlayer.gameObject.SetActive(true);

        soundPlayer.Play(clip);
    }

    /// <summary>
    /// 3D 플레이 함수(원근감 사운드 : 피치 사용여부, 사운드 최대거리 설정 가능)
    /// </summary>
    public void SFX3DPlay(AudioClip clip, Transform playTr, float pitch, float maxDistance)
    {
        SoundPlayer soundPlayer = soundPool.GetObject();
        soundPlayer.SetDelay(clip.length);
        soundPlayer.SetMaxDistance(maxDistance);
        soundPlayer.SetPitch(pitch);
        soundPlayer.SetVolume(1);
        soundPlayer.SetSound3D(playTr);

        soundPlayer.gameObject.SetActive(true);

        soundPlayer.Play(clip);
    }


    /// <summary>
    /// 불륨 설정해주는 함수
    /// </summary>
    public void SetVolume(SoundType type, float volume)
    {
        audioMixer.SetFloat(type.EnumToString(), Mathf.Log10(volume) * 20);
    }
}