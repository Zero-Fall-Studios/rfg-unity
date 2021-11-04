using UnityEngine;

namespace RFG
{
  public enum EquipmentType
  {
    Helmet,
    Amulet,
    Armor,
    Belt,
    Legs,
    Boots,
    Gloves,
    Hand,
    Ring
  }

  public abstract class Equipable : Item, IEquipable
  {
    [Header("Equipable Settings")]
    public EquipmentType EquipmentType;
    public bool IsEquipped = false;
    public bool EquipOnPickUp = false;
    public Sprite EquipSprite;
    public string EquipText;
    public string UnequipText;
    public string[] EquipEffects;
    public string[] UnequipEffects;

    public virtual void Equip(Transform transform, Inventory inventory)
    {
      IsEquipped = true;
      transform.SpawnFromPool(EquipEffects, Quaternion.identity, new object[] { EquipText });
    }

    public virtual void Unequip(Transform transform, Inventory inventory)
    {
      IsEquipped = false;
      transform.SpawnFromPool(UnequipEffects, Quaternion.identity, new object[] { UnequipText });
    }

  }
}