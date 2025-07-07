using UnityEditor;
using UnityEngine;

namespace Common.AssetResolver
{
    public class EditorHandler : Editor
    {
        public static void DrawHeader(GUISkin skin, string content, int space)
        {
            GUILayout.Space(space);
            GUILayout.Box(new GUIContent(""), skin.FindStyle(content));
        }

        public static bool DrawToggle(bool value, GUISkin skin, string content)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            value = GUILayout.Toggle(value, new GUIContent(content), skin.FindStyle("Toggle"));

            GUILayout.EndHorizontal();
            return value;
        }
    }
}