using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Common.AssetResolver
{
    public class AssetResolver
    {
        /// <summary>
        /// Resources 경로들 자동 저장
        /// </summary>
        public static void SetAutoResourcesPath()
        {
            AssetManager assetManager = Resources.Load<AssetManager>("Asset Manager");

            assetManager.ResourcesPathList.Clear();

            string[] guids = AssetDatabase.FindAssets("", new[] { "Assets/Resources/Auto" });

            foreach (var guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                if (assetPath.EndsWith(".meta")) continue;
                if (AssetDatabase.IsValidFolder(assetPath)) continue;

                string relativePath = assetPath.Substring("Assets/Resources/Auto/".Length);
                string resourcePath = Path.ChangeExtension(relativePath, null);

                assetManager.ResourcesPathList.Add(resourcePath);
            }

            AssetDatabase.SaveAssets();
        }
    }
}