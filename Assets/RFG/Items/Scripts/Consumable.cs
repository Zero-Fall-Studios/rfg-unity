using UnityEngine;

namespace RFG
{
  public abstract class Consumable : Item, IConsumable
  {
    [Header("Consumable Settings")]
    public bool ConsumeOnPickUp = false;
    public string ConsumeText;
    public string[] ConsumeEffects;

    public virtual void Consume(Transform transform, Inventory inventory)
    {
      transform.SpawnFromPool(ConsumeEffects, Quaternion.identity, new object[] { ConsumeText });
    }

  }
}