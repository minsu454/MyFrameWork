using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO", menuName = "ScriptableObject/AddressableLoader", order = 0)]
public class ResourcesLoaderSO : ScriptableObject
{
    public List<LoadData> loadDataList;
}

