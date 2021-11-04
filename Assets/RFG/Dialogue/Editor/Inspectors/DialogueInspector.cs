using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RFG.Dialogue
{
  [CustomEditor(typeof(DialogueController))]
  public class DialogueInspector : Editor
  {
    private SerializedProperty dialogueContainerProperty;
    private SerializedProperty dialogueGroupProperty;
    private SerializedProperty dialogueProperty;
    private SerializedProperty groupedDialoguesProperty;
    private SerializedProperty startingDialoguesOnlyProperty;

    public override void OnInspectorGUI()
    {

    }

    private void OnEnable()
    {
      dialogueContainerProperty = serializedObject.FindProperty("dialogueContainer");
      dialogueGroupProperty = serializedObject.FindProperty("dialogueGroupData");
      dialogueProperty = serializedObject.FindProperty("dialogue");
      groupedDialoguesProperty = serializedObject.FindProperty("groupedDialogues");
      startingDialoguesOnlyProperty = serializedObject.FindProperty("startingDialoguesOnly");
    }
  }
}