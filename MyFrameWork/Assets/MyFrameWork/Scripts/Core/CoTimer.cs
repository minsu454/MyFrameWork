using System;
using System.Collections;

namespace Common.Timer
{
    public static class CoTimer
    {
        /// <summary>
        /// delay이후 callback함수 호출해주는 코루틴
        /// </summary>
        public static IEnumerator Start(float delayTime, Action callback)
        {
            yield return Yield.YieldCache.WaitForSeconds(delayTime);
            callback.Invoke();
        }
    }
}