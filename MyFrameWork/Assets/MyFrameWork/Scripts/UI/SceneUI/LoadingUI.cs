using Common.Objects;
using Common.SceneEx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : BaseSceneUI
{
    [Header("ProgressBar")]
    [SerializeField] private Image progressBar;

    private float fillAmount;

    private async void Start()
    {
        await LoadSceneProcessAsync();
    }

    /// <summary>
    /// 로딩 비동기 실행 함수
    /// </summary>
    private async UniTask LoadSceneProcessAsync()
    {
        fillAmount = 0.0f;
        SetProgressBar();

        await ObjectManager.Add(SceneManagerEx.NextScene);
        AsyncOperation op = SceneManagerEx.LoadNextSceneAsync();

        float timer = 0f;
        while (!op.isDone)
        {
            await UniTask.Yield(PlayerLoopTiming.Update);

            if (fillAmount < 0.9f && op.progress == 0.9f)
            {
                fillAmount = Mathf.MoveTowards(fillAmount, 1f, Time.deltaTime);
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                fillAmount = Mathf.Lerp(0.9f, 1f, timer);
                if (fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    return;
                }
            }
            SetProgressBar();
        }
    }

    private void SetProgressBar()
    {
        if (progressBar != null)
            progressBar.fillAmount = fillAmount;
    }
}
