using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Behaviours/Health")]
  public class HealthBehaviour : MonoBehaviour
  {
    [Header("Settings")]
    public FloatReference HealthReference;
    public FloatReference MaxHealthReference;
    public float Health = 100f;
    public float MaxHealth = 100f;
    public float DefaultMaxHealth = 100f;

    [Header("Game Events")]
    public GameEvent KillEvent;

    private Character _character;

    private void Awake()
    {
      _character = GetComponent<Character>();
      if (HealthReference != null)
      {
        Health = HealthReference.Value;
      }
      if (MaxHealthReference != null)
      {
        MaxHealthReference.Value = DefaultMaxHealth;
        MaxHealth = MaxHealthReference.Value;
      }
    }

    public void SetHealth(float amount)
    {
      if (amount >= MaxHealth)
      {
        amount = MaxHealth;
      }
      Health = amount;
      if (HealthReference != null)
      {
        HealthReference.Value = Health;
      }
      if (Health <= 0)
      {
        KillEvent?.Raise();
        _character.Kill();
      }
    }

    public void AddHealth(float amount)
    {
      SetHealth(Health += amount);
    }

    public void TakeDamage(float damage)
    {
      SetHealth(Health - damage);
    }

    public void ResetHealth()
    {
      SetHealth(MaxHealth);
    }

    public void AddMaxHealth(float amount)
    {
      MaxHealth += amount;
      if (MaxHealthReference != null)
      {
        MaxHealthReference.Value = MaxHealth;
      }
      ResetHealth();
    }
  }
}
