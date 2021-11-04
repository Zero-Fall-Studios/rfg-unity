using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RFG.SceneGraph
{
  public class Scene : ScriptableObject
  {
    [HideInInspector] public string guid;
    [HideInInspector] public Vector2 position;
    [HideInInspector] public string scenePath;
    [HideInInspector] public List<SceneTransition> sceneTransitions = new List<SceneTransition>();
    public Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 10);

#if UNITY_EDITOR
    public void ChangeSceneName(string scenePath)
    {
      if (!this.scenePath.Equals(scenePath))
      {
        this.scenePath = scenePath;
        EditorUtility.SetDirty(this);
      }
    }
#endif

  }
}