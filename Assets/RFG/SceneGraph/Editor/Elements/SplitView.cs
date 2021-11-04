using UnityEngine.UIElements;

namespace RFG.SceneGraph
{
  public class SplitView : TwoPaneSplitView
  {
    public new class UxmlFactory : UxmlFactory<SplitView, TwoPaneSplitView.UxmlTraits> { }
  }
}