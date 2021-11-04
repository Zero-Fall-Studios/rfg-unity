using System;
using System.Collections.Generic;
using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Items/Equipables/Weapon/Weapon")]
  public class Weapon : MonoBehaviour
  {
    [Header("Settings")]
    public WeaponItem WeaponItem;
    public Transform FirePoint;
    public EquipmentSet EquipmentSet;

    [Header("States")]
    public WeaponState[] States;
    public WeaponState DefaultState;
    public WeaponState CurrentState;
    public Type PreviousStateType { get; private set; }
    public Type CurrentStateType { get; private set; }

    [HideInInspector]

    private float _fireRateElapsed;
    private float _cooldownElapsed;
    private float _gainAmmoOverTimeElapsed;
    private Dictionary<Type, WeaponState> _states;

    private void Awake()
    {
      WeaponItem.Ammo = WeaponItem.StartingAmmo;
      _states = new Dictionary<Type, WeaponState>();
      foreach (WeaponState state in States)
      {
        _states.Add(state.GetType(), state);
      }
    }

    private void Start()
    {
      Reset();
    }

    private void Update()
    {
      if (!WeaponItem.IsEquipped)
      {
        return;
      }
      Type newStateType = CurrentState.Execute(this);
      if (newStateType != null)
      {
        ChangeState(newStateType);
      }
    }

    public void ChangeState(Type newStateType)
    {
      if (_states[newStateType].Equals(CurrentState))
      {
        return;
      }
      if (CurrentState != null)
      {
        PreviousStateType = CurrentState.GetType();
        CurrentState.Exit(this);
      }
      CurrentState = _states[newStateType];
      CurrentStateType = newStateType;
      CurrentState.Enter(this);
    }

    public void Reset()
    {
      CurrentState = null;
      if (DefaultState != null)
      {
        ChangeState(DefaultState.GetType());
      }
    }

    public void RestorePreviousState()
    {
      ChangeState(PreviousStateType);
    }

    private void LateUpdate()
    {
      if (!WeaponItem.IsEquipped)
      {
        return;
      }
      _fireRateElapsed += Time.deltaTime;
      if (_fireRateElapsed >= WeaponItem.FireRate)
      {
        _fireRateElapsed = 0;
        WeaponItem.CanUse = true;
      }

      if (WeaponItem.IsInCooldown)
      {
        _cooldownElapsed += Time.deltaTime;
        if (_cooldownElapsed >= WeaponItem.Cooldown)
        {
          _cooldownElapsed = 0;
          WeaponItem.IsInCooldown = false;
        }
      }

      if (CurrentStateType == typeof(WeaponIdleState) && !WeaponItem.IsInCooldown && !WeaponItem.UnlimitedAmmo)
      {
        _gainAmmoOverTimeElapsed += Time.deltaTime;
        if (_gainAmmoOverTimeElapsed >= WeaponItem.GainAmmoOverTime)
        {
          _gainAmmoOverTimeElapsed = 0;
          WeaponItem.AddAmmo(WeaponItem.AmmoGain);
        }
      }
    }

    // private void OnPickUp(InventoryManager InventoryManager)
    // {
    //   // if (InventoryManager.InInventory(WeaponItem.Guid))
    //   // {
    //   //   WeaponItem.Refill();
    //   // }
    // }

    // private void OnEquip(InventoryManager InventoryManager)
    // {
    //   if (EquipmentSet.PrimaryWeapon == null)
    //   {
    //     EquipmentSet.EquipPrimaryWeapon(WeaponItem);
    //     WeaponItem.IsEquipped = true;
    //   }
    //   else if (!EquipmentSet.PrimaryWeapon.Equals(WeaponItem) && EquipmentSet.SecondaryWeapon == null)
    //   {
    //     EquipmentSet.EquipSecondaryWeapon(WeaponItem);
    //     WeaponItem.IsEquipped = true;
    //   }
    // }

    // private void OnUnequip(InventoryManager InventoryManager)
    // {
    //   WeaponItem.IsEquipped = false;
    // }

    private void OnStateChange(Type state)
    {
      ChangeState(state);
    }

    private void OnEnable()
    {
      // WeaponItem.OnPickUp += OnPickUp;
      // WeaponItem.OnEquip += OnEquip;
      // WeaponItem.OnUnequip += OnUnequip;
      WeaponItem.OnStateChange += OnStateChange;
    }

    private void OnDisable()
    {
      // WeaponItem.OnPickUp -= OnPickUp;
      // WeaponItem.OnEquip -= OnEquip;
      // WeaponItem.OnUnequip -= OnUnequip;
      WeaponItem.OnStateChange -= OnStateChange;
    }

  }
}