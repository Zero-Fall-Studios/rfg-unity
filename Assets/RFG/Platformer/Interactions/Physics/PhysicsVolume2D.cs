using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Interactions/Physics/PhysicsVolume2D")]
  public class PhysicsVolume2D : MonoBehaviour
  {
    public CharacterControllerParameters2D parameters;

    private void OnTriggerEnter2D(Collider2D other)
    {
      CharacterController2D character = other.gameObject.GetComponent<CharacterController2D>();
      if (character != null)
      {
        character.SetForce(Vector2.zero);
        character.SetOverrideParameters(parameters);
      }
      // TODO - also add override for settings pack or input
    }

    private void OnTriggerExit2D(Collider2D other)
    {
      CharacterController2D character = other.gameObject.GetComponent<CharacterController2D>();
      if (character != null)
      {
        character.SetForce(Vector2.zero);
        character.SetOverrideParameters(null);
      }
      // TODO - also add override for settings pack or input
    }
  }
}