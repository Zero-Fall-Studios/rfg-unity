using System.Collections.Generic;
using UnityEngine;

namespace RFG
{
  public class InventoryUI : Singleton<InventoryUI>
  {
    [field: SerializeField] public Inventory Inventory { get; set; }
    [field: SerializeField] private List<ItemUI> Items { get; set; }
    [field: SerializeField] private List<ItemSlotUI> ItemSlots { get; set; }
    [field: SerializeField] private List<EquipmentSlotUI> EquipmentSlots { get; set; }
    [field: SerializeField] private List<string> InventoryFullEffects { get; set; }
    [field: SerializeField] private string InventoryFullMessage { get; set; } = "Inventory is full";

    public ItemUI SelectedItemUI { get; set; }
    public ItemUI ReplacedItemUI { get; set; }

    #region Unity Methods
    private void Start()
    {
      InitializeSlots();
      InitializeItems();
      InitializeEquipment();
      SelectedItemUI = null;
      ReplacedItemUI = null;
    }

    private void OnEnable()
    {
      Inventory.OnAdd += OnAdd;
      Inventory.OnUpdateQuantity += OnUpdateQuantity;
      Inventory.OnRemove += OnRemove;
    }

    private void OnDisable()
    {
      Inventory.OnAdd -= OnAdd;
      Inventory.OnUpdateQuantity -= OnUpdateQuantity;
      Inventory.OnRemove -= OnRemove;
    }
    #endregion

    #region Init
    private void InitializeSlots()
    {
      for (int i = 0; i < ItemSlots.Count; i++)
      {
        ItemSlots[i].SlotIndex = i;
      }
    }

    private void InitializeItems()
    {
      foreach (KeyValuePair<int, InventoryData> kvp in Inventory.Items)
      {
        CreateItemUI(kvp.Key, kvp.Value);
      }
    }

    private void InitializeEquipment()
    {
      foreach (EquipmentSlotUI equipmentSlot in EquipmentSlots)
      {
        Equipable equipable = null;
        switch (equipmentSlot.EquipmentSlot)
        {
          case EquipmentSlot.Helmet:
            if (Inventory.Helmet != null)
            {
              equipable = Inventory.Helmet;
            }
            break;
          case EquipmentSlot.Amulet:
            if (Inventory.Amulet != null)
            {
              equipable = Inventory.Amulet;
            }
            break;
          case EquipmentSlot.Armor:
            if (Inventory.Armor != null)
            {
              equipable = Inventory.Armor;
            }
            break;
          case EquipmentSlot.Belt:
            if (Inventory.Belt != null)
            {
              equipable = Inventory.Belt;
            }
            break;
          case EquipmentSlot.Legs:
            if (Inventory.Legs != null)
            {
              equipable = Inventory.Legs;
            }
            break;
          case EquipmentSlot.Boots:
            if (Inventory.Boots != null)
            {
              equipable = Inventory.Boots;
            }
            break;
          case EquipmentSlot.Gloves:
            if (Inventory.Gloves != null)
            {
              equipable = Inventory.Gloves;
            }
            break;
          case EquipmentSlot.LeftHand:
            if (Inventory.LeftHand != null)
            {
              equipable = Inventory.LeftHand;
            }
            break;
          case EquipmentSlot.RightHand:
            if (Inventory.RightHand != null)
            {
              equipable = Inventory.RightHand;
            }
            break;
          case EquipmentSlot.LeftRing:
            if (Inventory.LeftRing != null)
            {
              equipable = Inventory.LeftRing;
            }
            break;
          case EquipmentSlot.RightRing:
            if (Inventory.RightRing != null)
            {
              equipable = Inventory.RightRing;
            }
            break;
        }

        if (equipable != null)
        {
          InventoryData inventoryData = Inventory.CreateInventoryData(equipable);
          CreateItemUI(equipmentSlot, inventoryData);
        }
      }
    }
    #endregion

    #region Create
    public void CreateItemUI(int slotIndex, InventoryData inventoryData)
    {
      if (slotIndex >= ItemSlots.Count)
      {
        Inventory.RemoveSlot(slotIndex);
        transform.SpawnFromPool(InventoryFullEffects.ToArray(), new object[] { InventoryFullMessage });
        if (PickUp.LastPickUp != null)
        {
          PickUp.LastPickUp.Drop();
        }
        return;
      }

      // Create the item ui
      ItemUI itemUI = Items.Find(i => !i.gameObject.activeInHierarchy);
      itemUI.gameObject.SetActive(true);
      itemUI.SetInventoryData(inventoryData);

      // Set the slot
      ItemSlotUI itemSlotUI = ItemSlots[slotIndex];
      itemUI.SetPosition(itemSlotUI.GetPosition());
      itemUI.SlotIndex = slotIndex;
    }

