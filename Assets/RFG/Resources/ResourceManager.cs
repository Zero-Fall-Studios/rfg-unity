using System;
using System.Collections.Generic;
using UnityEngine;

namespace RFG
{
  [Serializable]
  public class ResourceAsset
  {
    public string Guid;
    public ScriptableObject Asset;
  }

  public interface IResourceIndex
  {
    List<ResourceAsset> GetAssets();
  }

  [AddComponentMenu("RFG/Resource Manager/Resource Manager")]
  public class ResourceManager : PersistentSingleton<ResourceManager>
  {
    public ScriptableObject ResourceIndex;
    public Dictionary<string, ScriptableObject> HashTable;

    protected override void Awake()
    {
      base.Awake();
      HashTable = new Dictionary<string, ScriptableObject>();
      IResourceIndex index = (IResourceIndex)ResourceIndex;
      foreach (ResourceAsset item in index.GetAssets())
      {
        HashTable.Add(item.Guid, item.Asset);
      }
    }

  }

  public static class ResourceManagerEx
  {
    public static ScriptableObject FindObject(this string guid)
    {
      if (guid != null && !guid.Equals("") && ResourceManager.Instance.HashTable.ContainsKey(guid))
      {
        return ResourceManager.Instance.HashTable[guid];
      }
      return null;
    }

    public static List<T> FindObjects<T>(this string[] guids) where T : ScriptableObject
    {
      List<T> objs = new List<T>();
      foreach (string guid in guids)
      {
        T value = (T)guid.FindObject();
        if (value != null)
        {
          objs.Add(value);
        }
      }
      return objs;
    }

    public static string FindGuid(this ScriptableObject obj)
    {
      foreach (KeyValuePair<string, ScriptableObject> keyValuePair in ResourceManager.Instance.HashTable)
      {
        if (keyValuePair.Value.Equals(obj))
        {
          return keyValuePair.Key;
        }
      }
      return null;
    }

    public static string[] FindGuids<T>(this List<T> objs) where T : ScriptableObject
    {
      List<string> guids = new List<string>();
      foreach (T obj in objs)
      {
        string guid = obj.FindGuid();
        if (guid != null)
        {
          guids.Add(guid);
        }
      }
      return guids.ToArray();
    }
  }
}
