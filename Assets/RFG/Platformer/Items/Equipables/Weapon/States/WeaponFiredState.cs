using System;
using UnityEngine;

namespace RFG
{
  [CreateAssetMenu(fileName = "New Weapon Fired State", menuName = "RFG/Platformer/Items/Equipable/Weapon/States/Fired")]
  public class WeaponFiredState : WeaponState
  {
    public override Type Execute(Weapon weapon)
    {
      return typeof(WeaponIdleState);
    }
  }
}