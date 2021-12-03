using System;
using System.Collections.Generic;
using UnityEngine;

namespace RFG
{
  [Serializable]
  public struct AnimationClipItem
  {
    public string name;
    public int framesStart;
    public int framesEnd;
  }

  [CreateAssetMenu(fileName = "New Animation Clips", menuName = "RFG/Animation/Animation Clips")]
  public class AnimationClips : ScriptableObject
  {
    public List<AnimationClipItem> clips = new List<AnimationClipItem>();
  }
}