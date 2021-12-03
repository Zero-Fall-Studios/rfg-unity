using System;
using System.Collections.Generic;
using UnityEngine;

namespace RFG
{
  [Serializable]
  public struct AnimationItem
  {
    public string name;
    public int frames;
  }

  [CreateAssetMenu(fileName = "New Animation Slice", menuName = "RFG/Animation/Animation Slice")]
  public class AnimationSlice : ScriptableObject
  {
    public Vector2 cellSize = new Vector2(16, 16);
    public SpriteAlignment alignment = SpriteAlignment.BottomCenter;
    public List<AnimationItem> animations = new List<AnimationItem>();
  }
}