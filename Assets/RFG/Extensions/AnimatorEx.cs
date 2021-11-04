using UnityEngine;

namespace RFG
{
  public static class AnimatorEx
  {
    public static void ResetCurrentClip(this Animator animator)
    {
      AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
      int stateName = currentState.fullPathHash;
      animator.Play(stateName, 0, 0.0f);
      animator.speed = 1;
    }

  }
}