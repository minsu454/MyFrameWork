using Common.Objects;
using Common.SceneEx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : BaseSceneUI
{
    [SerializeField] private Image progressBar;

    private async void Start()
    {
        await LoadSceneProcessAsync();
    }

    /// <summary>
    /// 로딩 비동기 실행 함수
    /// </summary>
    private async UniTask LoadSceneProcessAsync()
    {
        progressBar.fillAmount = 0.0f;

        await ObjectManager.Add(SceneManagerEx.NextScene);
        AsyncOperation op = SceneManagerEx.LoadNextSceneAsync();

        float timer = 0f;
        while (!op.isDone)
        {
            await UniTask.Yield(PlayerLoopTiming.Update);

            if (progressBar.fillAmount < 0.9f && op.progress == 0.9f)
            {
                progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, 1f, Time.deltaTime);
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);
                if (progressBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    return;
                }
            }
        }
    }

}
