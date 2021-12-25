using System;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RFG.Actions
{
  [Serializable]
  [ActionMenu("Behaviour Tree/Change To Default Behaviour Tree")]
  public class CharacterToDefaultBehaviourTreeAction : RFG.Actions.Action
  {
    public RFG.BehaviourTree.BehaviourTreeRunner runner;

    public override State Run()
    {
      runner.ChangeToDefaultTree();
      return RFG.Actions.State.Success;
    }

#if UNITY_EDITOR
    public override void Draw(ActionNode node)
    {
      VisualElement container = node.Q<VisualElement>("container");
      IMGUIContainer guiContainer = new IMGUIContainer(() =>
      {
        runner = (RFG.BehaviourTree.BehaviourTreeRunner)EditorGUILayout.ObjectField("Behaviour Tree Runner:", runner, typeof(RFG.BehaviourTree.BehaviourTreeRunner), true);
      });
      container.Add(guiContainer);
    }
#endif

  }
}