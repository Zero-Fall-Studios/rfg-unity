using UnityEngine;

namespace RFG
{
  [CreateAssetMenu(fileName = "New Max Health Item", menuName = "RFG/Platformer/Items/Consumable/Max Health")]
  public class MaxHealthItem : Consumable
  {
    [Header("Max Health Item Settings")]
    public int MaxHealthToAdd = 1;

    public override void Consume(Transform transform, Inventory inventory)
    {
      base.Consume(transform, inventory);
      HealthBehaviour health = transform.gameObject.GetComponent<HealthBehaviour>();
      if (health != null)
      {
        health.AddMaxHealth(MaxHealthToAdd);
      }
    }
  }
}