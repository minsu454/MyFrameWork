using System.Collections.Generic;
using UnityEngine;

namespace Common.AssetResolver
{
    [CreateAssetMenu(fileName = "new Asset Manager", menuName = "ScriptableObject/Asset Resolver/Asset Manager", order = 0)]
    public class AssetManager : ScriptableObject
    {
        public bool UseResources = false;
        public bool UseAddressables = true;

        [SerializeField] public List<string> ResourcesPathList = new List<string>();
    }
}