    public void CreateItemUI(EquipmentSlotUI equipmentSlot, InventoryData inventoryData)
    {
      // Create the item ui
      ItemUI itemUI = Items.Find(i => !i.gameObject.activeInHierarchy);
      itemUI.gameObject.SetActive(true);
      itemUI.SetInventoryData(inventoryData);

      // Set the slot
      itemUI.SetPosition(equipmentSlot.GetPosition());
      itemUI.SlotIndex = -1;

      equipmentSlot.inventoryData = inventoryData;
    }
    #endregion

    #region Get
    public ItemSlotUI GetItemSlotUI(int slotIndex)
    {
      if (slotIndex == -1)
      {
        return null;
      }
      return ItemSlots[slotIndex];
    }

    public EquipmentSlotUI GetEquipmentSlotUI(EquipmentSlot slot)
    {
      foreach (EquipmentSlotUI equipmentSlot in EquipmentSlots)
      {
        if (equipmentSlot.EquipmentSlot == slot)
        {
          return equipmentSlot;
        }
      }
      return null;
    }

    public EquipmentSlot GetEquipmentSlot(Equipable equipable)
    {
      foreach (EquipmentSlotUI equipmentSlot in EquipmentSlots)
      {
        if (equipmentSlot.inventoryData != null && equipmentSlot.inventoryData.item == equipable)
        {
          return equipmentSlot.EquipmentSlot;
        }
      }
      return EquipmentSlot.None;
    }
    #endregion

    #region Selection
    public void SelectItemUI(ItemUI itemUI)
    {
      if (SelectedItemUI != null)
      {
        if (itemUI.SlotIndex > -1)
        {
          if (
            SelectedItemUI.inventoryData.item.IsStackable &&
            itemUI.inventoryData.item.IsStackable &&
            SelectedItemUI.inventoryData.item.Guid.Equals(itemUI.inventoryData.item.Guid)
          )
          {
            int addQuantity = itemUI.inventoryData.item.StackableLimit - itemUI.inventoryData.quantity;

            if (addQuantity >= SelectedItemUI.inventoryData.quantity)
            {
              addQuantity = SelectedItemUI.inventoryData.quantity;
              Inventory.UpdateQuantity(SelectedItemUI.SlotIndex, SelectedItemUI.inventoryData, -addQuantity);
              Inventory.UpdateQuantity(itemUI.SlotIndex, itemUI.inventoryData, addQuantity);

              Inventory.RemoveSlot(SelectedItemUI.SlotIndex);
              SelectedItemUI.Fade(false);
              SelectedItemUI = null;
            }
            else
            {
              Inventory.UpdateQuantity(SelectedItemUI.SlotIndex, SelectedItemUI.inventoryData, -addQuantity);
              Inventory.UpdateQuantity(itemUI.SlotIndex, itemUI.inventoryData, addQuantity);
            }
          }
          else
          {
            ItemSlotUI itemSlotUI = ItemSlots[itemUI.SlotIndex];
            ReplaceSlot(itemSlotUI);
            SelectedItemUI = itemUI;
            itemUI.Fade(true);
          }
        }
        else if (itemUI.inventoryData.item is Equipable equipable)
        {
          EquipmentSlot slot = GetEquipmentSlot(equipable);
          if (slot != EquipmentSlot.None)
          {
            EquipmentSlotUI equipmentSlotUI = GetEquipmentSlotUI(slot);
            ReplaceEquipmentSlot(equipmentSlotUI);
            SelectedItemUI = itemUI;
            itemUI.Fade(true);
          }
        }
      }
      else
      {
        SelectedItemUI = itemUI;
        itemUI.Fade(true);
      }
    }
    #endregion

    #region Mutate
    public void DropIntoSlot(ItemSlotUI itemSlotUI)
    {
      if (SelectedItemUI != null)
      {
        if (SelectedItemUI.SlotIndex == itemSlotUI.SlotIndex)
        {
          SelectedItemUI.SetPosition(itemSlotUI.GetPosition());
          SelectedItemUI.Fade(false);
          SelectedItemUI = null;
          return;
        }

        if (ReplacedItemUI == null)
        {
          Inventory.RemoveSlot(SelectedItemUI.SlotIndex);
        }
        else
        {
          ReplacedItemUI = null;
        }

        // Set new slot position in the inventory data
        Inventory.SetSlot(itemSlotUI.SlotIndex, SelectedItemUI.inventoryData);

        // If the item we are dropping is equipable, then unequip it since its the normal
        // inventory we are dropping onto
        if (SelectedItemUI.inventoryData.item is Equipable equipable)
        {
          if (equipable.IsEquipped)
          {
            UnEquipEquipmentSlot(equipable);
          }
        }

        SelectedItemUI.SlotIndex = itemSlotUI.SlotIndex;
        SelectedItemUI.SetPosition(itemSlotUI.GetPosition());
        SelectedItemUI.Fade(false);
        SelectedItemUI = null;
      }
    }

