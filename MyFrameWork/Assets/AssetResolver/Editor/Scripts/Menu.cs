using UnityEditor;
using UnityEngine;

namespace Common.AssetResolver
{
    public class Menu
    {
        [MenuItem("Tools/Asset Resolver/Show Asset Manager")]
        static void ShowManager()
        {
            Selection.activeObject = Resources.Load("Asset Manager");

            if (Selection.activeObject == null)
                Debug.Log("Make sure you have 'Asset Manager' file in Resources folder.");
        }
    }
}
