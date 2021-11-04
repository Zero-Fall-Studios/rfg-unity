using System;
using UnityEngine;

namespace RFG
{
  [Serializable]
  public class WeaponItemSave
  {
    public string Guid;
    public int Ammo;
    public bool IsEquipped;
  }

  [CreateAssetMenu(fileName = "New Weapon Item", menuName = "RFG/Platformer/Items/Equipable/Weapon")]
  public class WeaponItem : Equipable
  {
    public enum WeaponType { InstaFire, Chargable }

    [Header("Weapon Settings")]
    public WeaponType weaponType = WeaponType.InstaFire;
    public float FireRate = 1f;
    public float Cooldown = 0f;
    public bool IsInCooldown = false;
    public bool IsFiring = false;
    public bool CanUse = false;

    [Header("Ammo")]
    public int Ammo = 0;
    public int MaxAmmo = 100;
    public int StartingAmmo = 10;
    public int RefillAmmo = 10;
    public float GainAmmoOverTime = 0;
    public int AmmoGain = 0;
    public bool UnlimitedAmmo = false;

    public Action<int> OnAmmoChange;
    public Action<Type> OnStateChange;

    public override void Equip(Transform transform, Inventory inventory)
    {
      if (!IsEquipped)
      {
        if (inventory.LeftHand == null)
        {
          inventory.Equip(EquipmentSlot.LeftHand, this);
          return;
        }
        if (inventory.RightHand == null)
        {
          inventory.Equip(EquipmentSlot.RightHand, this);
          return;
        }
      }
      base.Equip(transform, inventory);
    }

    public void Started()
    {
      if (!IsEquipped || !CanUse || IsInCooldown || Ammo <= 0)
      {
        return;
      }
      if (weaponType == WeaponItem.WeaponType.Chargable)
      {
        IsFiring = true;
        OnStateChange?.Invoke(typeof(WeaponChargingState));
      }
    }

    public void Cancel()
    {
      if (!IsEquipped || !IsFiring)
      {
        return;
      }
      if (weaponType == WeaponItem.WeaponType.Chargable)
      {
        OnStateChange?.Invoke(typeof(WeaponIdleState));
      }
    }

    public void Perform()
    {
      if (!IsEquipped || !CanUse || IsInCooldown || (Ammo <= 0 && !UnlimitedAmmo) || (weaponType == WeaponItem.WeaponType.Chargable && !IsFiring))
      {
        return;
      }
      OnStateChange?.Invoke(typeof(WeaponFiringState));
      if (!UnlimitedAmmo)
      {
        AddAmmo(-1);
      }
      CanUse = false;
    }

    public void AddAmmo(int amount)
    {
      Ammo += amount;
      if (Ammo <= 0)
      {
        Ammo = 0;
        if (Cooldown > 0)
        {
          IsInCooldown = true;
        }
      }
      else if (Ammo >= MaxAmmo)
      {
        Ammo = MaxAmmo;
      }
      OnAmmoChange?.Invoke(Ammo);
    }

    public void Refill()
    {
      AddAmmo(RefillAmmo);
    }

    public WeaponItemSave GetWeaponSave()
    {
      WeaponItemSave save = new WeaponItemSave();
      save.Guid = Guid;
      save.Ammo = Ammo;
      save.IsEquipped = IsEquipped;
      return save;
    }

  }
}