    public void ReplaceSlot(ItemSlotUI toItemSlotUI)
    {
      if (SelectedItemUI != null)
      {
        // Store that we are replacing an item ui
        ReplacedItemUI = SelectedItemUI;

        // Remove From Slot
        Inventory.RemoveSlot(SelectedItemUI.SlotIndex);

        // Set To Slot
        Inventory.RemoveSlot(toItemSlotUI.SlotIndex);
        Inventory.SetSlot(toItemSlotUI.SlotIndex, SelectedItemUI.inventoryData);

        // If the item we are dropping is equipable, then unequip it since its the normal
        // inventory we are dropping onto
        if (SelectedItemUI.inventoryData.item is Equipable equipable)
        {
          if (equipable.IsEquipped)
          {
            UnEquipEquipmentSlot(equipable);
          }
        }

        SelectedItemUI.SlotIndex = toItemSlotUI.SlotIndex;
        SelectedItemUI.SetPosition(toItemSlotUI.GetPosition());
        SelectedItemUI.Fade(false);
        SelectedItemUI = null;
      }
    }

    public bool DropIntoSlot(EquipmentSlotUI equipmentSlotUI)
    {
      if (SelectedItemUI != null)
      {
        InventoryData inventoryData = SelectedItemUI.inventoryData;
        Item item = inventoryData.item;
        if (item is Equipable equipable)
        {
          if (equipmentSlotUI.EquipmentType == equipable.EquipmentType)
          {
            if (SelectedItemUI.SlotIndex > -1)
            {
              // If setting equipment slot then remove the slot from the normal inventory data
              Inventory.RemoveSlot(SelectedItemUI.SlotIndex);
            }
            SelectedItemUI.SetPosition(equipmentSlotUI.GetPosition());
            Inventory.Equip(equipmentSlotUI.EquipmentSlot, equipable);

            SelectedItemUI.SlotIndex = -1;
            SelectedItemUI.Fade(false);
            SelectedItemUI = null;

            equipmentSlotUI.inventoryData = inventoryData;

            return true;
          }
        }
      }
      return false;
    }

    public void ReplaceEquipmentSlot(EquipmentSlotUI toEquipmentSlotUI)
    {
      if (SelectedItemUI != null)
      {
        // Unequip the current item in the slot
        if (toEquipmentSlotUI.inventoryData.item is Equipable equipable)
        {
          if (equipable.IsEquipped)
          {
            UnEquipEquipmentSlot(equipable);
          }
        }
        DropIntoSlot(toEquipmentSlotUI);
      }
    }


    public void UnEquipEquipmentSlot(Equipable equipable)
    {
      EquipmentSlot slot = EquipmentSlot.None;
      foreach (EquipmentSlotUI equipmentSlot in EquipmentSlots)
      {
        if (equipmentSlot.inventoryData != null && equipmentSlot.inventoryData.item == equipable)
        {
          slot = equipmentSlot.EquipmentSlot;
          equipmentSlot.inventoryData = null;
          break;
        }
      }
      if (slot != EquipmentSlot.None)
      {
        Inventory.UnEquip(slot, equipable);
      }
    }
    #endregion

    #region Events
    private void OnAdd(KeyValuePair<int, InventoryData> kvp)
    {
      CreateItemUI(kvp.Key, kvp.Value);
    }

    private void OnUpdateQuantity(KeyValuePair<int, InventoryData> kvp)
    {
      foreach (ItemUI itemUI in Items)
      {
        if (itemUI.gameObject.activeInHierarchy && itemUI.SlotIndex == kvp.Key)
        {
          itemUI.SetInventoryData(kvp.Value);
        }
      }
    }

    private void OnRemove(KeyValuePair<int, InventoryData> kvp)
    {
      foreach (ItemUI itemUI in Items)
      {
        if (itemUI.SlotIndex == kvp.Key && kvp.Value.quantity <= 0)
        {
          itemUI.gameObject.SetActive(false);
        }
      }
    }

    public void OnConsume(KeyValuePair<int, InventoryData> kvp)
    {
      Inventory.Consume(kvp.Key);
    }
    #endregion

  }
}