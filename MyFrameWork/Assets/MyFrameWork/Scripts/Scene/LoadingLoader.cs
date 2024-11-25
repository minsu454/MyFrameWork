using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingLoader : BaseSceneLoader<LoadingLoader>
{
    protected override async UniTask InitScene()
    {
        await UniTask.CompletedTask;
    }
}
