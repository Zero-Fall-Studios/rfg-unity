using UnityEngine;
using UnityEngine.EventSystems;

namespace RFG
{
  public class ItemSlotUI : MonoBehaviour, IPointerClickHandler
  {
    public int SlotIndex { get; set; }

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
        InventoryUI.Instance.DropIntoSlot(this);
      }
    }
  }
}