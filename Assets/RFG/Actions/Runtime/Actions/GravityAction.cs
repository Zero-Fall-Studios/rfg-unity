using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace RFG.Actions
{
  [Serializable]
  [ActionMenu("Rigid Body/Gravity")]
  public class GravityAction : Action
  {
    public Rigidbody2D rigidbody2D;
    public int gravityScale;

    public override State Run()
    {
      rigidbody2D.gravityScale = gravityScale;
      return State.Success;
    }

#if UNITY_EDITOR
    public override void Draw(ActionNode node)
    {
      VisualElement container = node.Q<VisualElement>("container");

      IMGUIContainer guiContainer = new IMGUIContainer(() =>
      {
        rigidbody2D = (Rigidbody2D)EditorGUILayout.ObjectField("RigigBody 2D:", rigidbody2D, typeof(Rigidbody2D), true);
        gravityScale = EditorGUILayout.IntField("Gravity scale:", gravityScale);
      });
      container.Add(guiContainer);
    }
#endif

  }
}