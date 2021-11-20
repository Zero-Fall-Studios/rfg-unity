using UnityEngine;

namespace RFG
{
  [CreateAssetMenu(fileName = "New Smash Down Started State", menuName = "RFG/Platformer/Character/States/Movement State/Smash Down Started")]
  public class SmashDownStartedState : State
  {
    public override void Enter(IStateContext context)
    {
      StateCharacterContext characterContext = context as StateCharacterContext;
      characterContext.controller.SetForce(Vector2.zero);
      characterContext.controller.GravityActive(false);
      characterContext.character.EnableAllInput(false);
      base.Enter(context);
    }
  }
}