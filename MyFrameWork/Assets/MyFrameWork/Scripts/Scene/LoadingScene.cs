using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScene : BaseScene<InGameScene>
{
    protected override async UniTask InitScene()
    {
        await UniTask.CompletedTask;
    }
}
