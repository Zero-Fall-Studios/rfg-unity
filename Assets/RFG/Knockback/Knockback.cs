using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Interactions/Knockback")]
  public class Knockback : MonoBehaviour
  {
    public KnockbackData KnockbackData;

    private Vector2 GetKnockbackVelocity(Vector2 target1, Vector2 target2)
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

    private void ChangeState(GameObject other)
    {
      Character character = other.gameObject.GetComponent<Character>();

      if (character != null)
      {
        if (KnockbackData.ChangeCharacterState != null)
        {
          character.CharacterState.ChangeState(KnockbackData.ChangeCharacterState.GetType());
        }
        if (KnockbackData.ChangeMovementState != null)
        {
          character.MovementState.ChangeState(KnockbackData.ChangeMovementState.GetType());
        }
      }
    }

    private void AddVelocityCharacterController2D(GameObject other)
    {
      if (KnockbackData.AffectCharacterController2D && !KnockbackData.Velocity.Equals(Vector2.zero))
      {
        CharacterController2D controller = other.GetComponent<CharacterController2D>();
        if (controller != null)
        {
          Vector2 velocity = GetKnockbackVelocity(other.transform.position, transform.position);
          if (controller != null && controller.Parameters.Weight > 0)
          {
            velocity /= controller.Parameters.Weight;
          }
          controller.SetForce(velocity);
        }
      }
    }

    private void AddVelocityRigidBody2D(GameObject other)
    {
      if (KnockbackData.AffectRigidBody2D && !KnockbackData.Velocity.Equals(Vector2.zero))
      {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
          Vector2 velocity = GetKnockbackVelocity(other.transform.position, transform.position);
          rb.AddForce(velocity);
        }
      }
    }

    private void AddDamage(GameObject other)
    {
      if (KnockbackData.Damage > 0f)
      {
        HealthBehaviour health = other.GetComponent<HealthBehaviour>();
        if (health != null)
        {
          health.TakeDamage(KnockbackData.Damage);
        }
      }
    }

    private void PerformKnockback(GameObject other)
    {
      if (KnockbackData == null)
      {
        return;
      }
      if (KnockbackData.LayerMask.Contains(other.layer) || (KnockbackData.Tags != null && other.CompareTags(KnockbackData.Tags)))
      {
        transform.SpawnFromPool(KnockbackData.Effects);
        ChangeState(other);
        AddVelocityCharacterController2D(other);
        AddVelocityRigidBody2D(other);
        AddDamage(other);
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