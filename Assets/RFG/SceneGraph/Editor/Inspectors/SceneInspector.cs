using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RFG.SceneGraph
{
  [CustomEditor(typeof(Scene))]
  [CanEditMultipleObjects]
  public class SceneInspector : Editor
  {
    public override void OnInspectorGUI()
    {
      serializedObject.Update();

      DrawDefaultInspector();

      SceneDropdown();

      serializedObject.ApplyModifiedProperties();
    }

    private void SceneDropdown()
    {
      Scene scene = (Scene)target;
      EditorGUILayout.Space();

      List<string> options = GetAllScenes();
      int selected = options.FindIndex(s => s.Equals(scene.scenePath));
      if (selected == -1)
      {
        selected = 0;
      }
      selected = EditorGUILayout.Popup("Scene", selected, options.ToArray());
      scene.ChangeSceneName(options[selected]);

      if (GUILayout.Button("Load Scene"))
      {
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scene.scenePath);
      }
    }

    private List<string> GetAllScenes()
    {
      var list = new List<string>();

      for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; ++i)
      {
        // string name = System.IO.Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i));
        string name = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
        list.Add(name);
      }

      return list;
    }

  }
}