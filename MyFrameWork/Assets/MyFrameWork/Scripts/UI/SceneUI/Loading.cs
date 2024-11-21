using Common.SceneEx;
using Cysharp.Threading.Tasks;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : BaseSceneUI
{
    [SerializeField] private Image progressBar;

    private void Start()
    {
        StartCoroutine(LoadSceneProcess());
    }

    IEnumerator LoadSceneProcess()
    {
        progressBar.fillAmount = 0.0f;

        AsyncOperation op = SceneManagerEx.LoadNextSceneAsync();

        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;

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
                    yield break;
                }
            }
        }
    }
}
