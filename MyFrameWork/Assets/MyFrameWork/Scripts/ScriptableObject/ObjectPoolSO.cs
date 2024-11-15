using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoolSO", menuName = "ScriptableObject/ObjectPool")]
public class ObjectPoolSO : ScriptableObject
{
    public List<PoolData> poolDataList;
}

[Serializable]
public class PoolData
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public int Count { get; private set; }
}