using Common.Assets;
using Cysharp.Threading.Tasks;
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
            await AddressableAsync(label);
        }

        /// <summary>
        /// Addressable에서 로드해주는 함수
        /// </summary>
        private static async UniTask AddressableAsync(string label)
        {
            var list = await AddressableAssets.LoadDataWithLabelAsync(label);

            List<UniTask> taskList = new List<UniTask>();

            try
            {
                foreach (var item in list)
                {
                    taskList.Add(LoadAndAddObjectAsync(item.PrimaryKey));
                }
                await UniTask.WhenAll(taskList);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Resources에서 로드해주는 함수
        /// </summary>
        private static async UniTask ResourcesAsync(string label)
        {
            await UniTask.CompletedTask;
            ResourcesLoaderSO loaderSO = Resources.Load<ResourcesLoaderSO>($"Loader/{label}LoadSO");

            foreach (LoadData data in loaderSO.loadDataList)
            {
                if (objectContainerDict.ContainsKey(data.path))
                {
                    continue;
                }

                objectContainerDict.Add(data.path, data.setObject);
            }
        }


        /// <summary>
        /// 개별 오브젝트를 비동기로 로드하고 딕셔너리에 추가하는 함수
        /// </summary>
        private static async UniTask LoadAndAddObjectAsync(string primaryKey)
        {
            Object obj = await AddressableAssets.LoadDataAsync<Object>(primaryKey);
            objectContainerDict.Add(primaryKey, obj);
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

        /// <summary>
        /// 재너릭 변환 오브젝트 반환 함수
        /// </summary>
        public static GameObject Instantiate(string path)
        {
            if (!objectContainerDict.TryGetValue(path, out Object value))
            {
                Debug.LogError($"Is Not Found Object : {path}");
                return null;
            }

            GameObject go = value as GameObject;

            if (go == null)
            {
                Debug.LogError($"Object Is Not GameObject : {path}");
                return null;
            }

            return Object.Instantiate(go);
        }

        /// <summary>
        /// 재너릭 변환 오브젝트 반환 함수
        /// </summary>
        public static GameObject Instantiate(string path, Transform parent)
        {
            if (!objectContainerDict.TryGetValue(path, out Object value))
            {
                Debug.LogError($"Is Not Found Object : {path}");
                return null;
            }

            GameObject go = value as GameObject;

            if (go == null)
            {
                Debug.LogError($"Object Is Not GameObject : {path}");
                return null;
            }

            return Object.Instantiate(go, parent);
        }
    }
}
