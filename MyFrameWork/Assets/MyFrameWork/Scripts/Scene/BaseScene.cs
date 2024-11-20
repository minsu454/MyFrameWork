using Common.Assets;
using Common.Path;
using Common.Pool;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public abstract class BaseScene<T> : MonoBehaviour, IAddressable, ISceneDynamicCreatable where T : BaseScene<T>
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

    public async UniTask Init(string sceneName)
    {
        if (instance != null)
        {
            Debug.LogError($"Instance has not been initialized : {typeof(T).Name}");
            return;
        }

        instance = this as T;

        await InitScene();
        await Pooling(sceneName);
    }

    public async UniTask Pooling(string sceneName)
    {
        ObjectPoolSO objectPoolSO = await AddressableAssets.LoadDataAsync<ObjectPoolSO>(AddressablePath.ObjectPoolSOPath(sceneName));

        if (objectPoolSO == null)
        {
            Debug.LogWarning($"Addressable is Not Found ObjectPoolSO : {sceneName}");
            return;
        }

        foreach (var item in objectPoolSO.poolDataList)
        {
            await ObjectPoolContainer.Instance.CreateObjectPool(sceneName, item.Name, item.Count);
        }
    }

    /// <summary>
    /// 씬 동적 생성 해줄 오브젝트 몰빵하는 함수
    /// </summary>
    public abstract UniTask InitScene();

    protected virtual void OnDestroy()
    {
        instance = null;
        ReleaseEvent?.Invoke(gameObject);
    }


}
