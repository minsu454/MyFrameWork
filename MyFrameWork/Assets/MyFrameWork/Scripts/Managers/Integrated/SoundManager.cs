using Common.Assets;
using Common.Path;
using Common.Pool;
using Common.SceneEx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using Scene = UnityEngine.SceneManagement.Scene;

public class SoundManager : MonoBehaviour, IInit
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

        SceneLoader.Add(LoadPriorityType.Sound, OnSceneLoaded);
    }
    
    /// <summary>
    /// 씬 로드시 bgm깔아주는 이벤트 함수
    /// </summary>
    private async UniTask OnSceneLoaded(Scene scene)
    {
        AudioClip clip = await AddressableAssets.LoadDataAsync<AudioClip>(AddressablePath.BGMPath(scene.name));

        bgmSource.clip = clip;
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
    public void SFX3DPlay(AudioClip clip)
    {
        SoundPlayer soundPlayer = soundPool.GetObject();
        soundPlayer.SetDelay(clip.length);
        soundPlayer.SetSound3D();
        soundPlayer.gameObject.SetActive(true);

        soundPlayer.Play(clip);
    }

    public void SetVolume(SoundType type, float volume)
    {
        audioMixer.SetFloat(type.ToString(), Mathf.Log10(volume) * 20);
    }
}
