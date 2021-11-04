using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace RFG
{
  [AddComponentMenu("RFG/Input/Input Action Event")]
  public class InputActionEvent : MonoBehaviour
  {
    [field: SerializeField] private InputActionReference InputActionReference { get; set; }
    [field: SerializeField] private UnityEvent OnStarted;
    [field: SerializeField] private UnityEvent OnCanceled;
    [field: SerializeField] private UnityEvent OnPerformed;

    private void OnStartedCallback(InputAction.CallbackContext ctx)
    {
      OnStarted?.Invoke();
    }

    private void OnCanceledCallback(InputAction.CallbackContext ctx)
    {
      OnCanceled?.Invoke();
    }

    private void OnPerformedCallback(InputAction.CallbackContext ctx)
    {
      OnPerformed?.Invoke();
    }

    private void OnEnable()
    {
      InputActionReference.action.Enable();
      InputActionReference.action.started += OnStartedCallback;
      InputActionReference.action.canceled += OnCanceledCallback;
      InputActionReference.action.performed += OnPerformedCallback;
    }

    private void OnDisable()
    {
      InputActionReference.action.Disable();
      InputActionReference.action.started -= OnStartedCallback;
      InputActionReference.action.canceled -= OnCanceledCallback;
      InputActionReference.action.performed -= OnPerformedCallback;
    }
  }
}