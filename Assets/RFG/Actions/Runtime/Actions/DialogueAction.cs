using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RFG.Actions
{
  [Serializable]
  [ActionMenu("Dialogue/Dialogue")]
  public class DialogueAction : Action
  {
    public string dialogue;
    public float waitAfter = 0f;
    public bool canSkip = false;
    private State _currentState = State.Init;

    private List<KeyValuePair<DialogueText, string>> _parsedDialogue;
    private KeyValuePair<DialogueText, string> _currentDialogue;
    private int _currentDialogueIndex = 0;

    public override void Init()
    {
      _parsedDialogue = ParseDialogue();
      _currentDialogue = _parsedDialogue[_currentDialogueIndex];
    }
    public override State Run()
    {
      if (_currentState == State.Init)
      {
        _currentState = State.Running;
        ProcessDialogue();
      }
      return _currentState;
    }

    private void ProcessDialogue()
    {
      RemoveListeners();
      AddListeners();
      _currentDialogue.Key.Say(_currentDialogue.Value, waitAfter);
    }

    private void AddListeners()
    {
      _currentDialogue.Key.onStart.AddListener(onStart);
      _currentDialogue.Key.onComplete.AddListener(onComplete);
    }

    private void RemoveListeners()
    {
      _currentDialogue.Key.onStart.RemoveListener(onStart);
      _currentDialogue.Key.onComplete.RemoveListener(onComplete);
    }

    private void onStart()
    {
    }

    private void onComplete()
    {
      RemoveListeners();
      if (_currentDialogueIndex + 1 == _parsedDialogue.Count)
      {
        _currentState = State.Success;
      }
      else
      {
        _currentDialogue = _parsedDialogue[++_currentDialogueIndex];
        ProcessDialogue();
      }
    }

    private List<KeyValuePair<DialogueText, string>> ParseDialogue()
    {
      List<KeyValuePair<DialogueText, string>> list = new List<KeyValuePair<DialogueText, string>>();
      Dictionary<string, DialogueText> dialogueTexts = new Dictionary<string, DialogueText>();

      string[] lines = dialogue.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      foreach (string line in lines)
      {
        string[] speakerLine = line.Trim().Split(':');

        if (speakerLine.Length != 2)
        {
          throw new Exception("You must have your line in this format '[GameObject<DialogueText>]: Text'. Text must not have any extra ':'");
        }
        string speaker = speakerLine[0];
        string speakerText = speakerLine[1];

        if (!dialogueTexts.ContainsKey(speaker))
        {
          GameObject speakerGameObject = GameObject.Find(speaker);
          DialogueText dialogueText = speakerGameObject.GetComponent<DialogueText>();
          if (dialogueText != null)
          {
            dialogueTexts.Add(speaker, dialogueText);
          }
          else
          {
            throw new Exception($"{speaker} does not have a DialogueText component.");
          }
        }

        list.Add(new KeyValuePair<DialogueText, string>(dialogueTexts[speaker], speakerText.Trim()));
      }

      return list;
    }

#if UNITY_EDITOR
    public override void Draw(ActionNode node)
    {
      VisualElement container = node.Q<VisualElement>("container");
      IMGUIContainer guiContainer = new IMGUIContainer(() =>
      {
        dialogue = EditorGUILayout.TextArea(dialogue);
        waitAfter = EditorGUILayout.FloatField("Wait After:", waitAfter);
        canSkip = EditorGUILayout.Toggle("Can Skip:", canSkip);
      });
      container.Add(guiContainer);
    }
#endif

  }
}