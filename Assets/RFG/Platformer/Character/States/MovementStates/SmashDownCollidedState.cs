using System;
using UnityEngine;

namespace RFG
{
  [CreateAssetMenu(fileName = "New Smash Down Collided State", menuName = "RFG/Platformer/Character/States/Movement State/Smash Down Collided")]
  public class SmashDownCollidedState : State
  {
    public override Type Execute(IStateContext context)
    {
      return typeof(IdleState);
    }

    public override void Exit(IStateContext context)
    {
      StateCharacterContext characterContext = context as StateCharacterContext;
      characterContext.controller.SetForce(Vector2.zero);
      characterContext.controller.GravityActive(true);
      characterContext.character.EnableAllInput(true);
      base.Exit(context);
    }
  }
}
