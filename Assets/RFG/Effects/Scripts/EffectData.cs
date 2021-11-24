using System.Collections.Generic;
using UnityEngine;

namespace RFG
{
  [CreateAssetMenu(fileName = "New Effect Data", menuName = "RFG/Effects/Effect Data")]
  public class EffectData : ScriptableObject
  {
    [Header("Settings")]
    public float lifetime = 3f;
    public bool pooledObject = true;

    [Header("Animations")]
    public string animationClip;

    [Header("Audio")]
    public List<AudioData> soundEffects;

    [Header("Effects")]
    public string[] spawnEffects;

    [Header("Camera Shake")]
    public float cameraShakeIntensity = 0f;
    public float cameraShakeTime = 0f;
    public bool cameraShakeFade = false;

    [Header("Spawn Offset")]
    public Vector2 spawnOffset;
    public bool invertX = false;
    public bool invertY = false;
  }
}