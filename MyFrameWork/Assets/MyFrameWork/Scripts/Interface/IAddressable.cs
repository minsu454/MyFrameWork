using System;
using UnityEngine;

/// <summary>
/// Addressable로 데이터 받는 친구들 인터페이스
/// </summary>
public interface IAddressable
{
    /// <summary>
    /// Addressable Release 이벤트(Destroy에 꼭 Invoke(this))
    /// </summary>
    public event Action<GameObject> ReleaseEvent;
}
