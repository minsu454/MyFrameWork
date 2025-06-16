using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class ResourcesToAddressablesPackageLikeImporter : EditorWindow
{
    private const string ResourcesFolder = "Assets/Resources";
    private const string AddressableExportRoot = "Assets/Addressables";

    private TreeViewState treeViewState;
    private FileTreeView treeView;

    [MenuItem("Tools/Convert/01_Import Style Resources → Addressables")]
    public static void ShowWindow()
    {
        var window = GetWindow<ResourcesToAddressablesPackageLikeImporter>(true, "Import Resources Package", true);
        window.minSize = new Vector2(600, 300);
        window.InitTree();
    }

    private void InitTree()
    {
        treeViewState = new TreeViewState();
        treeView = new FileTreeView(treeViewState, ResourcesFolder);
    }

    private void OnGUI()
    {
        GUILayout.Label("Import Resources Like UnityPackage", EditorStyles.boldLabel);

        Rect treeRect = GUILayoutUtility.GetRect(0, 100000, 0, position.height - 60);
        treeView.OnGUI(treeRect);

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Cancel")) Close();
        if (GUILayout.Button("Import")) ConvertSelected();
        GUILayout.EndHorizontal();
    }

    private void ConvertSelected()
    {
        var selected = treeView.GetSelectedFilePaths();
        if (selected.Count == 0)
        {
            EditorUtility.DisplayDialog("Nothing Selected", "Please select files to import.", "OK");
            return;
        }

        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("Addressable Asset Settings not found.");
            return;
        }

        foreach (string oldPath in selected)
        {
            string relativePath = oldPath.Substring(ResourcesFolder.Length + 1);
            string exportPath = Path.Combine(AddressableExportRoot, relativePath).Replace("\\", "/");
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
        AssetDatabase.Refresh();
        settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true);
        AssetDatabase.SaveAssets();
        Close();

        Debug.Log("Conversion complete. Resources moved to Addressables.");
    }
}

public class FileTreeView : TreeView
{
    private void SetChildrenSelection(TreeViewItem parent, bool selected)
    {
        if (parent.hasChildren)
        {
            foreach (FileItem child in parent.children)
            {
                child.selected = selected;
                SetChildrenSelection(child, selected);
            }
        }
    }

    private class FileItem : TreeViewItem
    {
        public string fullPath;
        public bool selected;
        public bool isFolder;
    }

    private readonly List<FileItem> allItems = new();
    private readonly string rootFolder;

    public FileTreeView(TreeViewState state, string rootFolderPath) : base(state)
    {
        rootFolder = rootFolderPath;
        Reload();
    }

    protected override TreeViewItem BuildRoot()
    {
        var root = new FileItem
        {
            id = 0,
            depth = -1,
            displayName = "Addressables",
            fullPath = rootFolder,
            isFolder = true,
            selected = true
        };
        int id = 1;
        BuildTree(rootFolder, root, ref id);
        SetupDepthsFromParentsAndChildren(root);
        return root;
    }

    private void BuildTree(string currentPath, TreeViewItem parent, ref int id)
    {
        foreach (var dir in Directory.GetDirectories(currentPath))
        {
            var item = new FileItem
            {
                id = id++,
                depth = parent.depth + 1,
                displayName = Path.GetFileName(dir),
                fullPath = dir.Replace("\\", "/"),
                isFolder = true
            };
            allItems.Add(item);
            parent.AddChild(item);
            BuildTree(dir, item, ref id);
        }

        foreach (var file in Directory.GetFiles(currentPath))
        {
            if (file.EndsWith(".meta")) continue;
            var item = new FileItem
            {
                id = id++,
                depth = parent.depth + 1,
                displayName = Path.GetFileName(file),
                fullPath = file.Replace("\\", "/"),
                isFolder = false
            };
            allItems.Add(item);
            parent.AddChild(item);
        }
    }

    protected override void RowGUI(RowGUIArgs args)
    {
        var item = (FileItem)args.item;
        float indent = GetContentIndent(item);
        Rect toggleRect = new Rect(args.rowRect.x + indent, args.rowRect.y, 18, args.rowRect.height);
        Rect iconRect = new Rect(toggleRect.xMax + 2, args.rowRect.y, 18, args.rowRect.height);
        Rect labelRect = new Rect(iconRect.xMax + 2, args.rowRect.y, args.rowRect.width - iconRect.xMax - 60, args.rowRect.height);
        Rect newLabelRect = new Rect(args.rowRect.xMax - 50, args.rowRect.y, 50, args.rowRect.height);

        EditorGUI.BeginChangeCheck();
        bool toggled = EditorGUI.Toggle(toggleRect, item.selected);
        if (toggled != item.selected)
        {
            item.selected = toggled;
            SetChildrenSelection(item, toggled);
        }
        if (EditorGUI.EndChangeCheck())
        {
            item.selected = toggled;
        }

        Texture icon = item.isFolder
            ? EditorGUIUtility.IconContent("Folder Icon").image
            : AssetDatabase.GetCachedIcon(item.fullPath);
        if (icon != null)
        {
            GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
        }

        EditorGUI.LabelField(labelRect, item.displayName);

        EditorGUI.LabelField(labelRect, item.displayName);

        if (!item.isFolder)
        {
            GUI.Label(newLabelRect, "New", EditorStyles.miniBoldLabel);
        }
    }


    public List<string> GetSelectedFilePaths()
    {
        return allItems.Where(i => i.selected && !i.isFolder).Select(i => i.fullPath).ToList();
    }

    public void SelectAll()
    {
        foreach (var i in allItems)
        {
            if (!i.isFolder) i.selected = true;
        }
        Reload();
    }

    public void DeselectAll()
    {
        foreach (var i in allItems)
        {
            i.selected = false;
        }
        Reload();
    }
}
