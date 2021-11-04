using UnityEngine;
using UnityEngine.Audio;

namespace RFG
{
  [CreateAssetMenu(fileName = "New Audio Mixer Settings", menuName = "RFG/Audio/Audio Mixer Settings")]
  public class AudioMixerSettings : ScriptableObject
  {
    public AudioMixer AudioMixer;
    public float Volume;

    public void Initialize()
    {
      SetVolume(Volume);
    }

    public void SetVolume(float volume)
    {
      if (volume < 0.001f)
      {
        volume = 0.001f;
      }
      Volume = volume;
      AudioMixer.SetFloat("Volume", Mathf.Log(Volume) * 20);
    }

    public float GetVolume()
    {
      return Volume;
    }
  }
}