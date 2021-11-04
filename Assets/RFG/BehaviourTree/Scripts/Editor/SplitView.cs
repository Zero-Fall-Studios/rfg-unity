using UnityEngine.UIElements;

namespace RFG.BehaviourTree
{
  public class SplitView : TwoPaneSplitView
  {
    public new class UxmlFactory : UxmlFactory<SplitView, TwoPaneSplitView.UxmlTraits> { }
  }
}