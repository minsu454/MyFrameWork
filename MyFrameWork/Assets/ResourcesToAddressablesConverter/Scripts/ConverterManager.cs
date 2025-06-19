using UnityEngine;

namespace Common.ResourcesToAddressablesConverter
{
    [CreateAssetMenu(fileName = "new Converter Manager", menuName = "ScriptableObject/Resources To Addressables Converter/Converter Manager", order = 0)]
    public class ConverterManager : ScriptableObject
    {
        public bool UseResources = false;
        public bool UseAddressables = true;
    }
}
