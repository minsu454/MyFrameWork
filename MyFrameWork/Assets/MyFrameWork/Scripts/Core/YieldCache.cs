using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Yield//using문을 사용하게 하는 문장
{
    public static class YieldCache//클래스 앞에 스테틱이면 변수들도 스테틱이어야함
    {
        private static readonly Dictionary<float, WaitForSeconds> waitForSecondsDic = new Dictionary<float, WaitForSeconds>();


        /// <summary>
        /// new WaitForSeconds를 dictionary에 가져오는 함수(dictionary에 값이 없을 시엔 add해줌)
        /// </summary>
        public static WaitForSeconds WaitForSeconds(float delayTime)
        {
            if (waitForSecondsDic.TryGetValue(delayTime, out WaitForSeconds wait))
            {
                return wait;
            }

            WaitForSeconds waitForSeconds = new WaitForSeconds(delayTime);
            waitForSecondsDic.Add(delayTime, waitForSeconds);
            return waitForSeconds;
        }
    }
}
