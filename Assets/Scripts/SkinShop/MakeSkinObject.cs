using UnityEngine;
#if UNITY_EDITOR 
using UnityEditor;
#endif


public class MakeSkinObject
{
#if UNITY_EDITOR
    [MenuItem("Assets/Create/Skin Object")]
    public static void Create()
    {
        SkinObject asset = ScriptableObject.CreateInstance<SkinObject>();
        AssetDatabase.CreateAsset(asset, "Assets/NewSkinObject.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
#endif
}