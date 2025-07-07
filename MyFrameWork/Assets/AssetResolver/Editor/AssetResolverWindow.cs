using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Common.AssetResolver
{
    public class AssetResolverWindow : EditorWindow
    {
        private const string ResourcesFolder = "Assets/Resources";
        private const string AddressableFolder = "Assets/Addressables";

        private TreeViewState treeViewState;
        private FileTreeView treeView;
        private string currentRoot;

        /// <summary>
        /// ResourcesConverter 창 생성 함수
        /// </summary>
        public static void ResourcesConverterWindow()
        {
            var window = GetWindow<AssetResolverWindow>(true, "Resources Converter", true);
            window.minSize = new Vector2(350, 500);
            window.maxSize = new Vector2(350, 500);
            window.InitTree(ResourcesFolder);
            window.currentRoot = ResourcesFolder;
        }

        /// <summary>
        /// AddressablesConverter 창 생성 함수
        /// </summary>
        public static void AddressablesConverterWindow()
        {
            var window = GetWindow<AssetResolverWindow>(true, "Addressables Converter", true);
            window.minSize = new Vector2(350, 500);
            window.maxSize = new Vector2(350, 500);
            window.InitTree(AddressableFolder);
            window.currentRoot = AddressableFolder;
        }

        /// <summary>
        /// TreeViewState 초기화 함수
        /// </summary>
        private void InitTree(string path)
        {
            treeViewState = new TreeViewState();
            treeView = new FileTreeView(treeViewState, path);
        }

        private void OnGUI()
        {
            if (treeView == null || treeView.IsEmpty())
            {
                bool result = EditorUtility.DisplayDialog("Error", "The converter will not work because the file is missing.", "OK");

                if (result)
                    Close();

                return;
            }

            Rect treeRect = GUILayoutUtility.GetRect(0, 100000, 0, position.height - 30);
            treeView.OnGUI(treeRect);

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Cancel")) Close();
            if (GUILayout.Button("Convert")) ConvertSelected();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 변환 함수
        /// </summary>
        private void ConvertSelected()
        {
            var selectedList = treeView.GetSelectedFilePathList();

            if (selectedList.Count == 0)
            {
                EditorUtility.DisplayDialog("No Files to Convert", "There are no files selected or nothing left to convert.", "OK");
                return;
            }

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("Addressable Asset Settings not found.");
                return;
            }

            if (currentRoot == ResourcesFolder)
            {
                ConvertAddressables(selectedList, settings);
            }
            else if (currentRoot == AddressableFolder)
            {
                ConvertResources(selectedList, settings);
            }

            AssetDatabase.Refresh();
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true);
            AssetDatabase.SaveAssets();
            Close();

            Debug.Log("Conversion complete.");
        }

        /// /// <summary>
        /// Addressables로 바꾸는 함수
        /// </summary>
        private void ConvertAddressables(List<string> selectedList, AddressableAssetSettings settings)
        {
            foreach (string oldPath in selectedList)
            {
                string relativePath = oldPath.Substring(ResourcesFolder.Length + 1);
                string exportPath = Path.Combine(AddressableFolder, relativePath).Replace("\\", "/");
                string exportDir = Path.GetDirectoryName(exportPath);

                if (!Directory.Exists(exportDir))
                    Directory.CreateDirectory(exportDir);

                AssetDatabase.CopyAsset(oldPath, exportPath);
                AssetDatabase.DeleteAsset(oldPath);

                string groupName = relativePath.Split('/')[0];
                string address = Path.ChangeExtension(relativePath, null);

                var group = settings.FindGroup(groupName) ?? settings.CreateGroup(groupName, false, false, false, null, typeof(BundledAssetGroupSchema));
                string newGuid = AssetDatabase.AssetPathToGUID(exportPath);
                var entry = settings.CreateOrMoveEntry(newGuid, group);
                entry.address = address;
            }

            DeleteAllEmptyFolders(ResourcesFolder);
        }

        /// <summary>
        /// Resources로 바꾸는 함수
        /// </summary>
        private void ConvertResources(List<string> selectedList, AddressableAssetSettings settings)
        {
            foreach (string oldPath in selectedList)
            {
                string relativePath = oldPath.Substring(AddressableFolder.Length + 1);
                string exportPath = Path.Combine(ResourcesFolder, relativePath).Replace("\\", "/");
                string exportDir = Path.GetDirectoryName(exportPath);

                if (!Directory.Exists(exportDir))
                    Directory.CreateDirectory(exportDir);

                AssetDatabase.CopyAsset(oldPath, exportPath);
                AssetDatabase.DeleteAsset(oldPath);

                string newGuid = AssetDatabase.AssetPathToGUID(oldPath);
                settings.RemoveAssetEntry(newGuid);
            }

            DeleteAllEmptyFolders(AddressableFolder);
            DeleteAllEmptyGroups(settings);
        }

        /// <summary>
        /// 아무 것도 없는 폴더삭제 함수
        /// </summary>
        private void DeleteAllEmptyFolders(string root)
        {
            foreach (string dir in Directory.GetDirectories(root, "*", SearchOption.AllDirectories).OrderByDescending(d => d.Length))
            {
                // Skip if any non-meta files exist
                if (Directory.GetFiles(dir).Any(file => !file.EndsWith(".meta")) || Directory.GetDirectories(dir).Length > 0)
                    continue;

                Directory.Delete(dir);
                string metaFile = dir + ".meta";
                if (File.Exists(metaFile))
                    File.Delete(metaFile);
            }
        }

        /// <summary>
        /// 아무 것도 없는 Addressable Group삭제 함수
        /// </summary>
        private void DeleteAllEmptyGroups(AddressableAssetSettings settings)
        {
            var groupsToDelete = settings.groups
                .Where(g => g != null && g.entries.Count == 0 && !g.ReadOnly)
                .ToList();

            foreach (var group in groupsToDelete)
            {
                settings.RemoveGroup(group);
            }
        }
    }
}