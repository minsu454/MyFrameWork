using Common.Assets;
using Cysharp.Threading.Tasks;
using System;
using UnityEditor.Build.Content;
using UnityEngine;

public class InGameLoader : BaseSceneLoader<InGameLoader>
{
    protected override async UniTask InitScene()
    {
        await UniTask.CompletedTask;
    }
}
