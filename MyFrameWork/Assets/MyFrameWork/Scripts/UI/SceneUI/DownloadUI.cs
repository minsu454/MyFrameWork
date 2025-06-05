using Common.Assets;
using Common.EnumExtensions;
using Common.SceneEx;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class DownloadUI : BaseSceneUI
{
    [SerializeField] private GameObject waitGo;     //들어가는 중 띄워주는 obj
    [SerializeField] private GameObject downGo;     //다운로드 obj

    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI sizeInfoText;
    [SerializeField] private TextMeshProUGUI downValueText;

    private readonly List<string> labelList = new List<string>() { SceneType.Title.EnumToString(), SceneType.InGame.EnumToString() };

    private long patchSize;
    private readonly Dictionary<string, long> patchDict = new Dictionary<string, long>();

    //private async void Start()
    //{
    //    waitGo.SetActive(true);
    //    downGo.SetActive(false);

    //    await AddressableAssets.Init();
    //    await CheckUpdateFiles();
    //}

    public async void DownLoadBtn()
    {
        await PatchFileAsync();
    }

    /// <summary>
    /// 업데이트 파일이 있는지 체크해주는 함수
    /// </summary>
    private async UniTask CheckUpdateFiles()
    {
        patchSize = await AddressableAssets.DownLoadAllSizeAsync(labelList);

        if (patchSize > decimal.Zero)
        {
            waitGo.SetActive(false);
            downGo.SetActive(true);

            sizeInfoText.text = GetFileSize(patchSize);
        }
        else
        {
            downValueText.text = " 100 % ";
            progressBar.fillAmount = 1f;
            await UniTask.WaitForSeconds(2f);
            GoTitle();
        }
    }

    /// <summary>
    /// 파일용량 string으로 변환 함수
    /// </summary>
    private string GetFileSize(long byteCount)
    {
        string size = "0 Bytes";

        if (byteCount >= 1073741824.0)
        {
            size = $"{byteCount / 1073741824.0:##.##} GB";
        }
        else if (byteCount >= 1048576.0)
        {
            size = $"{byteCount / 1048576.0:##.##} MB";
        }
        else if (byteCount >= 1024.0)
        {
            size = $"{byteCount / 1024.0:##.##} KB";
        }
        else if (byteCount > 0)
        {
            size = $"{byteCount} Bytes";
        }

        return size;
    }

    public async UniTask PatchFileAsync()
    {
        foreach (var label in labelList)
        {
            long result = await AddressableAssets.DownLoadSizeAsync(label);

            if (result != 0)
            {
                await DownLoadLabelAsync(label);
            }
        }

        await CheckDownLoadAsync();
    }

    private async UniTask DownLoadLabelAsync(string label)
    {
        patchDict[label] = 0;

        var handle = Addressables.DownloadDependenciesAsync(label);
        while (!handle.IsDone)
        {
            patchDict[label] = handle.GetDownloadStatus().DownloadedBytes;
            await UniTask.Yield();
        }

        patchDict[label] = handle.GetDownloadStatus().TotalBytes;
        Addressables.Release(handle);
    }

    private async UniTask CheckDownLoadAsync()
    {
        float total = 0f;
        downValueText.text = "0 %";

        while (true)
        {
            total += patchDict.Sum(temp => temp.Value);

            progressBar.fillAmount = total / patchSize;
            downValueText.text = ((int)(progressBar.fillAmount * 100)) + " %";

            if (total >= patchSize)
            {
                GoTitle();
                break;
            }

            total = 0f;
            await UniTask.Yield();
        }
    }

    public void GoTitle()
    {
        SceneManagerEx.LoadingAndNextScene(SceneType.Title);
    }

    public void GoInGame()
    {
        SceneManagerEx.LoadingAndNextScene(SceneType.InGame);
    }
}
