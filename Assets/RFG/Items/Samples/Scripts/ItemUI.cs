using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;

namespace RFG
{
  public class ItemUI : MonoBehaviour, IPointerClickHandler
  {
    public InventoryData inventoryData;
    [field: SerializeField] public int SlotIndex { get; set; }

    [SerializeField] private Canvas canvas;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Image _image;
    private TMP_Text _text;
    private Camera _camera;

    #region Unity Methods
    private void Awake()
    {
      _camera = Camera.main;
      _rectTransform = GetComponent<RectTransform>();
      _canvasGroup = GetComponent<CanvasGroup>();

      Transform imageTransform = transform.Find("Image");
      _image = imageTransform.gameObject.GetComponent<Image>();

      Transform quantityTransform = transform.Find("Quantity");
      _text = quantityTransform.gameObject.GetComponent<TMP_Text>();
    }

    private void Update()
    {
      if (InventoryUI.Instance.SelectedItemUI == this)
      {
        UpdatePositionFromMouse();
      }
    }
    #endregion

    #region Position
    public void SetPosition(Vector2 position)
    {
      _rectTransform.anchoredPosition = position;
    }

    public void UpdatePositionFromMouse()
    {
      Mouse mouse = Mouse.current;
      if (mouse != null)
      {
        Vector2 movePos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            mouse.position.ReadValue(),
            canvas.worldCamera,
            out movePos);

        transform.position = canvas.transform.TransformPoint(movePos);
      }
    }
    #endregion

    #region Helper
    public void SetInventoryData(InventoryData inventoryData)
    {
      this.inventoryData = inventoryData;
      _image.sprite = inventoryData.item.PickUpSprite;

      if (inventoryData.item.StackableLimit == 1)
      {
        _text.gameObject.SetActive(false);
      }
      else
      {
        _text.SetText(inventoryData.quantity.ToString());
      }
    }

    public void Fade(bool fade)
    {
      if (fade)
      {
        _canvasGroup.alpha = .6f;
        _canvasGroup.blocksRaycasts = false;
      }
      else
      {
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
      }
    }
    #endregion

    #region Events
    public void OnPointerClick(PointerEventData eventData)
    {
      if (eventData.button.ToString().Equals("Left"))
      {
        InventoryUI.Instance.SelectItemUI(this);
      }
      else if (eventData.button.ToString().Equals("Right"))
      {
        if (inventoryData.item is Consumable consumable)
        {
          consumable.Consume(transform, InventoryUI.Instance.Inventory);
          InventoryUI.Instance.OnConsume(new KeyValuePair<int, InventoryData>(SlotIndex, inventoryData));
          SetInventoryData(inventoryData);
        }
      }
    }
    #endregion

  }
}