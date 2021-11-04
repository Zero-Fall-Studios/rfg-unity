using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Items/Player Inventory")]
  public class PlayerInventory : MonoBehaviour
  {
    [field: SerializeField] public Inventory Inventory { get; set; }
  }
}