using UnityEngine;

namespace RFG
{
  [CreateAssetMenu(fileName = "New Health Item", menuName = "RFG/Platformer/Items/Consumable/Health")]
  public class HealthItem : Consumable
  {
    [Header("Health Item Settings")]
    public int HealthToAdd = 5;

    public override void Consume(Transform transform, Inventory inventory)
    {
      base.Consume(transform, inventory);
      HealthBehaviour health = transform.gameObject.GetComponent<HealthBehaviour>();

      if (health != null)
      {
        health.AddHealth(HealthToAdd);
      }
    }
  }
}