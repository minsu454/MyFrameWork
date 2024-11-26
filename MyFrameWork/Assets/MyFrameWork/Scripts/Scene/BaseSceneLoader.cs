using Common.Assets;
using Common.Objects;
using Common.Path;
using Common.Pool;
using Common.SceneEx;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public abstract class BaseSceneLoader<T> : MonoBehaviour, IAddressable, IInit where T : BaseSceneLoader<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    public event Action<GameObject> ReleaseEvent;

    public void Init()
    {
        if (instance != null)
        {
            Debug.LogError($"Instance has not been initialized : {typeof(T).Name}");
            return;
        }

        instance = this as T;

        InitScene();
        Pooling();
    }

    /// <summary>
    /// 지금 씬에서 사용할 오브젝트 풀링해주는 함수
    /// </summary>
    private void Pooling()
    {
        string sceneName = SceneManagerEx.NextScene;

        ObjectPoolSO objectPoolSO = ObjectManager.Return<ObjectPoolSO>(AddressablePath.ObjectPoolSOPath(sceneName));

        if (objectPoolSO == null)
        {
            Debug.LogWarning($"Addressable is Not Found ObjectPoolSO : {sceneName}");
            return;
        }

        foreach (var item in objectPoolSO.poolDataList)
        {
            ObjectPoolContainer.Instance.CreateObjectPool(sceneName, item.Name, item.Count);
        }
    }

    /// <summary>
    /// 씬 동적 생성 해줄 오브젝트 몰빵하는 함수
    /// </summary>
    protected abstract void InitScene();

    protected virtual void OnDestroy()
    {
        instance = null;
        ReleaseEvent?.Invoke(gameObject);
    }


}
