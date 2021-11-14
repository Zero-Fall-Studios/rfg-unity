using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace RFG
{
  [CustomEditor(typeof(AnimationMap))]
  public class AnimationsEditor : Editor
  {
    private VisualElement rootElement;
    private Editor editor;

    public void OnEnable()
    {
      rootElement = new VisualElement();

      var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/RFG/Animation/Editor/AnimationEditor/AnimationEditor.uss");
      rootElement.styleSheets.Add(styleSheet);
    }

    public override VisualElement CreateInspectorGUI()
    {
      rootElement.Clear();

      UnityEngine.Object.DestroyImmediate(editor);
      editor = Editor.CreateEditor(this);
      IMGUIContainer container = new IMGUIContainer(() =>
      {
        if (target)
        {
          OnInspectorGUI();
        }
      });
      rootElement.Add(container);

      var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RFG/Animation/Editor/AnimationEditor/AnimationEditor.uxml");
      visualTree.CloneTree(rootElement);

      VisualElement mainContainer = rootElement.Q<VisualElement>("container");

      Button generateButton = new Button()
      {
        text = "Generate"
      };
      generateButton.clicked += () =>
      {
        AnimationSpriteSlicer.Slice((AnimationMap)target);
      };
      mainContainer.Add(generateButton);

      return rootElement;
    }

  }
}