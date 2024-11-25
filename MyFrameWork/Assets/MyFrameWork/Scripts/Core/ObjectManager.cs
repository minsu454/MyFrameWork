using Common.Assets;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Objects
{
    public static class ObjectManager
    {
        private static readonly Dictionary<string, Object> objectContainerDict = new Dictionary<string, Object>();

        public static async UniTask Add(string label)
        {
            var list = await AddressableAssets.LoadDataWithLabelAsync(label);

            foreach (var item in list)
            {
                objectContainerDict.Add(item.PrimaryKey, item.Data as Object);
            }
        }

        public static void Remove()
        {
            objectContainerDict.Clear();
        }

        public static T Return<T>(string path) where T : Object
        {
            if (!objectContainerDict.TryGetValue(path, out Object value))
            {
                Debug.LogError($"Is Not Found Object : {path}");
                return default(T);
            }

            if (!value is T)
            {
                Debug.LogError($"Object Is Not Inheritance : {typeof(T).Name}");
                return default(T);
            }

            return (T)value;
        }
    }
}
