using System;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RFG.Actions
{
  [Serializable]
  [ActionMenu("Behaviour Tree/Change Behaviour Tree")]
  public class CharacterBehaviourTreeAction : RFG.Actions.Action
  {
    public RFG.BehaviourTree.BehaviourTreeRunner runner;
    public RFG.BehaviourTree.BehaviourTree tree;

    public override State Run()
    {
      runner.ChangeTree(tree);
      return RFG.Actions.State.Success;
    }

#if UNITY_EDITOR
    public override void Draw(ActionNode node)
    {
      VisualElement container = node.Q<VisualElement>("container");
      IMGUIContainer guiContainer = new IMGUIContainer(() =>
      {
        runner = (RFG.BehaviourTree.BehaviourTreeRunner)EditorGUILayout.ObjectField("Behaviour Tree Runner:", runner, typeof(RFG.BehaviourTree.BehaviourTreeRunner), true);
        tree = (RFG.BehaviourTree.BehaviourTree)EditorGUILayout.ObjectField("Behaviour Tree:", tree, typeof(RFG.BehaviourTree.BehaviourTree), true);
      });
      container.Add(guiContainer);
    }
#endif

  }
}