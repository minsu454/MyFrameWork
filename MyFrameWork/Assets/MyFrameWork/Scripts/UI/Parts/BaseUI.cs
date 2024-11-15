using System;
using UnityEngine;

public class BaseUI : MonoBehaviour, IAddressable
{
    public event Action<GameObject> ReleaseEvent;

    public void OnDestroy()
    {
        ReleaseEvent?.Invoke(gameObject);
    }
}