using System;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RFG.Platformer
{
  using Actions;

  [Serializable]
  [ActionMenu("Platformer/Character/Character Player Input")]
  public class CharacterPlayerInputAction : Action
  {
    public Character character;
    public bool enable;

    public override State Run()
    {
      if (character.IsReady)
      {
        character.EnableAllInput(enable);
        return RFG.Actions.State.Success;
      }
      return RFG.Actions.State.Running;
    }

#if UNITY_EDITOR
    public override void Draw(ActionNode node)
    {
      VisualElement container = node.Q<VisualElement>("container");
      IMGUIContainer guiContainer = new IMGUIContainer(() =>
      {
        character = (Character)EditorGUILayout.ObjectField("Character:", character, typeof(Character), true);
        enable = EditorGUILayout.Toggle("Enable:", enable);
      });
      container.Add(guiContainer);
    }
#endif

  }
}