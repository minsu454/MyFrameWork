using Common.Objects;
using Common.Path;
using Common.StringEx;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

namespace Common.SceneEx
{
    public static class SceneJobLoader
    {
        public readonly static SortedList<LoadPriorityType, Action<string>> completedList = new SortedList<LoadPriorityType, Action<string>>(); //씬로드 되었을 때 호출순서 정렬 list
        public static bool UseOnLoadCompleted = false;

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
        private static void OnLoadCompleted(Scene scene, LoadSceneMode sceneMode)
        {
            if (!UseOnLoadCompleted)
            {
                completedList[LoadPriorityType.Sound]?.Invoke("");
                return;
            }

            string sceneName = scene.name.ToFirstName("_");

            foreach (var item in completedList)
            {
                try
                {
                    item.Value.Invoke(sceneName);
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
        public static void Add(LoadPriorityType type, Action<string> loadCompleted)
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
        private static void LoadScene(string sceneName)
        {
            GameObject prefab = ObjectManager.Return<GameObject>(AddressablePath.LoaderPath(sceneName));

            if (prefab == null)
            {
                Debug.LogWarning($"Addressable is Not Found GameObject : {sceneName}");
                return;
            }

            GameObject go = UnityEngine.Object.Instantiate(prefab);

            if (!go.TryGetComponent(out IInit loader))
            {
                Debug.LogError($"GameObject Is Not BaseScene Inheritance : {go}");
                return;
            }

            loader.Init();
        }
    }
}