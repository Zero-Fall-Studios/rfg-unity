using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace RFG
{
  [AddComponentMenu("RFG/Input/Input Action Map To Text")]
  public class InputActionMapToText : MonoBehaviour
  {
    [field: SerializeField] private InputActionReference InputActionReference { get; set; }
    [field: SerializeField] private int Index { get; set; }
    [field: SerializeField] private string BeforeText { get; set; }
    [field: SerializeField] private string AfterText { get; set; }
    [field: SerializeField] public string InputActionText { get; private set; }

    private TMP_Text _text;

    private void Awake()
    {
      InputActionText = BeforeText + InputActionReference.action.GetBindingDisplayString(Index) + AfterText;
      _text = GetComponent<TMP_Text>();
      _text.SetText(InputActionText);
    }
  }
}