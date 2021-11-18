using System;
using UnityEngine;

namespace RFG
{
  [CreateAssetMenu(fileName = "New Spawn State", menuName = "RFG/Platformer/Character/States/Character State/Spawn")]
  public class SpawnState : State
  {
    public override void Enter(IStateContext context)
    {
      base.Enter(context);

      StateCharacterContext characterContext = context as StateCharacterContext;

      // Reset health
      characterContext.healthBehaviour?.ResetHealth();

      // If its a player then need to respawn at a player spawn
      if (characterContext.character.CharacterType == CharacterType.Player)
      {
        characterContext.character.SetSpawnPosition();
      }

      // Reenable all the components
      characterContext.transform.gameObject.SetActive(true);
      characterContext.controller.SetForce(Vector2.zero);
      characterContext.controller.enabled = true;
      characterContext.character.EnableAllAbilities(true);
      characterContext.character.MovementState.Enabled = true;

    }

    public override Type Execute(IStateContext context)
    {
      // Set the spawn position
      StateCharacterContext characterContext = context as StateCharacterContext;

      if (characterContext.character.SpawnAt != null)
      {
        characterContext.transform.position = characterContext.character.SpawnAt.position;
      }

      return typeof(AliveState);
    }

  }
}
