using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Yield
{
    public static class YieldCache
    {
        private static readonly Dictionary<float, WaitForSeconds> _waitForSecondsDict = new();

        /// <summary>
        /// new WaitForSeconds를 dictionary에 가져오는 함수(dictionary에 값이 없을 시엔 add해줌)
        /// </summary>
        public static WaitForSeconds WaitForSeconds(float delayTime)
        {
            if (_waitForSecondsDict.TryGetValue(delayTime, out WaitForSeconds wait))
            {
                return wait;
            }

            WaitForSeconds waitForSeconds = new WaitForSeconds(delayTime);
            _waitForSecondsDict.Add(delayTime, waitForSeconds);
            return waitForSeconds;
        }
    }
}
