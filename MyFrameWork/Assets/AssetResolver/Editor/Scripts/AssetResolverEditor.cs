using UnityEditor;
using UnityEngine;

namespace Common.AssetResolver
{
    [CustomEditor(typeof(AssetManager))]
    [System.Serializable]
    public class AssetResolverEditor : Editor
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

            serializedObject.ApplyModifiedProperties();
            Repaint();

            GUILayout.Space(10);

            EditorGUI.BeginDisabledGroup(true);
            var ResourcesPathList = serializedObject.FindProperty("ResourcesPathList");
            EditorGUILayout.PropertyField(ResourcesPathList, true);
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Set Resources Path", customSkin.button)) { SetResourcesPath(); serializedObject.ApplyModifiedProperties(); Repaint(); }

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Resources Converter", customSkin.button)) { ResourcesConverter(); }
            if (GUILayout.Button("Addressables Converter", customSkin.button)) { AddressablesConverter(); }

            GUILayout.EndHorizontal();
        }

        private void SetResourcesPath() { AssetResolver.SetAutoResourcesPath(); }

        private void ResourcesConverter() { AssetResolverWindow.ResourcesConverterWindow(); }
        private void AddressablesConverter() { AssetResolverWindow.AddressablesConverterWindow(); }
    }
}
