using Cysharp.Threading.Tasks;

public class InGameScene : BaseScene<InGameScene>
{
    public override async UniTask InitScene()
    {
        await UniTask.CompletedTask;
    }
}
