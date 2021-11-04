using UnityEngine;
using UnityEngine.InputSystem;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Ability/Pause")]
  public class PauseAbility : MonoBehaviour, IAbility
  {
    [HideInInspector]
    private Transform _transform;
    private Character _character;
    private InputActionReference _pauseInput;
    private SettingsPack _settings;

    private void Awake()
    {
      _transform = transform;
    }

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
    }

    private void OnEnable()
    {
      // Make sure to setup new events
      OnDisable();

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