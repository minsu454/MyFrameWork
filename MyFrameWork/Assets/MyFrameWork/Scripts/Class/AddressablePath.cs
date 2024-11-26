using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Path
{
    public static class AddressablePath
    {
        /// <summary>
        /// Loader 경로 반환 함수
        /// </summary>
        public static string LoaderPath(string name)
        {
            return $"Loader/{name}";
        }

        /// <summary>
        /// UI 경로 반환 함수
        /// </summary>
        public static string UIPath(string name)
        {
            return $"UI/{name}";
        }

        /// <summary>
        /// ObjectPoolSO 경로 반환 함수
        /// </summary>
        public static string ObjectPoolSOPath(string name)
        {
            return $"Pool/{name}";
        }

        /// <summary>
        /// ObjectPool 경로 반환 함수
        /// </summary>
        public static string ObjectPoolPath(string sceneName, string name)
        {
            return $"Pool/{sceneName}/{name}.prefab";
        }

        /// <summary>
        /// BGM 경로 반환 함수
        /// </summary>
        public static string BGMPath(string name)
        {
            return $"Sound/{name}";
        }
    }

    public static class ScenePath
    {
        /// <summary>
        /// 실질적인 씬 이름 반환해주는 함수
        /// </summary>
        public static string SceneName(SceneType type)
        {
            return $"{type}_Scene";
        }

        /// <summary>
        /// 실질적인 씬 이름 반환해주는 함수
        /// </summary>
        public static string SceneName(string sceneName)
        {
            return $"{sceneName}_Scene";
        }
    }
}

