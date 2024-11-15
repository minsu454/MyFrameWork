using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

namespace Common.Assets
{
    public static class AddressableAssets
    {
        /// <summary>
        /// 동기로 정보 가져오기(비권장)
        /// </summary>
        public static T LoadData<T>(string path) where T : class
        {
            var loadAsset = Addressables.LoadAssetAsync<T>(path);

            T t = loadAsset.WaitForCompletion();

            if (loadAsset.Status == AsyncOperationStatus.Succeeded)
            {
                return t;
            }

            throw new Exception($"Addressable Load Failed : {path}");
        }

        /// <summary>
        /// 비동기로 정보 가져오기
        /// </summary>
        public static async UniTask<T> LoadDataAsync<T>(string path) where T : class
        {
            T t = await Addressables.LoadAssetAsync<T>(path);
            
            return t;
        }

        /// <summary>
        /// 비동기로 Instantiate 해주는 함수
        /// </summary>
        public static async UniTask<GameObject> InstantiateAsync(string path)
        {
            GameObject go = await Addressables.InstantiateAsync(path);

            if (!go.TryGetComponent<IAddressable>(out var addressable))
            {
                Debug.LogError($"GameObject Is Not IAddressable Inheritance : {path}");
                return null;
            }

            addressable.ReleaseEvent += Release;

            return go;
        }

        /// <summary>
        /// 비동기로 Instantiate 해주는 함수
        /// </summary>
        public static async UniTask<GameObject> InstantiateAsync(string path, Transform parent)
        {
            GameObject go = await Addressables.InstantiateAsync(path, parent);
            if (!go.TryGetComponent<IAddressable>(out var addressable))
            {
                Debug.LogError($"GameObject Is Not IAddressable Inheritance : {path}");
                return null;
            }

            addressable.ReleaseEvent += Release;

            return go;
        }

        /// <summary>
        /// 비동기로 Instantiate 해주는 함수
        /// </summary>
        public static async UniTask<GameObject> InstantiateAsync(string path, Vector3 pos, Quaternion rot, Transform parent = null)
        {
            GameObject go = await Addressables.InstantiateAsync(path, pos, rot, parent);

            if (!go.TryGetComponent<IAddressable>(out var addressable))
            {
                Debug.LogError($"GameObject Is Not IAddressable Inheritance : {path}");
                return null;
            }

            addressable.ReleaseEvent += Release;

            return go;
        }

        /// <summary>
        /// Release해주는 함수
        /// </summary>
        public static void Release(GameObject go)
        {
            Addressables.ReleaseInstance(go);
        }
    }
}

