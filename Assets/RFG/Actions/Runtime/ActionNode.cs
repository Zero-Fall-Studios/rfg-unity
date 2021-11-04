#if UNITY_EDITOR
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;

namespace RFG.Actions
{
  public class ActionNode : Node
  {
    public ActionList actionList;
    public Action action;
    public Port input;
    public Port output;

    public ActionNode(ActionList actionList, Action action) : base("Assets/RFG/Actions/UIBuilder/ActionNode.uxml")
    {
      this.actionList = actionList;
      this.action = action;
      this.title = action.title;
      this.viewDataKey = action.guid;
      style.left = action.position.x;
      style.top = action.position.y;
      SetPosition(new Rect(action.position, Vector2.zero));
    }

    public override void SetPosition(Rect newPos)
    {
      base.SetPosition(newPos);
      action.position.x = newPos.xMin;
      action.position.y = newPos.yMin;
      EditorUtility.SetDirty(actionList);
    }

    public void Draw()
    {
      action.Draw(this);
      action.DrawBase(this);
    }

  }
}
#endif