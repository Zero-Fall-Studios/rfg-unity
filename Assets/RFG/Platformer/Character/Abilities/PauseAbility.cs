using UnityEngine;
using UnityEngine.InputSystem;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Ability/Pause")]
  public class PauseAbility : MonoBehaviour, IAbility
  {
    private Character _character;
    private PlayerInput _playerInput;
    private InputAction _pauseInput;
    private SettingsPack _settings;

    private void Start()
    {
      _character = GetComponent<Character>();
      _playerInput = GetComponent<PlayerInput>();
      _pauseInput = _playerInput.actions["Pause"];
      _settings = _character.Context.settingsPack;

      OnEnable();
    }

    public void OnPausedPerformed(InputAction.CallbackContext ctx)
    {
      if (GameManager.Instance.IsPaused)
      {
        _settings.UnPauseEvent?.Raise();
      }
      else
      {
        _settings.PauseEvent?.Raise();
      }
    }

    private void OnEnable()
    {
      OnDisable();
      if (_pauseInput != null)
      {
        _pauseInput.performed += OnPausedPerformed;
      }
    }

    private void OnDisable()
    {
      if (_pauseInput != null)
      {
        _pauseInput.performed -= OnPausedPerformed;
      }
    }

  }
}