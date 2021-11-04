using System;

namespace RFG.SceneGraph
{
  using Actions;

  [Serializable]
  [ActionMenu("Scene Graph/Load Next Scene")]
  public class LoadNextSceneAction : Action
  {
    public override State Run()
    {
      SceneGraphManager.Instance.LoadNextScene();
      return State.Success;
    }
  }
}