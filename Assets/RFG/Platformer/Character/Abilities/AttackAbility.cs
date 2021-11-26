using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Ability/Attack")]
  public class AttackAbility : MonoBehaviour, IAbility
  {
    private Character _character;
    private PlayerInventory _playerInventory;
    private InputActionReference _primaryAttackInput;
    private InputActionReference _secondaryAttackInput;
    private bool _pointerOverUi = false;

    private void Awake()
    {
      _character = GetComponent<Character>();
      _playerInventory = GetComponent<PlayerInventory>();
      _primaryAttackInput = _character.InputPack.PrimaryAttackInput;
      _secondaryAttackInput = _character.InputPack.SecondaryAttackInput;
    }

    public void OnPrimaryAttackStarted(InputAction.CallbackContext ctx)
    {
      _pointerOverUi = MouseOverUILayerObject.IsPointerOverUIObject();
      if (!_pointerOverUi)
      {
        if (
          _character.IsAnyPrimaryAttack ||
          (_character.IsInAirMovementState && !_character.SettingsPack.CanAttackInAirPrimary)
        )
        {
          return;
        }
        bool changedState = _character.MovementState.ChangeState(typeof(PrimaryAttackStartedState));
        if (changedState)
        {
          WeaponItem leftHand = _playerInventory.Inventory.LeftHand as WeaponItem;
          if (leftHand != null)
          {
            leftHand.Started();
          }
        }
      }
    }

    public void OnPrimaryAttackCanceled(InputAction.CallbackContext ctx)
    {
      if (_character.MovementState.IsntInState(typeof(PrimaryAttackStartedState)))
      {
        return;
      }
      bool changedState = _character.MovementState.ChangeState(typeof(PrimaryAttackCanceledState));
      if (changedState)
      {
        WeaponItem leftHand = _playerInventory.Inventory.LeftHand as WeaponItem;
        if (leftHand != null)
        {
          leftHand.Cancel();
        }
      }
    }

    public void OnPrimaryAttackPerformed(InputAction.CallbackContext ctx)
    {
      if (_character.MovementState.IsntInState(typeof(PrimaryAttackStartedState)))
      {
        return;
      }
      bool changedState = _character.MovementState.ChangeState(typeof(PrimaryAttackPerformedState));
      if (changedState)
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
      _pointerOverUi = MouseOverUILayerObject.IsPointerOverUIObject();
      if (!_pointerOverUi)
      {
        if (
          _character.IsAnySecondaryAttack ||
          (_character.IsInAirMovementState && !_character.SettingsPack.CanAttackInAirSecondary)
        )
        {
          return;
        }
        bool changedState = _character.MovementState.ChangeState(typeof(SecondaryAttackStartedState));
        if (changedState)
        {
          WeaponItem rightHand = _playerInventory.Inventory.RightHand as WeaponItem;
          if (rightHand != null)
          {
            rightHand.Started();
          }
        }
      }
    }

    public void OnSecondaryAttackCanceled(InputAction.CallbackContext ctx)
    {
      if (_character.MovementState.IsntInState(typeof(SecondaryAttackStartedState)))
      {
        return;
      }
      bool changedState = _character.MovementState.ChangeState(typeof(SecondaryAttackCanceledState));
      if (changedState)
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
      if (_character.MovementState.IsntInState(typeof(SecondaryAttackStartedState)))
      {
        return;
      }
      bool changedState = _character.MovementState.ChangeState(typeof(SecondaryAttackPerformedState));
      if (changedState)
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
      if (_primaryAttackInput != null)
      {
        _primaryAttackInput.action.started += OnPrimaryAttackStarted;
        _primaryAttackInput.action.canceled += OnPrimaryAttackCanceled;
        _primaryAttackInput.action.performed += OnPrimaryAttackPerformed;
      }

      if (_secondaryAttackInput != null)
      {
        _secondaryAttackInput.action.started += OnSecondaryAttackStarted;
        _secondaryAttackInput.action.canceled += OnSecondaryAttackCanceled;
        _secondaryAttackInput.action.performed += OnSecondaryAttackPerformed;
      }
    }

    private void OnDisable()
    {
      if (_primaryAttackInput != null)
      {
        _primaryAttackInput.action.started -= OnPrimaryAttackStarted;
        _primaryAttackInput.action.canceled -= OnPrimaryAttackCanceled;
        _primaryAttackInput.action.performed -= OnPrimaryAttackPerformed;
      }

      if (_secondaryAttackInput != null)
      {
        _secondaryAttackInput.action.started -= OnSecondaryAttackStarted;
        _secondaryAttackInput.action.canceled -= OnSecondaryAttackCanceled;
        _secondaryAttackInput.action.performed -= OnSecondaryAttackPerformed;
      }
    }

  }
}