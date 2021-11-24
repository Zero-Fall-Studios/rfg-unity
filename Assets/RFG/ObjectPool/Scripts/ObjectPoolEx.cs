using System.Collections.Generic;
using UnityEngine;

namespace RFG
{
  public static class ObjectPoolEx
  {
    public static List<GameObject> SpawnFromPool(this Vector2 position, Vector2 rotation, string[] tags, params object[] objects)
    {
      if (tags != null && tags.Length > 0)
      {
        List<GameObject> spawnedObjects = new List<GameObject>();
        foreach (string tag in tags)
        {
          GameObject spawned = ObjectPool.Instance.SpawnFromPool(tag, position, Quaternion.Euler(rotation.x, rotation.y, 0), objects);
          spawnedObjects.Add(spawned);
        }
        return spawnedObjects;
      }
      return null;
    }

    public static List<GameObject> SpawnFromPool(this Vector3 position, Vector3 rotation, string[] tags, params object[] objects)
    {
      if (tags != null && tags.Length > 0)
      {
        List<GameObject> spawnedObjects = new List<GameObject>();
        foreach (string tag in tags)
        {
          GameObject spawned = ObjectPool.Instance.SpawnFromPool(tag, position, Quaternion.Euler(rotation.x, rotation.y, rotation.z), objects);
          spawnedObjects.Add(spawned);
        }
        return spawnedObjects;
      }
      return null;
    }

    public static List<GameObject> SpawnFromPool(this Transform transform, string[] tags, params object[] objects)
    {
      if (tags != null && tags.Length > 0)
      {
        List<GameObject> spawnedObjects = new List<GameObject>();
        foreach (string tag in tags)
        {
          GameObject spawned = ObjectPool.Instance.SpawnFromPool(tag, transform.position, transform.rotation, objects);
          spawnedObjects.Add(spawned);
        }
        return spawnedObjects;
      }
      return null;
    }

    public static List<GameObject> SpawnFromPool(this Transform transform, string[] tags, Quaternion rotation, params object[] objects)
    {
      if (tags != null && tags.Length > 0)
      {
        List<GameObject> spawnedObjects = new List<GameObject>();
        foreach (string tag in tags)
        {
          GameObject spawned = ObjectPool.Instance.SpawnFromPool(tag, transform.position, rotation, objects);
          spawnedObjects.Add(spawned);
        }
        return spawnedObjects;
      }
      return null;
    }

    public static void DeactivatePoolByTag(this Transform transform, string[] tags)
    {
      if (tags != null && tags.Length > 0)
      {
        foreach (string tag in tags)
        {
          ObjectPool.Instance.DeactivateAllByTag(tag);
        }
      }
    }
  }
}