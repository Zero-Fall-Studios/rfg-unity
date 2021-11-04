using System;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RFG.Platformer
{
  using Actions;

  [Serializable]
  [ActionMenu("Platformer/Character/Movement State")]
  public class MovementStateAction : Action
  {
    public Character character;
    public RFG.State State;

    public override State Run()
    {
      character.MovementState.ChangeState(State.GetType());
      return RFG.Actions.State.Success;
    }

#if UNITY_EDITOR
    public override void Draw(ActionNode node)
    {
      VisualElement container = node.Q<VisualElement>("container");
      IMGUIContainer guiContainer = new IMGUIContainer(() =>
      {
        character = (Character)EditorGUILayout.ObjectField("Character:", character, typeof(Character), true);
        State = (RFG.State)EditorGUILayout.ObjectField("Movement State:", State, typeof(RFG.State), true);
      });
      container.Add(guiContainer);
    }
#endif

  }
}