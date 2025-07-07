using Common.Assets;
using Common.AssetResolver;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Common.Objects
{
    public static class ObjectManager
    {
        private static readonly Dictionary<string, Object> _objectContainerDict = new();  //비동기 캐시해주는 Dictionary
        private static AssetManager assetManager;

        public static void Init()
        {
            assetManager = Resources.Load<AssetManager>("Asset Manager");

            if (assetManager.UseResources)
                ResourcesLoad();
        }

        /// <summary>
        /// 비동기로 오브젝트 추가해주는 함수
        /// </summary>
        public static async UniTask Add(string label)
        {
            if (assetManager.UseAddressables)
                await AddressableLoadAsync(label);
        }

        /// <summary>
        /// Addressable에서 로드해주는 함수
        /// </summary>
        private static async UniTask AddressableLoadAsync(string label)
        {
            var list = await AddressableAssets.LoadDataWithLabelAsync(label);

            List<UniTask> taskList = new List<UniTask>();

            try
            {
                foreach (var item in list)
                {
                    taskList.Add(AddAddressableObjectAsync(item.PrimaryKey));
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
        private static void ResourcesLoad()
        {
            var list = assetManager.ResourcesPathList;

            foreach (string path in list)
            {
                Object obj = Resources.Load($"Auto/{path}");
                _objectContainerDict.Add(path, obj);
            }
        }

        /// <summary>
        /// 개별 오브젝트를 비동기로 로드하고 딕셔너리에 추가하는 함수
        /// </summary>
        private static async UniTask AddAddressableObjectAsync(string path)
        {
            Object obj = await AddressableAssets.LoadDataAsync<Object>(path);
            _objectContainerDict.Add(path, obj);
        }

        /// <summary>
        /// Dictionary 초기화 함수
        /// </summary>
        public static void Clear()
        {
            _objectContainerDict.Clear();
        }

        /// <summary>
        /// 재너릭 변환 오브젝트 반환 함수
        /// </summary>
        public static T Return<T>(string path) where T : Object
        {
            if (!_objectContainerDict.TryGetValue(path, out Object value))
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
            if (!_objectContainerDict.TryGetValue(path, out Object value))
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
            if (!_objectContainerDict.TryGetValue(path, out Object value))
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
