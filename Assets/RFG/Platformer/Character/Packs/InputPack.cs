using UnityEngine;
using UnityEngine.InputSystem;

namespace RFG
{
  [CreateAssetMenu(fileName = "New Input Pack", menuName = "RFG/Platformer/Character/Packs/Input")]
  public class InputPack : ScriptableObject
  {
    /// <summary>Input Action that maps to the movement binding</summary>
    [Tooltip("Input Action that maps to the movement binding")]
    public InputActionReference Movement;

    /// <summary>Input Action that maps to the jump binding</summary>
    [Tooltip("Input Action to initiate the jump binding")]
    public InputActionReference JumpInput;

    /// <summary>Input Action that maps to the dash binding</summary>
    [Tooltip("Input Action that maps to the dash binding")]
    public InputActionReference DashInput;

    /// <summary>Input Action that maps to the primary attack binding</summary>
    [Tooltip("Input Action that maps to the primary attack binding")]
    public InputActionReference PrimaryAttackInput;

    /// <summary>Input Action that maps to the secondary attack binding</summary>
    [Tooltip("Input Action that maps to the secondary attack binding")]
    public InputActionReference SecondaryAttackInput;

    /// <summary>Input Action that maps to the use binding</summary>
    [Tooltip("Input Action that maps to the use binding")]
    public InputActionReference UseInput;

    /// <summary>Input Action that maps to the pause binding</summary>
    [Tooltip("Input Action that maps to the pause binding")]
    public InputActionReference PauseInput;


  }
}
