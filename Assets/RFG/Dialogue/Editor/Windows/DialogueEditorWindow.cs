using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace RFG.Dialogue
{
  public class DialogueEditorWindow : EditorWindow
  {
    private DialogueGraphView graphView;
    private readonly string defaultFileName = "DialoguesFileName";
    private static TextField fileNameTextField;
    private Button saveButton;
    private Button miniMapButton;

    [MenuItem("RFG/Dialogue Graph Editor")]
    public static void OpenWindow()
    {
      GetWindow<DialogueEditorWindow>("DSEditorWindow");
    }

    private void CreateGUI()
    {
      AddGraphView();
      AddToolbar();
      AddSyles();
    }

    #region Elements

    private void AddGraphView()
    {
      graphView = new DialogueGraphView(this);
      graphView.StretchToParentSize();
      rootVisualElement.Add(graphView);
    }

    private void AddToolbar()
    {
      Toolbar toolbar = new Toolbar();

      fileNameTextField = ElementUtility.CreateTextField(defaultFileName, "File Name:", callback =>
      {
        fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
      });
      saveButton = ElementUtility.CreateButton("Save", () => Save());

      Button loadButton = ElementUtility.CreateButton("Load", () => Load());
      Button clearButton = ElementUtility.CreateButton("Clear", () => Clear());
      Button resetButton = ElementUtility.CreateButton("Reset", () => ResetGraph());
      miniMapButton = ElementUtility.CreateButton("Minimap", () => ToggleMiniMap());

      toolbar.Add(fileNameTextField);
      toolbar.Add(saveButton);
      toolbar.Add(loadButton);
      toolbar.Add(clearButton);
      toolbar.Add(resetButton);
      toolbar.Add(miniMapButton);

      toolbar.AddStyleSheets("ToolbarStyles.uss");

      rootVisualElement.Add(toolbar);
    }

    private void AddSyles()
    {
      rootVisualElement.AddStyleSheets("Variables.uss");
    }

    #endregion

    #region Utilities
    public void EnableSaving(bool save)
    {
      saveButton.SetEnabled(save);
    }

    private void Save()
    {
      if (string.IsNullOrEmpty(fileNameTextField.value))
      {
        EditorUtility.DisplayDialog("Invalid file name.", "Please ensure the file name you've typed in is valid.", "Roger!");
        return;
      }
      IOUtility.Initialize(graphView, fileNameTextField.value);
      IOUtility.Save();
    }

    private void Load()
    {
      string filePath = EditorUtility.OpenFilePanel("Dialogue Graphs", "Assets/Editor/DialogueSystem/Graphs", "asset");

      if (string.IsNullOrEmpty(filePath))
      {
        return;
      }
      Clear();
      IOUtility.Initialize(graphView, Path.GetFileNameWithoutExtension(filePath));
      IOUtility.Load();
    }

    private void Clear()
    {
      graphView.ClearGraph();
    }

    private void ResetGraph()
    {
      Clear();
      UpdateFileName(defaultFileName);
    }

    public static void UpdateFileName(string newFilename)
    {
      fileNameTextField.value = newFilename;
    }

    private void ToggleMiniMap()
    {
      graphView.ToggleMiniMap();
      miniMapButton.ToggleInClassList("ds-toolbar__button__selected");
    }
    #endregion

  }
}