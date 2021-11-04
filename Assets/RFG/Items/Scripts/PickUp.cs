using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using MyBox;

namespace RFG
{
  [AddComponentMenu("RFG/Items/Pick Up")]
  public class PickUp : MonoBehaviour, IPointerClickHandler
  {
    public static PickUp LastPickUp;
    [field: SerializeField] private Inventory Inventory { get; set; }
    [field: SerializeField] public Item Item { get; set; }
    [field: SerializeField] private LayerMask LayerMask { get; set; }
    [field: SerializeField] private float RespawnTime { get; set; } = 0f;
    [field: SerializeField] private bool OnlyOne { get; set; } = false;
    [field: SerializeField] public List<Item> RandomItems { get; set; }

    [field: SerializeField] private UnityEvent OnPickUp;

    [HideInInspector]
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;
    private float _respawnTimeElapsed = 0f;
    private PlayerInventory _playerInventory;

    private void Awake()
    {
      if (RandomItems.Count > 0)
      {
        PickRandomItem();
      }
      _spriteRenderer = GetComponent<SpriteRenderer>();
      _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
      if (Inventory == null)
      {
        FindInventoryFromPlayer();
      }
      if (Inventory.InInventory(Item.Guid) && OnlyOne)
      {
        gameObject.SetActive(false);
      }
    }

    private void LateUpdate()
    {
      if (_spriteRenderer.enabled == false && RespawnTime > 0f)
      {
        if (_respawnTimeElapsed > RespawnTime)
        {
          _respawnTimeElapsed = 0f;
          Spawn();
        }
        _respawnTimeElapsed += Time.deltaTime;
      }
    }

    private void FindInventoryFromPlayer()
    {
      GameObject player = GameObject.FindGameObjectWithTag("Player");
      if (player != null)
      {
        _playerInventory = player.GetComponent<PlayerInventory>();
        if (_playerInventory != null)
        {
          Inventory = _playerInventory.Inventory;
        }
      }
    }

    public void Spawn()
    {
      if (RandomItems.Count > 0)
      {
        PickRandomItem();
      }
      if (!gameObject.activeInHierarchy)
      {
        gameObject.SetActive(true);
      }
      _boxCollider.enabled = true;
      _spriteRenderer.enabled = true;
    }

    public void Drop()
    {
      LastPickUp = null;
      _boxCollider.enabled = true;
      _spriteRenderer.enabled = true;
      gameObject.SetActive(true);
    }

    private void Hide()
    {
      if (LastPickUp != null)
      {
        _boxCollider.enabled = false;
        _spriteRenderer.enabled = false;
        if (RespawnTime == 0f)
        {
          gameObject.SetActive(false);
        }
      }
    }

    private void HandlePickUp()
    {
      LastPickUp = this;

      if (Item is Consumable consumable)
      {
        if (consumable.ConsumeOnPickUp)
        {
          consumable.Consume(_playerInventory.transform, Inventory);
          Hide();
          return;
        }
      }

      Inventory.Add(Item);
      _playerInventory.transform.SpawnFromPool(Item.PickUpEffects, Quaternion.identity, new object[] { Item.PickUpText });
      OnPickUp?.Invoke();
      Hide();

      if (Item is Equipable equipable)
      {
        if (equipable.EquipOnPickUp)
        {
          equipable.Equip(_playerInventory.transform, Inventory);
        }
      }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
      if (LayerMask.Contains(col.gameObject.layer))
      {
        HandlePickUp();
      }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
      HandlePickUp();
    }

    private void PickRandomItem()
    {
      Item = RandomItems[UnityEngine.Random.Range(0, RandomItems.Count)];
      GeneratePickup();
    }

    [ButtonMethod]
    public void GeneratePickup()
    {
      _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
      if (_spriteRenderer == null)
      {
        _spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
      }
      _spriteRenderer.sprite = Item.PickUpSprite;

      if (gameObject.GetComponent<BoxCollider2D>() == null)
      {
        _boxCollider = gameObject.AddComponent<BoxCollider2D>();
        _boxCollider.isTrigger = true;
      }
      tag = "PickUp";
      LayerMask = LayerMask.GetMask("Player");
    }

  }
}