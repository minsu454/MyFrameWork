using System;

namespace Common.StringEx
{
    public static class StringExtensions
    {
        /// <summary>
        /// string을 enum으로 변환 함수
        /// </summary>
        public static T StringToEnum<T>(this string value)
        {
            if (!Enum.IsDefined(typeof(T), value)) return default(T);

            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Split에 첫번째 이름 가져오는 함수
        /// </summary>
        public static string ToFirstName(this string value, string separator)
        {
            string[] arr = value.Split(separator);

            if (arr.Length <= 1)
            {
                throw new Exception($"The value is not the target of ToName : {value}");
            }

            return arr[0];
        }
    }
}
