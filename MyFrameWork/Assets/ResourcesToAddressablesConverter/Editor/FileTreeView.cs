using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class FileTreeView : TreeView
{
    private class FileItem : TreeViewItem
    {
        public string fullPath;
        public bool selected;
        public bool isFolder;
    }

    public bool IsEmpty() => _itemList.All(i => i.isFolder);

    private readonly List<FileItem> _itemList = new();
    private readonly string rootFolder;

    public FileTreeView(TreeViewState state, string rootFolderPath) : base(state)
    {
        rootFolder = rootFolderPath;
        Reload();

        foreach (var item in GetRows())
            SetExpandedRecursive(item.id, true);
    }

    /// <summary>
    /// 루트 노드를 반환 함수
    /// </summary>
    protected override TreeViewItem BuildRoot()
    {
        int id = 0;
        var virtualRoot = new TreeViewItem { id = id++, depth = -1, displayName = "Root" };
        var root = CreateItem(rootFolder, Path.GetFileName(rootFolder), 0, true, ref id);

        virtualRoot.AddChild(root);

        BuildTree(rootFolder, root, ref id);
        SetupDepthsFromParentsAndChildren(virtualRoot);

        return virtualRoot;
    }

    /// <summary>
    /// 트리 만들어주는 함수
    /// </summary>
    private void BuildTree(string currentPath, TreeViewItem parent, ref int id)
    {
        foreach (var dir in Directory.GetDirectories(currentPath))
        {
            var item = CreateItem(dir, Path.GetFileName(dir), parent.depth + 1, true, ref id);
            parent.AddChild(item);
            BuildTree(dir, item, ref id);
        }

        foreach (var file in Directory.GetFiles(currentPath))
        {
            if (file.EndsWith(".meta")) continue;
            var item = CreateItem(file, Path.GetFileName(file), parent.depth + 1, false, ref id);
            parent.AddChild(item);
        }
    }

    /// <summary>
    /// FileItem 민들어주는 함수
    /// </summary>
    private FileItem CreateItem(string fullPath, string displayName, int depth, bool isFolder, ref int id)
    {
        fullPath = fullPath.Replace("\\", "/");
        var existing = _itemList.FirstOrDefault(i => i.fullPath == fullPath);
        if (existing != null) return existing;

        var newItem = new FileItem
        {
            id = id++,
            depth = depth,
            displayName = displayName,
            fullPath = fullPath,
            isFolder = isFolder,
            selected = false
        };
        _itemList.Add(newItem);
        return newItem;
    }

    protected override void RowGUI(RowGUIArgs args)
    {
        var item = (FileItem)args.item;
        float indent = GetContentIndent(item);
        Rect toggleRect = new Rect(args.rowRect.x + indent, args.rowRect.y, 18, args.rowRect.height);
        Rect iconRect = new Rect(toggleRect.xMax + 2, args.rowRect.y, 18, args.rowRect.height);
        Rect labelRect = new Rect(iconRect.xMax + 2, args.rowRect.y, args.rowRect.width - iconRect.xMax - 60, args.rowRect.height);

        EditorGUI.BeginChangeCheck();
        bool toggled = EditorGUI.Toggle(toggleRect, item.selected);
        if (toggled != item.selected)
        {
            item.selected = toggled;
            SetParentSelection(item, toggled);
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
    }

    public List<string> GetSelectedFilePathList()
    {
        return _itemList.Where(i => i.selected && !i.isFolder).Select(i => i.fullPath).ToList();
    }

    /// <summary>
    /// 자식오브젝트들 부모오브젝트에 맞춰 선택해주는 함수
    /// </summary>
    private void SetParentSelection(FileItem child, bool selected)
    {
        var current = child.parent;

        while (current != null)
        {
            if (selected is false && current.children.Any(child => (child as FileItem)?.selected == true))
            {
                break;
            }

            if (current is FileItem parentItem)
            {
                parentItem.selected = selected;
            }

            current = current.parent;
        }
    }

    /// <summary>
    /// 자식오브젝트들 부모오브젝트에 맞춰 선택해주는 함수
    /// </summary>
    private void SetChildrenSelection(FileItem parent, bool selected)
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

    /// <summary>
    /// 전체 선택 함수
    /// </summary>
    public void SelectAll()
    {
        foreach (var i in _itemList)
        {
            i.selected = true;
        }
    }

    /// <summary>
    /// 전체 선택해제 함수
    /// </summary>
    public void DeselectAll()
    {
        foreach (var i in _itemList)
        {
            i.selected = false;
        }
    }
}
