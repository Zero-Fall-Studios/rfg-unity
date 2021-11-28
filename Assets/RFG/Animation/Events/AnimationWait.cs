using UnityEngine;

namespace RFG
{
  public class AnimationWait : MonoBehaviour
  {
    [field: SerializeField] private GameEvent AnimationWaitEvent { get; set; }
    [field: SerializeField] private GameEvent AnimationDoneEvent { get; set; }

    public void AnimationWaitRaise()
    {
      AnimationWaitEvent?.Raise();
    }

    public void AnimationDoneRaise()
    {
      AnimationDoneEvent?.Raise();
    }
  }
}