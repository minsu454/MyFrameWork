using UnityEngine;

namespace Common.ColorEx
{
    public static class ColorEx
    {
        /// <summary>
        /// 알파 값 변경 함수
        /// </summary>
        public static Color Alpha(this Color color, float value)
        {
            Color temp = color;
            temp.a = value;
            return temp;
        }
        
    }
}