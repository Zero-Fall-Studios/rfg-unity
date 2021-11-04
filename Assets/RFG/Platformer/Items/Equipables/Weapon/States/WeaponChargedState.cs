using System;
using UnityEngine;

namespace RFG
{
  [CreateAssetMenu(fileName = "New Weapon Charged State", menuName = "RFG/Platformer/Items/Equipable/Weapon/States/Charged")]
  public class WeaponChargedState : WeaponState
  {
    public override Type Execute(Weapon weapon)
    {
      return null;
    }
  }
}