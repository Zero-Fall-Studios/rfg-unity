using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RFG.SceneGraph
{
  [CustomEditor(typeof(SceneDoor))]
  [CanEditMultipleObjects]
  public class SceneDoorInspector : Editor
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
      SceneDoor sceneDoor = (SceneDoor)target;
      EditorGUILayout.Space();

      Scene currentScene = SceneGraphManager.Instance.CurrentScene;

      if (currentScene == null)
        return;

      List<string> options = new List<string>();
      Dictionary<string, SceneTransition> optionsSceneTransitions = new Dictionary<string, SceneTransition>();

      int selected = 0;
      SceneTransition selectedTransition;


      for (int i = 0; i < currentScene.sceneTransitions.Count; i++)
      {
        SceneTransition sceneTransition = currentScene.sceneTransitions[i];
        if (sceneTransition.to != null)
        {
          string option = sceneTransition.to.parent.scenePath;
          options.Add(option);
          optionsSceneTransitions.Add(option, sceneTransition);
        }

        if (sceneTransition.from != null)
        {
          string option = sceneTransition.from.parent.scenePath;
          options.Add(option);
          optionsSceneTransitions.Add(option, sceneTransition);
        }
      }

      if (sceneDoor.sceneTransition != null)
      {
        for (int i = 0; i < options.Count; i++)
        {
          if (sceneDoor.sceneTransition.from != null && sceneDoor.sceneTransition.from.parent != null && options[i].Equals(sceneDoor.sceneTransition.from.parent.scenePath))
          {
            selected = i;
          }
          else if (sceneDoor.sceneTransition.to != null && sceneDoor.sceneTransition.to.parent != null && options[i].Equals(sceneDoor.sceneTransition.to.parent.scenePath))
          {
            selected = i;
          }
        }
      }

      selected = EditorGUILayout.Popup("Scene Path", selected, options.ToArray());

      for (int i = 0; i < options.Count; i++)
      {
        if (i == selected)
        {
          selectedTransition = optionsSceneTransitions[options[i]];
          sceneDoor.sceneTransition = selectedTransition;
          if (sceneDoor.sceneTransition.from != null && options[i].Equals(sceneDoor.sceneTransition.from.parent.scenePath))
          {
            sceneDoor.fromTo = 0;
          }
          else if (sceneDoor.sceneTransition.to != null && options[i].Equals(sceneDoor.sceneTransition.to.parent.scenePath))
          {
            sceneDoor.fromTo = 1;
          }
          EditorUtility.SetDirty(sceneDoor);
        }
      }
    }

  }
}