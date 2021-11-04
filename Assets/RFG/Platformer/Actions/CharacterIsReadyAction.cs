using System;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RFG.Platformer
{
  using Actions;

  [Serializable]
  [ActionMenu("Platformer/Character/Character Is Ready")]
  public class CharacterReadyAction : Action
  {
    public Character character;

    public override State Run()
    {
      if (character.IsReady)
      {
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
      });
      container.Add(guiContainer);
    }
#endif

  }
}