using UnityEngine;
using UnityEngine.Audio;

namespace RFG
{
  [CreateAssetMenu(fileName = "New Audio Data", menuName = "RFG/Audio/Audio Data")]
  public class AudioData : ScriptableObject
  {
    public AudioClip clip;
    public AudioMixerGroup outputAudioMixerGroup;

    [Range(0, 1)]
    public float volume = 1f;

    [Range(.25f, 3f)]
    public float pitch = 1f;

    public bool randomPitch = false;

    [Range(0f, 1f)]
    public float minPitch;
    [Range(0f, 1f)]
    public float maxPitch;


    public bool playOnAwake = false;
    public bool loop = false;
    public float fadeTime = 0f;

    [Range(0f, 1f)]
    public float spacialBlend = 1f;
    public float minDistance = 1f;
    public float maxDistance = 100f;
    public AudioRolloffMode rolloffMode;

  }
}