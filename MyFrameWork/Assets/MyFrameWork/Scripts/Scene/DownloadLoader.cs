using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadLoader : BaseSceneLoader<DownloadLoader>
{
    protected override async UniTask InitScene()
    {
        await UniTask.CompletedTask;
    }
}
