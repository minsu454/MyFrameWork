using Common.SceneEx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Managers : MonoBehaviour
{
    private static Managers instance;

    public static UIManager UI { get { return instance.uiManager; } }
    public static SoundManager Sound { get { return instance.soundManager; } }

    private UIManager uiManager;
    private SoundManager soundManager;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        GameObject go = new GameObject("Managers");
        instance = go.AddComponent<Managers>();

        DontDestroyOnLoad(go);

        instance.uiManager = CreateManager<UIManager>(go.transform);
        instance.soundManager = CreateManager<SoundManager>(go.transform);

        SceneJobLoader.Init();
    }

    /// <summary>
    /// 매니저 생성 함수
    /// </summary>
    private static T CreateManager<T>(Transform parent) where T : Component, IInit
    {
        GameObject go = new GameObject(typeof(T).Name);
        T t = go.AddComponent<T>();
        go.transform.parent = parent;

        t.Init();

        return t;
    }
}