using System;
using UnityEngine;

/// <summary>
/// Load 데이터
/// </summary>
[Serializable]
public class LoadData
{
    [field: SerializeField] public string path { get; private set; }
    [field: SerializeField] public UnityEngine.Object setObject { get; private set; }
}