using Common.EnumExtensions;
using Common.Path;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Common.SceneEx
{
    public static class SceneManagerEx
    {
        private static string nextScene;    //로딩 씬 후 다음 씬 이름 저장 변수
        public static string NextScene
        {
            get { return nextScene; }
        }

        /// <summary>
        /// 씬 로드 함수
        /// </summary>
        public static void LoadScene(SceneType type)
        {
            SceneManager.LoadScene(ScenePath.SceneName(type));
        }

        /// <summary>
        /// 로딩 후 씬 로드 함수
        /// </summary>
        public static void LoadingAndNextScene(SceneType nextSceneType)
        {
            nextScene = nextSceneType.EnumToString();

            SceneJobLoader.UseOnLoadCompleted = false;
            SceneManager.LoadScene("Loading_Scene");
        }

        /// <summary>
        /// 다음 씬 비동기 로드 함수
        /// </summary>
        public static AsyncOperation LoadNextSceneAsync()
        {
            SceneJobLoader.UseOnLoadCompleted = true;
            AsyncOperation op = SceneManager.LoadSceneAsync(ScenePath.SceneName(nextScene));
            op.allowSceneActivation = false;

            return op;
        }
    }
}
