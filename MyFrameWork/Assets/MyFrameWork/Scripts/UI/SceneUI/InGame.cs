using Common.SceneEx;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGame : BaseSceneUI
{
    public void Btn()
    {
        SceneManagerEx.LoadingAndNextScene(SceneType.Title);
    }
}
