using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RFG
{
  public class EquipmentSlotUI : MonoBehaviour, IPointerClickHandler
  {
    public InventoryData inventoryData;
    [field: SerializeField] public EquipmentType EquipmentType { get; set; }
    [field: SerializeField] public EquipmentSlot EquipmentSlot { get; set; }

    public List<string> IncorrectSlotEffects;
    public string IncorrectSlotMessage = "That cannot be equipped";
    public List<string> EquipEffects;
    public string EquipMessage = "Equipped";
    public List<string> UnEquipEffects;
    public string UnEquipMessage = "UnEquipped";

    private RectTransform _rectTransform;

    private void Awake()
    {
      _rectTransform = GetComponent<RectTransform>();
    }

    public Vector2 GetPosition()
    {
      return _rectTransform.anchoredPosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
      if (eventData.button.ToString().Equals("Left"))
      {
        if (InventoryUI.Instance.DropIntoSlot(this))
        {
          transform.SpawnFromPool(EquipEffects.ToArray(), new object[] { EquipMessage });
        }
        else
        {
          transform.SpawnFromPool(IncorrectSlotEffects.ToArray(), new object[] { IncorrectSlotMessage });
        }
      }
    }
  }
}