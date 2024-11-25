using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Event
{
    public delegate void EventListener(object args);

    public static class EventManager
    {

        //eventListener저장해주는 Dictionary
        private static readonly Dictionary<GameEventType, List<EventListener>> eventListenerDict = new Dictionary<GameEventType, List<EventListener>>();

        /// <summary>
        /// 구독하는 함수
        /// </summary>
        public static void Subscribe(GameEventType type, EventListener listener)
        {
            if (!eventListenerDict.TryGetValue(type, out var list))
            {
                list = new List<EventListener>();
                eventListenerDict[type] = list;
            }

            list.Add(listener);
        }

        /// <summary>
        /// 구독취소하는 함수
        /// </summary>
        public static void Unsubscribe(GameEventType type, EventListener listener)
        {
            if (!eventListenerDict.TryGetValue(type, out var list))
            {
                return;
            }

            list.Remove(listener);
            if (list.Count == 0)
            {
                eventListenerDict.Remove(type);
            }
        }

        /// <summary>
        /// 이벤트 달성 시 구독한 리스트 실행해주는 함수
        /// </summary>
        public static void Dispatch(GameEventType type, object arg)
        {
            if (!eventListenerDict.TryGetValue(type, out var list))
            {
                return;
            }

            foreach (var listener in list)
            {
                try
                {
                    listener.Invoke(arg);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}