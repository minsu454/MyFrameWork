using Common.SceneEx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadUI : BaseSceneUI
{
    private void Awake()
    {
        Managers.Init();
    }

    public void GoTitle()
    {
        SceneManagerEx.LoadingAndNextScene(SceneType.Title);
    }

    public void GoInGame()
    {
        SceneManagerEx.LoadingAndNextScene(SceneType.InGame);
    }
}
