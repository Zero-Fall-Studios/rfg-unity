using UnityEngine;

namespace RFG
{
  [CreateAssetMenu(fileName = "New Knockback Data", menuName = "RFG/Platformer/Interactions/Knockback")]
  public class KnockbackData : ScriptableObject
  {
    [Header("Settings")]
    public float Damage;
    public Vector2 Velocity;
    public float Threshold = 0.5f;
    public LayerMask LayerMask;

    [Header("Effects")]
    public string[] Effects;
  }
}