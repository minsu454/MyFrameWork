using Common.SceneEx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Managers : MonoBehaviour
{
    private static Managers instance;

    public static UIManager UI { get { return instance.uiManager; } }

    private UIManager uiManager;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        GameObject go = new GameObject("Managers");
        instance = go.AddComponent<Managers>();

        DontDestroyOnLoad(go);

        instance.uiManager = CreateManager<UIManager>(go.transform);

        SceneLoader.Init();
    }

    private static T CreateManager<T>(Transform parent) where T : Component, IInit
    {
        GameObject go = new GameObject(typeof(T).Name);
        T t = go.AddComponent<T>();
        go.transform.parent = parent;

        t.Init();

        return t;
    }
}