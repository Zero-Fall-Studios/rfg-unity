using System;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RFG.Platformer
{
  using Actions;

  [Serializable]
  [ActionMenu("Platformer/Character/Character Controller Flip")]
  public class CharacterControllerFlipAction : Action
  {
    public CharacterController2D characterController;

    public override State Run()
    {
      characterController.Flip();
      return RFG.Actions.State.Success;
    }

#if UNITY_EDITOR
    public override void Draw(ActionNode node)
    {
      VisualElement container = node.Q<VisualElement>("container");
      IMGUIContainer guiContainer = new IMGUIContainer(() =>
      {
        characterController = (CharacterController2D)EditorGUILayout.ObjectField("Character Controller 2D :", characterController, typeof(CharacterController2D), true);
      });
      container.Add(guiContainer);
    }
#endif

  }
}