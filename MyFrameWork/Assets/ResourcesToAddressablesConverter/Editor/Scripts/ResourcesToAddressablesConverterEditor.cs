using UnityEditor;
using UnityEngine;

namespace Common.ResourcesToAddressablesConverter
{
    [CustomEditor(typeof(ConverterManager))]
    [System.Serializable]
    public class ResourcesToAddressablesConverterEditor : Editor
    {
        GUISkin customSkin;

        void OnEnable()
        {
            customSkin = (GUISkin)Resources.Load("Editor\\DUI Skin");
        }

        public override void OnInspectorGUI()
        {
            if (customSkin == null)
            {
                EditorGUILayout.HelpBox("Editor variables are missing. You can manually fix this by deleting.", MessageType.Error);
                return;
            }

            EditorHandler.DrawHeader(customSkin, "Options Header", 14);
            GUILayout.Space(3);

            var useResources = serializedObject.FindProperty("UseResources");
            useResources.boolValue = EditorHandler.DrawToggle(useResources.boolValue, customSkin, "Use Resources");

            var useAddressables = serializedObject.FindProperty("UseAddressables");
            useAddressables.boolValue = EditorHandler.DrawToggle(useAddressables.boolValue, customSkin, "Use Addressables");
        }
    }
}
