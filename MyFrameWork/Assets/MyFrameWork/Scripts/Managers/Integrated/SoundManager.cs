using Common.Assets;
using Common.EnumExtensions;
using Common.Objects;
using Common.Path;
using Common.Pool;
using Common.SceneEx;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Audio;
using Scene = UnityEngine.SceneManagement.Scene;

public sealed class SoundManager : MonoBehaviour, IInit
{
    private ObjectPool<SoundPlayer> soundPool;

    private AudioMixer audioMixer;
    private AudioSource bgmSource;

    private const int SoundPlayerCount = 10;

    public void Init()
    {
        audioMixer = Resources.Load<AudioMixer>("Sound/AudioMixer");

        CreateAudioSource(SoundType.BGM.ToString());
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
        AudioClip clip = ObjectManager.Return<AudioClip>(AddressablePath.BGMPath(sceneName));

        if (clip == null)
        {
            Debug.LogWarning($"Addressable is Not Found AudioClip : {sceneName}");
            return;
        }

        bgmSource.clip = clip;
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

            SetVolume(type, PlayerPrefs.GetFloat(name));
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
    /// 2D 플레이 함수(일반 플레이)
    /// </summary>
    public void SFX2DPlay(AudioClip clip)
    {
        SoundPlayer soundPlayer = soundPool.GetObject();
        soundPlayer.SetDelay(clip.length);
        soundPlayer.SetSound2D();
        soundPlayer.gameObject.SetActive(true);

        soundPlayer.Play(clip);
    }

    /// <summary>
    /// 3D 플레이 함수(원근감 사운드)
    /// </summary>
    public void SFX3DPlay(AudioClip clip, Transform playTr)
    {
        SoundPlayer soundPlayer = soundPool.GetObject();
        soundPlayer.SetDelay(clip.length);
        soundPlayer.SetSound3D(playTr);
        soundPlayer.gameObject.SetActive(true);

        soundPlayer.Play(clip);
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetVolume(SoundType type, float volume)
    {
        audioMixer.SetFloat(type.EnumToString(), Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(type.EnumToString(), volume);
    }
}
