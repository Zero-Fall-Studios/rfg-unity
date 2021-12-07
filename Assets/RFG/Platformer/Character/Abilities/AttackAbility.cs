using UnityEngine;
using UnityEngine.InputSystem;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Ability/Attack")]
  public class AttackAbility : MonoBehaviour, IAbility
  {
    private Character _character;
    private PlayerInput _playerInput;
    private PlayerInventory _playerInventory;
    private InputAction _primaryAttackInput;
    private InputAction _secondaryAttackInput;
    private bool _pointerOverUi = false;

    #region Unity Methods
    private void Awake()
    {
      _character = GetComponent<Character>();
      _playerInput = GetComponent<PlayerInput>();
      _playerInventory = GetComponent<PlayerInventory>();
      _primaryAttackInput = _playerInput.actions["PrimaryAttack"];
      _secondaryAttackInput = _playerInput.actions["SecondaryAttack"];
    }

    private void OnEnable()
    {
      if (_primaryAttackInput != null)
      {
        _primaryAttackInput.started += OnPrimaryAttackStarted;
        _primaryAttackInput.canceled += OnPrimaryAttackCanceled;
        _primaryAttackInput.performed += OnPrimaryAttackPerformed;
      }

      if (_secondaryAttackInput != null)
      {
        _secondaryAttackInput.started += OnSecondaryAttackStarted;
        _secondaryAttackInput.canceled += OnSecondaryAttackCanceled;
        _secondaryAttackInput.performed += OnSecondaryAttackPerformed;
      }
    }

    private void OnDisable()
    {
      if (_primaryAttackInput != null)
      {
        _primaryAttackInput.started -= OnPrimaryAttackStarted;
        _primaryAttackInput.canceled -= OnPrimaryAttackCanceled;
        _primaryAttackInput.performed -= OnPrimaryAttackPerformed;
      }

      if (_secondaryAttackInput != null)
      {
        _secondaryAttackInput.started -= OnSecondaryAttackStarted;
        _secondaryAttackInput.canceled -= OnSecondaryAttackCanceled;
        _secondaryAttackInput.performed -= OnSecondaryAttackPerformed;
      }
    }
    #endregion

    #region Events
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
    #endregion

  }
}