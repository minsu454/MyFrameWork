using Common.Assets;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Objects
{
    public static class ObjectManager
    {
        private static readonly Dictionary<string, Object> objectContainerDict = new Dictionary<string, Object>();  //비동기 캐시해주는 Dictionary

        /// <summary>
        /// 비동기로 오브젝트 추가해주는 함수
        /// </summary>
        public static async UniTask Add(string label)
        {
            var list = await AddressableAssets.LoadDataWithLabelAsync(label);

            try
            {
                foreach (var item in list)
                {
                    Object obj = await AddressableAssets.LoadDataAsync<Object>(item.PrimaryKey);
                    objectContainerDict.Add(item.PrimaryKey, obj);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Dictionary 초기화 함수
        /// </summary>
        public static void Clear()
        {
            objectContainerDict.Clear();
        }

        /// <summary>
        /// 재너릭 변환 오브젝트 반환 함수
        /// </summary>
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
