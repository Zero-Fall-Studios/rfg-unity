using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace RFG
{
  public class EditorUtils
  {
    public static void AddLayers(string[] layerNames)
    {
      SerializedObject manager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
      SerializedProperty layersProp = manager.FindProperty("layers");

      foreach (string name in layerNames)
      {
        // check if layer is present
        bool found = false;
        for (int i = 0; i <= 31; i++)
        {
          SerializedProperty sp = layersProp.GetArrayElementAtIndex(i);
          if (sp != null && name.Equals(sp.stringValue))
          {
            found = true;
            break;
          }
        }

        // not found, add into 1st open slot
        if (!found)
        {
          SerializedProperty slot = null;
          for (int i = 8; i <= 31; i++)
          {
            SerializedProperty sp = layersProp.GetArrayElementAtIndex(i);
            if (sp != null && string.IsNullOrEmpty(sp.stringValue))
            {
              slot = sp;
              break;
            }
          }

          if (slot != null)
          {
            slot.stringValue = name;
          }
        }
      }

      // save
      manager.ApplyModifiedProperties();
    }

    public static void AddTags(string[] tagNames)
    {
      SerializedObject manager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
      SerializedProperty tagsProp = manager.FindProperty("tags");

      List<string> DefaultTags = new List<string>() { "Untagged", "Respawn", "Finish", "EditorOnly", "MainCamera", "Player", "GameController" };

      foreach (string name in tagNames)
      {
        if (DefaultTags.Contains(name)) continue;

        // check if tag is present
        bool found = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
          SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
          if (t.stringValue.Equals(name)) { found = true; break; }
        }

        // if not found, add it
        if (!found)
        {
          tagsProp.InsertArrayElementAtIndex(0);
          SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
          n.stringValue = name;
        }
      }

      // save
      manager.ApplyModifiedProperties();
    }

    public static void AddSortingLayers(string[] layers)
    {
      SerializedObject manager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
      SerializedProperty sortLayersProp = manager.FindProperty("m_SortingLayers");

      foreach (string name in layers)
      {
        // check if tag is present
        bool found = false;
        for (int i = 0; i < sortLayersProp.arraySize; i++)
        {
          SerializedProperty entry = sortLayersProp.GetArrayElementAtIndex(i);
          SerializedProperty t = entry.FindPropertyRelative("name");
          if (t.stringValue.Equals(name)) { found = true; break; }
        }

        // if not found, add it
        if (!found)
        {
          manager.ApplyModifiedProperties();
          AddSortingLayer();
          manager.Update();

          int idx = sortLayersProp.arraySize - 1;
          SerializedProperty entry = sortLayersProp.GetArrayElementAtIndex(idx);
          SerializedProperty t = entry.FindPropertyRelative("name");
          t.stringValue = name;
        }
      }

      // save
      manager.ApplyModifiedProperties();
    }

    // you need 'using System.Reflection;' for these
    private static Assembly editorAsm;
    private static MethodInfo AddSortingLayer_Method;

    /// <summary> add a new sorting layer with default name </summary>
    public static void AddSortingLayer()
    {
      if (AddSortingLayer_Method == null)
      {
        if (editorAsm == null) editorAsm = Assembly.GetAssembly(typeof(Editor));
        System.Type t = editorAsm.GetType("UnityEditorInternal.InternalEditorUtility");
        AddSortingLayer_Method = t.GetMethod("AddSortingLayer", (BindingFlags.Static | BindingFlags.NonPublic), null, new System.Type[0], null);
      }
      AddSortingLayer_Method.Invoke(null, null);
    }

    public static GameObject CreatePrefab(string path, string objName)
    {
      string fullPath = $"{path}/{objName}.prefab";
      Debug.Log($"Creating Prefab at path {fullPath}");
      UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(fullPath, typeof(GameObject));
      GameObject clone = PrefabUtility.InstantiatePrefab(obj) as GameObject;
      if (clone == null)
      {
        Debug.Log("Prefab returned null");
        return null;
      }
      Selection.activeObject = clone;
      clone.name = objName;
      return clone;
    }

    public static T CreateScriptableObject<T>(string path, string name = "") where T : ScriptableObject
    {
      T asset = ScriptableObject.CreateInstance<T>();
      string assetType = asset.GetType().ToString();
      if (string.IsNullOrEmpty(name))
      {
        name = asset.GetType().ToString().Last();
      }
      AssetDatabase.CreateAsset(asset, $"{path}/{name}.asset");
      AssetDatabase.SaveAssets();
      EditorUtility.FocusProjectWindow();
      Selection.activeObject = asset;
      return asset;
    }

    public static bool TryGetActiveFolderPath(out string path)
    {
      var _tryGetActiveFolderPath = typeof(ProjectWindowUtil).GetMethod("TryGetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
      object[] args = new object[] { null };
      bool found = (bool)_tryGetActiveFolderPath.Invoke(null, args);
      path = (string)args[0];
      return found;
    }

    public static string GetDirName(string name)
    {
      var g = AssetDatabase.FindAssets($"t:Script {name}");
      if (g == null || g.Length == 0)
      {
        Debug.Log($"t:Script {name} - Not Found");
        return null;

      }
      return AssetDatabase.GUIDToAssetPath(g[0]).BeforeLast("/");
    }

    public static T LoadAssetAtDir<T>(string dirName, string name) where T : UnityEngine.Object
    {
      string path = EditorUtils.GetDirName(dirName);
      return AssetDatabase.LoadAssetAtPath<T>($"{path}/{name}");
    }

    public static string CreateFolderStructure(string folderName, params string[] subfolderNames)
    {
      string path;
      if (EditorUtils.TryGetActiveFolderPath(out path))
      {
        string guid = AssetDatabase.CreateFolder(path, folderName);
        string newFolderPath = AssetDatabase.GUIDToAssetPath(guid);

        foreach (string subfolderName in subfolderNames)
        {
          AssetDatabase.CreateFolder(newFolderPath, subfolderName);
        }

        AssetDatabase.SaveAssets();
        return newFolderPath;
      }

      return null;
    }

    public static GameObject SaveAsPrefabAsset(GameObject obj, string folderPath, string name)
    {
      return PrefabUtility.SaveAsPrefabAsset(obj, $"{folderPath}/Prefabs/{name}.prefab");
    }
  }
}
