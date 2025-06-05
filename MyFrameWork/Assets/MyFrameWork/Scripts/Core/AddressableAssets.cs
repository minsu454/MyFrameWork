using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.ResourceLocations;
using System.Linq;

namespace Common.Assets
{
    public static class AddressableAssets
    {
        /// <summary>
        /// 초기화 함수
        /// </summary>
        public static async UniTask Init()
        {
            await Addressables.InitializeAsync();
        }

        /// <summary>
        /// 다운로드 사이즈 반환 함수
        /// </summary>
        public static async UniTask<long> DownLoadSizeAsync(string label)
        {
            var handle = Addressables.GetDownloadSizeAsync(label);
            await handle;

            return handle.Result;
        }

        /// <summary>
        /// 전체 다운로드 사이즈 반환 함수
        /// </summary>
        public static async UniTask<long> DownLoadAllSizeAsync(IList<string> labelList)
        {
            var tasks = labelList.Select(async label =>
            {
                var handle = Addressables.GetDownloadSizeAsync(label);
                await handle;
                return handle.Result;
            });
            var results = await UniTask.WhenAll(tasks);

            return results.Sum();
        }

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
            try
            {
                T t = await Addressables.LoadAssetAsync<T>(path);
                return t;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 비동기로 해당 라벨에 모든 데이터 긁어오는 함수
        /// </summary>
        public static async UniTask<IList<IResourceLocation>> LoadDataWithLabelAsync(string Label)
        {
            try
            {
                IList<IResourceLocation> list = await Addressables.LoadResourceLocationsAsync(Label);
                return list;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 비동기로 Instantiate 해주는 함수
        /// </summary>
        public static async UniTask<GameObject> InstantiateAsync(string path)
        {
            try
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
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 비동기로 Instantiate 해주는 함수
        /// </summary>
        public static async UniTask<GameObject> InstantiateAsync(string path, Transform parent)
        {
            try
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
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 비동기로 Instantiate 해주는 함수
        /// </summary>
        public static async UniTask<GameObject> InstantiateAsync(string path, Vector3 pos, Quaternion rot, Transform parent = null)
        {
            try
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
            catch
            {
                return null;
            }
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

