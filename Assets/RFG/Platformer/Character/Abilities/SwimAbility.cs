using UnityEngine;
using UnityEngine.InputSystem;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Ability/Swim")]
  public class SwimAbility : MonoBehaviour, IAbility
  {
    private Character _character;
    private Transform _transform;
    private CharacterController2D _controller;
    private CharacterControllerState2D _state;
    private PlayerInput _playerInput;
    private InputAction _swimInput;
    private SettingsPack _settings;

    #region Unity Methods
    private void Awake()
    {
      _transform = transform;
      _character = GetComponent<Character>();
      _controller = GetComponent<CharacterController2D>();
      _playerInput = GetComponent<PlayerInput>();
      _swimInput = _playerInput.actions["Jump"];
      _settings = _character.SettingsPack;
    }

    private void Start()
    {
      _state = _controller.State;
    }
    private void OnEnable()
    {
      if (_swimInput != null)
      {
        _swimInput.started += OnSwimStarted;
      }
    }

    private void OnDisable()
    {
      if (_swimInput != null)
      {
        _swimInput.started -= OnSwimStarted;
      }
    }
    #endregion

    private void HandleSwim()
    {
      if (!_character.IsSwimming)
      {
        return;
      }
      _controller.SetVerticalForce(Mathf.Sqrt(2f * _settings.SwimHeight * Mathf.Abs(_controller.Parameters.Gravity)));
    }

    #region Events
    private void OnSwimStarted(InputAction.CallbackContext ctx)
    {
      HandleSwim();
    }
    #endregion

  }
}