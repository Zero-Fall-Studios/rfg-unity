using System.Collections.Generic;
using UnityEngine;

namespace RFG
{
  [CreateAssetMenu(fileName = "New Playlist Data", menuName = "RFG/Audio/Playlist Data")]
  public class PlaylistData : ScriptableObject
  {
    public List<AudioData> audioList;
    public bool loop = false;
    public float waitForSeconds = 1f;
    public float fadeTime = 1f;
    public int currentIndex = 0;
    public bool playOnAwake = false;

    public void Initialize()
    {
      currentIndex = 0;
    }

    public AudioData GetCurrent()
    {
      return audioList[currentIndex];
    }

    public bool IsLast()
    {
      return currentIndex == audioList.Count - 1;
    }

    public bool IsFirst()
    {
      return currentIndex == 0;
    }

    public void Next()
    {
      int nextIndex = currentIndex + 1;
      if (nextIndex > audioList.Count - 1)
      {
        nextIndex = 0;
      }
      currentIndex = nextIndex;
    }

    public void Previous()
    {
      int nextIndex = currentIndex - 1;
      if (nextIndex < 0)
      {
        nextIndex = audioList.Count - 1;
      }
      currentIndex = nextIndex;
    }
  }
}