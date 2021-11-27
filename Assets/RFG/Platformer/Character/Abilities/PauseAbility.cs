using UnityEngine;
using UnityEngine.InputSystem;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Ability/Pause")]
  public class PauseAbility : MonoBehaviour, IAbility
  {
    private Character _character;
    private InputActionReference _pauseInput;
    private SettingsPack _settings;

    private void Start()
    {
      _character = GetComponent<Character>();
      _pauseInput = _character.Context.inputPack.PauseInput;
      _settings = _character.Context.settingsPack;

      // Setup events
      OnEnable();
    }

    public void OnPausedPerformed(InputAction.CallbackContext ctx)
    {
      _settings.PauseEvent?.Raise();
      _character.EnableAllInput(!GameManager.Instance.IsPaused);
    }

    private void OnEnable()
    {
      if (_pauseInput != null)
      {
        _pauseInput.action.Enable();
        _pauseInput.action.performed += OnPausedPerformed;
      }
    }

    private void OnDisable()
    {
      if (_pauseInput != null)
      {
        _pauseInput.action.Disable();
        _pauseInput.action.performed -= OnPausedPerformed;
      }
    }

  }
}