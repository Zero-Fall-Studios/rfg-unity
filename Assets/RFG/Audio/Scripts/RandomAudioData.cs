using System.Collections.Generic;
using UnityEngine;

namespace RFG
{
  [CreateAssetMenu(fileName = "New Random Audio Data", menuName = "RFG/Audio/Random Audio Data")]
  public class RandomAudioData : ScriptableObject
  {
    public List<AudioData> audioList;
    public float waitForSeconds = 3f;
    public float minDistance = 30f;
    public float maxDistance = 35f;
  }
}