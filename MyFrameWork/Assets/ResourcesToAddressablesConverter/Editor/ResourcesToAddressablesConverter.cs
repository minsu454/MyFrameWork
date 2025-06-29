using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class ResourcesToAddressablesConverter : EditorWindow
{
    private const string ResourcesFolder = "Assets/Resources";
    private const string AddressableFolder = "Assets/Addressables";

    private TreeViewState treeViewState;
    private FileTreeView treeView;
    private bool selectAllToggle;
    private string currentRoot;

    public static void ResourcesConverterWindow()
    {
        var window = GetWindow<ResourcesToAddressablesConverter>(true, "Resources Converter", true);
        window.minSize = new Vector2(350, 500);
        window.maxSize = new Vector2(350, 500);
        window.InitTree(ResourcesFolder);
        window.currentRoot = ResourcesFolder;
    }

    public static void AddressablesConverterWindow()
    {
        var window = GetWindow<ResourcesToAddressablesConverter>(true, "Addressables Converter", true);
        window.minSize = new Vector2(350, 500);
        window.maxSize = new Vector2(350, 500);
        window.InitTree(AddressableFolder);
        window.currentRoot = AddressableFolder;
    }

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
            
            if(result)
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

    private void ConvertSelected()
    {
        var selected = treeView.GetSelectedFilePathList();

        if (selected.Count == 0)
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
            foreach (string oldPath in selected)
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

            FileUtil.DeleteFileOrDirectory(ResourcesFolder);
        }
        else if (currentRoot == AddressableFolder)
        {
            foreach (string oldPath in selected)
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

            FileUtil.DeleteFileOrDirectory(AddressableFolder);
        }

        AssetDatabase.Refresh();
        settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true);
        AssetDatabase.SaveAssets();
        Close();

        Debug.Log("Conversion complete.");
    }
}
