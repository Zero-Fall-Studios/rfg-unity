using UnityEngine;

namespace RFG.Dialogue
{
  public class DialogueController : MonoBehaviour
  {
    [SerializeField] private DialogueContainer dialogueContainer;
    [SerializeField] private DialogueGroupData dialogueGroupData;
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private bool groupedDialogues;
    [SerializeField] private bool startingDialoguesOnly;
  }
}