using System;
using System.Collections.Generic;

namespace Common.EnumExtensions
{
    public static class EnumExtensions
    {
        private static readonly Dictionary<Enum, string> enumToStringDict = new Dictionary<Enum, string>();      //enum타입 string 변환 저장해 놓는 Dictionary

        /// <summary>
        /// enum을 string으로 변환 함수
        /// </summary>
        public static string EnumToString<T>(this T type) where T : Enum
        {
            if (!enumToStringDict.TryGetValue(type, out string value))
            {
                value = type.ToString();
                enumToStringDict[type] = value;
            }

            return value;
        }
    }
}