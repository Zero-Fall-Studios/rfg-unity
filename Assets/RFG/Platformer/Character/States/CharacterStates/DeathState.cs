using System;
using UnityEngine;

namespace RFG
{
  [CreateAssetMenu(fileName = "New Death State", menuName = "RFG/Platformer/Character/States/Character State/Death")]
  public class DeathState : State
  {
    public override void Enter(IStateContext context)
    {
      StateCharacterContext characterContext = context as StateCharacterContext;
      characterContext.controller.enabled = false;
      characterContext.character.MovementState.Enabled = false;
      characterContext.character.EnableAllAbilities(false);
      base.Enter(context);
    }

    public override Type Execute(IStateContext context)
    {
      return typeof(DeadState);
    }
  }
}