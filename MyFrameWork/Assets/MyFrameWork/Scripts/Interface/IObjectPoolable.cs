using System;
using UnityEngine;

public interface IObjectPoolable<T> where T : Component
{
    /// <summary>
    /// ObjectPool Return 이벤트(Invoke(this) 필수)
    /// </summary>
    public event Action<T> ReturnEvent;
}