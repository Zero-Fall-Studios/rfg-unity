using System;
using UnityEngine;

namespace RFG
{
  [CreateAssetMenu(fileName = "New Weapon Charging State", menuName = "RFG/Platformer/Items/Equipable/Weapon/States/Charging")]
  public class WeaponChargingState : WeaponState
  {
    public override Type Execute(Weapon weapon)
    {
      return typeof(WeaponChargedState);
    }
  }
}