using System;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RFG
{
  using Actions;

  public enum SpriteArrayActionType
  {
    Next,
    Previous
  }

  [Serializable]
  [ActionMenu("Sprite/Sprite Array")]
  public class SpriteArrayAction : Action
  {
    public SpriteArray spriteArray;
    public SpriteArrayActionType SpriteArrayActionType;

    public override RFG.Actions.State Run()
    {

      switch (SpriteArrayActionType)
      {
        case SpriteArrayActionType.Next:
          spriteArray.Next();
          break;
        case SpriteArrayActionType.Previous:
          spriteArray.Previous();
          break;
      }
      return RFG.Actions.State.Success;
    }

#if UNITY_EDITOR
    public override void Draw(ActionNode node)
    {
      VisualElement container = node.Q<VisualElement>("container");
      IMGUIContainer guiContainer = new IMGUIContainer(() =>
      {
        spriteArray = (SpriteArray)EditorGUILayout.ObjectField("Sprite Array:", spriteArray, typeof(SpriteArray), true);
        SpriteArrayActionType = (SpriteArrayActionType)EditorGUILayout.EnumPopup("Action:", SpriteArrayActionType);
      });
      container.Add(guiContainer);
    }
#endif

  }
}