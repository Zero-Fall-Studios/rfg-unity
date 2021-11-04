using UnityEngine;

namespace RFG
{
  public interface IConsumable
  {
    void Consume(Transform transform, Inventory inventory);
  }
}