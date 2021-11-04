using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Ability/Attack")]
  public class AttackAbility : MonoBehaviour, IAbility
  {
    [HideInInspector]
    private Character _character;
    private PlayerInventory _playerInventory;
    private InputActionReference _primaryAttackInput;
    private InputActionReference _secondaryAttackInput;

    private void Awake()
    {
      _character = GetComponent<Character>();
      _playerInventory = GetComponent<PlayerInventory>();
    }

    private void Start()
    {
      _primaryAttackInput = _character.Context.inputPack.PrimaryAttackInput;
      _secondaryAttackInput = _character.Context.inputPack.SecondaryAttackInput;

      // Setup Events
      OnEnable();
    }

    public void OnPrimaryAttackStarted(InputAction.CallbackContext ctx)
    {
      bool pointerOverUi = EventSystem.current.IsPointerOverGameObject();
      if (!pointerOverUi)
      {
        WeaponItem leftHand = _playerInventory.Inventory.LeftHand as WeaponItem;
        if (leftHand != null)
        {
          leftHand.Started();
        }
      }
    }

    public void OnPrimaryAttackCanceled(InputAction.CallbackContext ctx)
    {
      bool pointerOverUi = EventSystem.current.IsPointerOverGameObject();
      WeaponItem leftHand = _playerInventory.Inventory.LeftHand as WeaponItem;
      if (leftHand != null)
      {
        leftHand.Cancel();
      }
    }

    public void OnPrimaryAttackPerformed(InputAction.CallbackContext ctx)
    {
      bool pointerOverUi = EventSystem.current.IsPointerOverGameObject();
      if (!pointerOverUi)
      {
        WeaponItem leftHand = _playerInventory.Inventory.LeftHand as WeaponItem;
        if (leftHand != null)
        {
          leftHand.Perform();
        }
      }
    }

    public void OnSecondaryAttackStarted(InputAction.CallbackContext ctx)
    {
      bool pointerOverUi = EventSystem.current.IsPointerOverGameObject();
      if (!pointerOverUi)
      {
        WeaponItem rightHand = _playerInventory.Inventory.RightHand as WeaponItem;
        if (rightHand != null)
        {
          rightHand.Started();
        }
      }
    }

    public void OnSecondaryAttackCanceled(InputAction.CallbackContext ctx)
    {
      bool pointerOverUi = EventSystem.current.IsPointerOverGameObject();
      if (!pointerOverUi)
      {
        WeaponItem rightHand = _playerInventory.Inventory.RightHand as WeaponItem;
        if (rightHand != null)
        {
          rightHand.Cancel();
        }
      }
    }

    public void OnSecondaryAttackPerformed(InputAction.CallbackContext ctx)
    {
      bool pointerOverUi = EventSystem.current.IsPointerOverGameObject();
      if (!pointerOverUi)
      {
        WeaponItem rightHand = _playerInventory.Inventory.RightHand as WeaponItem;
        if (rightHand != null)
        {
          rightHand.Perform();
        }
      }
    }

    private void OnEnable()
    {

      // Make sure to setup new events
      OnDisable();

      if (_primaryAttackInput != null)
      {
        _primaryAttackInput.action.Enable();
        _primaryAttackInput.action.started += OnPrimaryAttackStarted;
        _primaryAttackInput.action.canceled += OnPrimaryAttackCanceled;
        _primaryAttackInput.action.performed += OnPrimaryAttackPerformed;
      }

      if (_secondaryAttackInput != null)
      {
        _secondaryAttackInput.action.Enable();
        _secondaryAttackInput.action.started += OnSecondaryAttackStarted;
        _secondaryAttackInput.action.canceled += OnSecondaryAttackCanceled;
        _secondaryAttackInput.action.performed += OnSecondaryAttackPerformed;
      }
    }

    private void OnDisable()
    {
      if (_primaryAttackInput != null)
      {
        _primaryAttackInput.action.Disable();
        _primaryAttackInput.action.started -= OnPrimaryAttackStarted;
        _primaryAttackInput.action.canceled -= OnPrimaryAttackCanceled;
        _primaryAttackInput.action.performed -= OnPrimaryAttackPerformed;
      }

      if (_secondaryAttackInput != null)
      {
        _secondaryAttackInput.action.Disable();
        _secondaryAttackInput.action.started -= OnSecondaryAttackStarted;
        _secondaryAttackInput.action.canceled -= OnSecondaryAttackCanceled;
        _secondaryAttackInput.action.performed -= OnSecondaryAttackPerformed;
      }
    }

  }
}