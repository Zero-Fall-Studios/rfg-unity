using UnityEngine;

namespace RFG
{
  public static class WaitUntilEx
  {
    public static WaitUntil GameObjectNotNull(string name)
    {
      return new WaitUntil(() => GameObject.Find(name) != null);
    }

    public static WaitUntil GameObjectsActive<T>() where T : UnityEngine.Object
    {
      return new WaitUntil(() =>
      {
        T[] objs = GameObject.FindObjectsOfType<T>();
        foreach (T obj in objs)
        {
          if (obj is GameObject gameObject)
          {
            if (!gameObject.activeInHierarchy)
            {
              return false;
            }
          }
        }
        return true;
      });
    }
  }
}