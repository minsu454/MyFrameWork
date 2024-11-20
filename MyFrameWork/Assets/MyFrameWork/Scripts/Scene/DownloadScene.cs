using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadScene : BaseScene<InGameScene>
{
    public override async UniTask InitScene()
    {
        await UniTask.CompletedTask;
    }
}
