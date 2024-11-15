using Cysharp.Threading.Tasks;

/// <summary>
/// 씬 동적생성 가능한 인터페이스
/// </summary>
public interface ISceneDynamicCreatable
{
    /// <summary>
    /// 비동기 -> 동기 Init
    /// </summary>
    public UniTask Init(string sceneName);
}