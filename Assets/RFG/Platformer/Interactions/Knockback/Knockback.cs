using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Interactions/Knockback")]
  public class Knockback : MonoBehaviour
  {
    public KnockbackData KnockbackData;

    public Vector2 GetKnockbackVelocity(Vector2 target1, Vector2 target2)
    {
      Vector2 dir = (target1 - target2).normalized;
      if (dir.x > -KnockbackData.Threshold && dir.x < KnockbackData.Threshold)
      {
        dir.x = KnockbackData.Threshold;
      }
      if (dir.y > -KnockbackData.Threshold && dir.y < KnockbackData.Threshold)
      {
        dir.y = KnockbackData.Threshold;
      }
      return dir * KnockbackData.Velocity;
    }

    private void PerformKnockback(GameObject other)
    {
      if (KnockbackData.LayerMask.Contains(other.layer))
      {
        transform.SpawnFromPool(KnockbackData.Effects);
        if (!KnockbackData.Velocity.Equals(Vector2.zero))
        {
          IPhysics2D body = other.transform.gameObject.GetComponent(typeof(IPhysics2D)) as IPhysics2D;
          if (body != null)
          {
            Vector2 velocity = GetKnockbackVelocity(other.transform.position, transform.position);

            CharacterController2D otherController = other.GetComponent<CharacterController2D>();

            if (otherController != null && otherController.Parameters.Weight > 0)
            {
              velocity /= otherController.Parameters.Weight;
            }

            body.SetForce(velocity);
          }
        }
        if (KnockbackData.Damage > 0f)
        {
          HealthBehaviour health = other.GetComponent<HealthBehaviour>();
          if (health != null)
          {
            health.TakeDamage(KnockbackData.Damage);
          }
        }
      }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
      PerformKnockback(col.gameObject);
    }

    private void OnParticleCollision(GameObject other)
    {
      PerformKnockback(other);
    }
  }
}