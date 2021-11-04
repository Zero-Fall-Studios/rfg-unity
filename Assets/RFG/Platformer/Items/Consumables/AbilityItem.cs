using System.Collections.Generic;
using UnityEngine;

namespace RFG
{
  [CreateAssetMenu(fileName = "New Ability Item", menuName = "RFG/Platformer/Items/Consumable/Ability")]
  public class AbilityItem : Consumable
  {
    public List<State> AbilityStates;

    public override void Consume(Transform transform, Inventory inventory)
    {
      base.Consume(transform, inventory);
      Character character = transform.gameObject.GetComponent<Character>();
      if (character != null)
      {
        AbilityStates.ForEach(state => character.MovementState.Add(state));
      }
    }
  }
}