using Common.Assets;
using Common.Path;
using Common.SceneEx;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class UIManager : MonoBehaviour, IInit
{
    private readonly List<BasePopupUI> showList = new List<BasePopupUI>();

    public void Init()
    {
        SceneLoader.Add(LoadPriorityType.UI, OnSceneLoaded);
    }

    private async UniTask OnSceneLoaded(Scene scene)
    {
        Clear();
        await CreateSceneUI(scene.name);
    }

    private async UniTask CreateSceneUI(string name)
    {
        GameObject prefab = await AddressableAssets.InstantiateAsync(AdressablePath.UIPath(name));

        if (!prefab.TryGetComponent(out BaseSceneUI sceneUI))
        {
            Debug.LogError($"GameObject Is Not BaseSceneUI Inheritance : {prefab}");
            return;
        }

        sceneUI.Init();
    }

    public async UniTask CreatePopup<T>(PopupOption option = null) where T : BasePopupUI
    {
        GameObject prefab = await AddressableAssets.InstantiateAsync(AdressablePath.UIPath(typeof(T).Name));

        if (!prefab.TryGetComponent(out T popupUI))
        {
            Debug.LogError($"GameObject Is Not BaseSceneUI Inheritance : {prefab}");
            return;
        }

        showList.Add(popupUI);
        popupUI.Init(option);
    }

    public void ClosePopup()
    {
        if (showList.Count == 0)
            return;

        BasePopupUI popup = showList[showList.Count - 1];
        showList.RemoveAt(showList.Count - 1);

        AddressableAssets.Release(popup.gameObject);
        Destroy(popup.gameObject);
    }

    public void ClosePopup<T>(T popup) where T : BasePopupUI
    {
        int idx = showList.Count - 1;

        while (idx == 0)
        {
            if (showList[idx] == popup)
                break;

            idx--;
        }

        if (idx == -1)
            Debug.LogError($"Is Not Found Popup : {typeof(T).Name}");

        showList.RemoveAt(idx);

        AddressableAssets.Release(popup.gameObject);
        Destroy(popup.gameObject);
    }

    private void Clear()
    {
        showList.Clear();
    }
}
