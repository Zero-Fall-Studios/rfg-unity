using UnityEngine;

namespace RFG
{
  public static class EffectEx
  {
    public static void ConfigureAudioSource(this AudioData audioData, AudioSource source)
    {
      source.clip = audioData.clip;
      source.outputAudioMixerGroup = audioData.outputAudioMixerGroup;
      source.playOnAwake = audioData.playOnAwake;
      source.loop = audioData.loop;
      source.volume = audioData.volume;
      source.spatialBlend = audioData.spacialBlend;
      source.pitch = audioData.pitch;
      source.minDistance = audioData.minDistance;
      source.maxDistance = audioData.maxDistance;
      source.rolloffMode = audioData.rolloffMode;
    }
  }
}