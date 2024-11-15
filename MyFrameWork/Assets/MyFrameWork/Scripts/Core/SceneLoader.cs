using Common.Assets;
using Common.Path;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

namespace Common.SceneEx
{
    public static class SceneLoader
    {
        public readonly static SortedList<LoadPriorityType, Func<Scene, UniTask>> completedList = new SortedList<LoadPriorityType, Func<Scene, UniTask>>(); //씬로드 되었을 때 호출순서 정렬 list
        
        /// <summary>
        /// 초기화 함수
        /// </summary>
        public static void Init()
        {
            Add(LoadPriorityType.BaseScene, LoadScene);

            SceneManager.sceneLoaded += OnLoadCompleted;
        }

        /// <summary>
        /// 씬 로드 완료 시 호출 이벤트 함수
        /// </summary>
        private static async void OnLoadCompleted(Scene scene, LoadSceneMode sceneMode)
        {
            foreach (var item in completedList)
            {
                try
                {
                    await item.Value.Invoke(scene);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        /// <summary>
        /// 씬 로드 시 실행 Action 추가 함수
        /// </summary>
        public static void Add(LoadPriorityType type, Func<Scene, UniTask> loadCompleted)
        {
            if (completedList.ContainsKey(type))
            {
                Debug.LogWarning($"There is already an identical LoadCompleted event : {type}");
                return;
            }

            completedList.Add(type, loadCompleted);
        }

        /// <summary>
        /// 씬 로드시 실행 Action 지우는 함수
        /// </summary>
        public static bool Remove(LoadPriorityType type)
        {
            if (completedList[type] == null)
            {
                Debug.LogError($"Is Not found competedList : {type}");
                return false;
            }

            completedList.Remove(type);
            return true;
        }

        /// <summary>
        /// 씬 로드시 메인 Manager생성 함수
        /// </summary>
        private static async UniTask LoadScene(Scene scene)
        {
            GameObject go = await AddressableAssets.InstantiateAsync(AdressablePath.ScenePath(scene.name));

            if (!go.TryGetComponent(out ISceneDynamicCreatable baseScene))
            {
                Debug.LogError($"GameObject Is Not BaseScene Inheritance : {go}");
                return;
            }

            await baseScene.Init(scene.name);
        }
    }
}