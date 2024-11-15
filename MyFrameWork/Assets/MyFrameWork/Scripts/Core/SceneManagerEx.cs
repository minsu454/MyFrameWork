using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Common.SceneEx
{
    public static class SceneManagerEx
    {
        private static readonly Dictionary<SceneType, string> typeToStringDic = new Dictionary<SceneType, string>();   //씬타입 string 변환 저장해 놓는 Dictionary

        private static string nextScene;

        /// <summary>
        /// 씬 로드 함수
        /// </summary>
        public static void LoadScene(SceneType type)
        {
            SceneManager.LoadScene(GetSceneName(type));
        }

        /// <summary>
        /// 로딩 후 씬 로드 함수
        /// </summary>
        public static void LoadingAndNextScene(SceneType nextSceneType)
        {
            nextScene = GetSceneName(nextSceneType);
            SceneManager.LoadScene("Loading");
        }

        /// <summary>
        /// 씬 다음 씬 비동기 로드 함수
        /// </summary>
        public static AsyncOperation LoadNextSceneAsync()
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
            op.allowSceneActivation = false;

            return op;
        }

        /// <summary>
        /// 씬 이름 반환해주는 함수
        /// </summary>
        private static string GetSceneName(SceneType type)
        {
            if (!typeToStringDic.TryGetValue(type, out string name))
            {
                name = type.ToString();
                typeToStringDic[type] = name;
            }

            return name;
        }
    }
}
