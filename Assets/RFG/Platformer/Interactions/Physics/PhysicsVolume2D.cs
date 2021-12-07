using UnityEngine;
using UnityEngine.Events;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Interactions/Physics/PhysicsVolume2D")]
  public class PhysicsVolume2D : MonoBehaviour
  {
    private static PhysicsVolume2D CurrentVolume;
    [field: SerializeField] private LayerMask LayerMask { get; set; }
    [field: SerializeField] private CharacterControllerParameters2D OverrideParameters { get; set; }
    [field: SerializeField] private bool HasEnterForce { get; set; }
    [field: SerializeField] private bool HasExitForce { get; set; }
    [field: SerializeField] private Vector2 EnterForce { get; set; }
    [field: SerializeField] private Vector2 ExitForce { get; set; }
    [field: SerializeField] private State EnterCharacterState { get; set; }
    [field: SerializeField] private State EnterMovementState { get; set; }
    [field: SerializeField] private State ExitCharacterState { get; set; }
    [field: SerializeField] private State ExitMovementState { get; set; }
    [field: SerializeField] private SettingsPack SettingsPack { get; set; }
    [field: SerializeField] private UnityEvent OnBeforeEnter { get; set; }
    [field: SerializeField] private UnityEvent OnBeforeExit { get; set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (!LayerMask.Contains(other.gameObject.layer))
      {
        return;
      }

      OnBeforeEnter?.Invoke();
      CurrentVolume = this;
      Character character = other.gameObject.GetComponentInParent<Character>();

      if (character != null)
      {
        if (EnterCharacterState != null)
        {
          character.CharacterState.ChangeState(EnterCharacterState.GetType());
        }
        if (EnterMovementState != null)
        {
          character.MovementState.ChangeState(EnterMovementState.GetType());
        }
        if (SettingsPack != null)
        {
          character.OverrideSettingsPack(SettingsPack);
        }
      }

      CharacterController2D characterController = other.gameObject.GetComponentInParent<CharacterController2D>();
      if (characterController != null)
      {
        if (EnterForce != null && HasEnterForce)
        {
          characterController.SetForce(EnterForce);
        }
        if (OverrideParameters != null)
        {
          characterController.SetOverrideParameters(OverrideParameters);
        }
      }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
      if (!LayerMask.Contains(other.gameObject.layer))
      {
        return;
      }

      if (CurrentVolume != null && CurrentVolume != this)
      {
        return;
      }

      OnBeforeExit?.Invoke();

      Character character = other.gameObject.GetComponentInParent<Character>();

      if (character != null)
      {
        if (ExitCharacterState != null)
        {
          character.CharacterState.ChangeState(ExitCharacterState.GetType());
        }
        if (ExitMovementState != null)
        {
          character.MovementState.ChangeState(ExitMovementState.GetType());
        }
        character.ResetSettingsPack();
      }

      CharacterController2D characterController = other.gameObject.GetComponentInParent<CharacterController2D>();
      if (characterController != null)
      {
        if (ExitForce != null && HasExitForce)
        {
          characterController.SetForce(ExitForce);
        }
        characterController.SetOverrideParameters(null);
      }
    }
  }
}