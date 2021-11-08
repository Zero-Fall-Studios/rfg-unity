using System.Collections.Generic;
using UnityEngine;

namespace RFG
{
  public static class GameObjectEx
  {

    static List<Component> m_ComponentCache = new List<Component>();

    public static bool CompareTags(this GameObject gameObject, string[] tags)
    {
      for (int i = 0; i < tags.Length; i++)
      {
        if (gameObject.CompareTag(tags[i]))
        {
          return true;
        }
      }
      return false;
    }

    public static bool CompareTags(this GameObject gameObject, List<string> tags)
    {
      foreach (string tag in tags)
      {
        if (gameObject.CompareTag(tag))
        {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Grabs a component without allocating memory uselessly
    /// </summary>
    /// <param name="this"></param>
    /// <param name="componentType"></param>
    /// <returns></returns>
    public static Component GetComponentNoAlloc(this GameObject @this, System.Type componentType)
    {
      @this.GetComponents(componentType, m_ComponentCache);
      Component component = m_ComponentCache.Count > 0 ? m_ComponentCache[0] : null;
      m_ComponentCache.Clear();
      return component;
    }

    /// <summary>
    /// Grabs a component without allocating memory uselessly
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="this"></param>
    /// <returns></returns>
    public static T GetComponentNoAlloc<T>(this GameObject @this) where T : Component
    {
      @this.GetComponents(typeof(T), m_ComponentCache);
      Component component = m_ComponentCache.Count > 0 ? m_ComponentCache[0] : null;
      m_ComponentCache.Clear();
      return component as T;
    }

    public static T GetOrAddComponent<T>(this GameObject @this) where T : Component
    {
      return (@this.GetComponent<T>() == null) ? @this.AddComponent<T>() : @this.GetComponent<T>();
    }

  }
}