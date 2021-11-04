using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace RFG
{
  public enum EquipmentSlot
  {
    Helmet,
    Amulet,
    Armor,
    Belt,
    Legs,
    Boots,
    Gloves,
    LeftHand,
    RightHand,
    LeftRing,
    RightRing,
    None
  }

  [Serializable]
  public class InventoryData
  {
    public Item item;
    public int quantity;
  }

  [CreateAssetMenu(fileName = "New Inventory", menuName = "RFG/Items/Inventory")]
  public class Inventory : ScriptableObject
  {
    public SerializableDictionary<int, InventoryData> Items;
    public Action<KeyValuePair<int, InventoryData>> OnAdd;
    public Action<KeyValuePair<int, InventoryData>> OnRemove;
    public Action<KeyValuePair<int, InventoryData>> OnUpdateQuantity;
    public Action<int> OnUpdateMoney;

    [Header("Equipment")]
    public Equipable Helmet;
    public Equipable Amulet;
    public Equipable Armor;
    public Equipable Belt;
    public Equipable Legs;
    public Equipable Boots;
    public Equipable Gloves;
    public Equipable LeftHand;
    public Equipable RightHand;
    public Equipable LeftRing;
    public Equipable RightRing;
    public Action<KeyValuePair<EquipmentSlot, Equipable>> OnEquip;
    public Action<KeyValuePair<EquipmentSlot, Equipable>> OnUnEquip;

    [Header("Other")]
    public int Money = 0;

    #region Add Methods
    public InventoryData Add(Item item)
    {
      if (item.OnlyOne && InInventory(item.Guid))
      {
        return null;
      }
      if (item.IsStackable)
      {
        return AddStackable(item);
      }
      else
      {
        return AddOne(item);
      }
    }

    private InventoryData AddStackable(Item item)
    {
      SerializableDictionary<int, InventoryData> d = FindByGuid(item.Guid);
      if (d.Count == 0)
      {
        // This means there are none of this item yet
        // so just add a brand new item
        return AddOne(item);
      }
      else
      {
        // Find the first stackable item and add to its quantity
        foreach (KeyValuePair<int, InventoryData> kvp in d)
        {
          if (kvp.Value.quantity < item.StackableLimit)
          {
            // It found a stack that hasn't exceeded the limit
            // so increment and return
            UpdateQuantity(kvp.Key, kvp.Value, 1);
            return kvp.Value;
          }
        }
        // If it reaches here then it means all stacks are full
        // so add a brand new item
        return AddOne(item);
      }
    }

    private InventoryData AddOne(Item item)
    {
      InventoryData inventoryData = CreateInventoryData(item);
      int slotIndex = NextAvailableSlot();
      Items.Add(slotIndex, inventoryData);
      OnAdd?.Invoke(new KeyValuePair<int, InventoryData>(slotIndex, inventoryData));
      return inventoryData;
    }

    public InventoryData CreateInventoryData(Item item)
    {
      InventoryData inventoryData = new InventoryData()
      {
        item = item,
        quantity = 1,
      };
      return inventoryData;
    }

    public void UpdateQuantity(int slotIndex, InventoryData inventoryData, int quantity)
    {
      int newQuantity = inventoryData.quantity + quantity;
      if (newQuantity <= inventoryData.item.StackableLimit)
      {
        inventoryData.quantity += quantity;
        OnUpdateQuantity?.Invoke(new KeyValuePair<int, InventoryData>(slotIndex, inventoryData));
      }
    }
    #endregion

    #region Find Methods
    public bool InInventory(string guid)
    {
      return FindByGuid(guid).Count > 0;
    }

    public SerializableDictionary<int, InventoryData> FindByType<T>() where T : Item
    {
      SerializableDictionary<int, InventoryData> d = new SerializableDictionary<int, InventoryData>();
      foreach (KeyValuePair<int, InventoryData> kvp in Items)
      {
        if (kvp.Value.item is T)
        {
          d.Add(kvp.Key, kvp.Value);
        }
      }
      return d;
    }

    public SerializableDictionary<int, InventoryData> FindByGuid(string guid)
    {
      SerializableDictionary<int, InventoryData> d = new SerializableDictionary<int, InventoryData>();
      foreach (KeyValuePair<int, InventoryData> kvp in Items)
      {
        if (kvp.Value.item.Guid.Equals(guid))
        {
          d.Add(kvp.Key, kvp.Value);
        }
      }
      return d;
    }
    #endregion

    #region Slot Methods
    public int GetSlotIndex(InventoryData inventoryData)
    {
      KeyValuePair<int, InventoryData> kvp = Items.First(i => i.Value.Equals(inventoryData));
      return kvp.Key;
    }

    public InventoryData GetSlot(int slotIndex)
    {
      if (Items.ContainsKey(slotIndex))
      {
        return Items[slotIndex];
      }
      return null;
    }

    public void SetSlot(int slotIndex, InventoryData inventoryData)
    {
      if (Items.ContainsKey(slotIndex))
      {
        Items[slotIndex] = inventoryData;
      }
      else
      {
        Items.Add(slotIndex, inventoryData);
      }
    }

    public void RemoveSlot(int slotIndex)
    {
      if (Items.ContainsKey(slotIndex))
      {
        OnRemove?.Invoke(new KeyValuePair<int, InventoryData>(slotIndex, GetSlot(slotIndex)));
        Items.Remove(slotIndex);
      }
    }

    private int NextAvailableSlot()
    {
      for (int i = 0; i < Items.Count; i++)
      {
        if (!Items.ContainsKey(i))
        {
          return i;
        }
      }
      return Items.Count;
    }
    #endregion

    #region Equipment
    public void Equip(EquipmentSlot equipmentSlot, Equipable equipable)
    {
      switch (equipmentSlot)
      {
        case EquipmentSlot.Helmet:
          Helmet = equipable;
          break;
        case EquipmentSlot.Amulet:
          Amulet = equipable;
          break;
        case EquipmentSlot.Armor:
          Armor = equipable;
          break;
        case EquipmentSlot.Belt:
          Belt = equipable;
          break;
        case EquipmentSlot.Legs:
          Legs = equipable;
          break;
        case EquipmentSlot.Boots:
          Boots = equipable;
          break;
        case EquipmentSlot.Gloves:
          Gloves = equipable;
          break;
        case EquipmentSlot.LeftHand:
          LeftHand = equipable;
          break;
        case EquipmentSlot.RightHand:
          RightHand = equipable;
          break;
        case EquipmentSlot.LeftRing:
          LeftRing = equipable;
          break;
        case EquipmentSlot.RightRing:
          RightRing = equipable;
          break;
      }
      equipable.IsEquipped = true;
      OnEquip?.Invoke(new KeyValuePair<EquipmentSlot, Equipable>(equipmentSlot, equipable));
    }

    public void UnEquip(EquipmentSlot equipmentSlot, Equipable equipable)
    {
      switch (equipmentSlot)
      {
        case EquipmentSlot.Helmet:
          Helmet = null;
          break;
        case EquipmentSlot.Amulet:
          Amulet = null;
          break;
        case EquipmentSlot.Armor:
          Armor = null;
          break;
        case EquipmentSlot.Belt:
          Belt = null;
          break;
        case EquipmentSlot.Legs:
          Legs = null;
          break;
        case EquipmentSlot.Boots:
          Boots = null;
          break;
        case EquipmentSlot.Gloves:
          Gloves = null;
          break;
        case EquipmentSlot.LeftHand:
          LeftHand = null;
          break;
        case EquipmentSlot.RightHand:
          RightHand = null;
          break;
        case EquipmentSlot.LeftRing:
          LeftRing = null;
          break;
        case EquipmentSlot.RightRing:
          RightRing = null;
          break;
      }
      equipable.IsEquipped = false;
      OnUnEquip?.Invoke(new KeyValuePair<EquipmentSlot, Equipable>(equipmentSlot, equipable));
    }
    #endregion

    #region Consumables
    public void Consume(int slotIndex)
    {
      InventoryData inventoryData = GetSlot(slotIndex);
      inventoryData.quantity -= 1;
      if (inventoryData.quantity <= 0)
      {
        RemoveSlot(slotIndex);
      }
    }
    #endregion

    #region Money
    public void AddMoney(int amount)
    {
      Money += amount;
      if (Money <= 0)
      {
        Money = 0;
      }
      OnUpdateMoney?.Invoke(Money);
    }
    #endregion

    #region Save and Load
    public void Save(string path)
    {
      this.SaveData(path);
    }

    public void Load(string path)
    {
      this.LoadData(path);
    }
    #endregion
  }
}