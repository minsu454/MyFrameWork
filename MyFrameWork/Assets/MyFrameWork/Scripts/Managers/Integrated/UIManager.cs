using Common.Objects;
using Common.ReturnPath;
using Common.SceneEx;
using System.Collections.Generic;
using UnityEngine;

public sealed class UIManager : MonoBehaviour, IInit
{
    private readonly List<BasePopupUI> showList = new List<BasePopupUI>();  //지금 켜져있는 팝업들 모음 List

    public void Init()
    {
        SceneJobLoader.Add(LoadPriorityType.UI, OnSceneLoaded);
    }

    /// <summary>
    /// 씬 로드 시 호출 될 이벤트 함수
    /// </summary>
    private void OnSceneLoaded(string sceneName)
    {
        Clear();
        CreateSceneUI(sceneName);
    }

    /// <summary>
    /// 씬 로드 시 해당 Scene에 메인 UI 배치 함수
    /// </summary>
    private void CreateSceneUI(string name)
    {
        GameObject prefab = ObjectManager.Return<GameObject>(AddressablePath.UIPath(name));

        GameObject uiGo = Instantiate(prefab);

        if (!uiGo.TryGetComponent(out BaseSceneUI sceneUI))
        {
            Debug.LogError($"GameObject Is Not BaseSceneUI Inheritance : {uiGo}");
            return;
        }


        sceneUI.Init();
    }

    /// <summary>
    /// 팝업 제작 함수
    /// </summary>
    public void CreatePopup<T>(PopupOption option = null) where T : BasePopupUI
    {
        GameObject prefab = ObjectManager.Return<GameObject>(AddressablePath.UIPath(typeof(T).Name));

        GameObject uiGo = Instantiate(prefab);

        if (!uiGo.TryGetComponent(out T popupUI))
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

        Destroy(popup.gameObject);
    }

    private void Clear()
    {
        showList.Clear();
    }
}
