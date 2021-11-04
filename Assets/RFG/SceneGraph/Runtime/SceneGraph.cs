using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RFG.SceneGraph
{
  [CreateAssetMenu(fileName = "New Scene Graph", menuName = "RFG/Scene Graph/Scene Graph")]
  public class SceneGraph : ScriptableObject
  {
    [HideInInspector] public List<Scene> scenes = new List<Scene>();

#if UNITY_EDITOR

    #region Scenes
    public Scene CreateScene(string title, Vector2 position)
    {
      Scene scene = ScriptableObject.CreateInstance(typeof(Scene)) as Scene;
      scene.name = title;
      scene.scenePath = title;
      scene.position = position;
      scene.guid = GUID.Generate().ToString();

      Undo.RecordObject(this, "Scene Graph (CreateScene)");
      scenes.Add(scene);

      if (!Application.isPlaying)
      {
        AssetDatabase.AddObjectToAsset(scene, this);
      }
      Undo.RegisterCreatedObjectUndo(scene, "Scene Graph (CreateScene)");
      AssetDatabase.SaveAssets();

      return scene;
    }

    public void DeleteScene(Scene scene)
    {
      Undo.RecordObject(this, "Scene Graph (DeleteScene)");
      scenes.Remove(scene);
      AssetDatabase.RemoveObjectFromAsset(scene);
      Undo.DestroyObjectImmediate(scene);
      AssetDatabase.SaveAssets();
    }
    #endregion

    #region Scene Transition

    public SceneTransition CreateSceneTransition(Scene scene, string title, Vector2 position)
    {
      SceneTransition sceneTransition = ScriptableObject.CreateInstance(typeof(SceneTransition)) as SceneTransition;
      sceneTransition.name = title;
      sceneTransition.position = position;
      sceneTransition.guid = GUID.Generate().ToString();
      sceneTransition.parent = scene;

      Undo.RecordObject(this, "Scene Graph (CreateSceneTransition)");
      scene.sceneTransitions.Add(sceneTransition);

      if (!Application.isPlaying)
      {
        AssetDatabase.AddObjectToAsset(sceneTransition, this);
      }
      Undo.RegisterCreatedObjectUndo(sceneTransition, "Scene Graph (CreateSceneTransition)");
      AssetDatabase.SaveAssets();

      return sceneTransition;
    }

    public void DeleteSceneTransition(Scene scene, SceneTransition sceneTransition)
    {
      Undo.RecordObject(this, "Scene Graph (DeleteSceneTransition)");
      scene.sceneTransitions.Remove(sceneTransition);
      AssetDatabase.RemoveObjectFromAsset(sceneTransition);
      Undo.DestroyObjectImmediate(sceneTransition);
      AssetDatabase.SaveAssets();
    }

    #endregion

    #region Edges
    public void RemoveConnection(SceneTransition from, SceneTransition to)
    {
      Undo.RecordObject(from, "Scene Graph (Remove From)");
      Undo.RecordObject(to, "Scene Graph (Remove To)");
      from.to = null;
      to.from = null;
      EditorUtility.SetDirty(from);
      EditorUtility.SetDirty(to);
    }
    #endregion
#endif

    #region Utilities
    public Scene FindScene(string scenePath)
    {
      return scenes.Find(s => s.scenePath.Equals(scenePath));
    }

    public SceneTransition FindSceneTransitionByGuid(string guid)
    {
      foreach (Scene scene in scenes)
      {
        foreach (SceneTransition sceneTransition in scene.sceneTransitions)
        {
          if (sceneTransition.guid.Equals(guid))
          {
            return sceneTransition;
          }
        }
      }
      return null;
    }
    #endregion

  }
}