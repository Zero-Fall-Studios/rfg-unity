using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RFG
{
  public static class ScriptableObjectEx
  {
    public static T CreateNewInstance<T>(this ScriptableObject obj) where T : ScriptableObject
    {
      Type type = obj.GetType();
      T newObj = (T)ScriptableObject.CreateInstance(type);
      newObj.name = type.ToString();
      return newObj;
    }

    public static T CloneInstance<T>(this ScriptableObject obj) where T : ScriptableObject
    {
      return ScriptableObject.Instantiate(obj) as T;
    }

    #region Save and Load
    public static void SaveData(this ScriptableObject obj, string path)
    {
      var fileName = $"{path}/{obj.name}.json";
      var data = JsonUtility.ToJson(obj);
      File.WriteAllText(fileName, data);
    }

    public static void LoadData(this ScriptableObject obj, string path)
    {
      var fileName = $"{path}/{obj.name}.json";
      if (File.Exists(fileName))
      {
        var json = File.ReadAllText(fileName);
        JsonUtility.FromJsonOverwrite(json, obj);
      }
    }
    #endregion

#if UNITY_EDITOR
    public static T CreateScriptableObject<T>(string path) where T : ScriptableObject
    {
      T asset = ScriptableObject.CreateInstance<T>();
      string assetType = asset.GetType().ToString();
      string name = asset.GetType().ToString().Last();
      AssetDatabase.CreateAsset(asset, $"{path}/{name}.asset");
      AssetDatabase.SaveAssets();
      EditorUtility.FocusProjectWindow();
      Selection.activeObject = asset;
      return asset;
    }
#endif

  }
}