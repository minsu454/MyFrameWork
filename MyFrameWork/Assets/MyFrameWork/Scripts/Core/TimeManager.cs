using System;
using System.Collections.Generic;

namespace Common.Time
{
    public static class TimeManager
    {
        private static readonly Dictionary<TimeType, float> _timeContainerDict = new(); 

        /// <summary>
        /// 초기화 함수
        /// </summary>
        public static void Init()
        {
            foreach (TimeType type in Enum.GetValues(typeof(TimeType)))
            {
                _timeContainerDict.Add(type, 1f);
            }
        }

        /// <summary>
        /// 시간 설정해주는 함수
        /// </summary>
        public static void SetTime(this TimeType type, float timeScale)
        {
            if (_timeContainerDict.TryGetValue(type, out float value) is false)
            {
                throw new Exception($"TimeType is None. : {type}");
            }

            _timeContainerDict[type] = timeScale;
        }

        /// <summary>
        /// 시간 반환 함수
        /// </summary>
        public static float Get(this TimeType type)
        {
            return _timeContainerDict[type];
        }
    }
}