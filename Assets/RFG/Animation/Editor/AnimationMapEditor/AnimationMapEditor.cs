using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace RFG
{
  [CustomEditor(typeof(AnimationMap))]
  public class AnimationMapEditor : Editor
  {
    private VisualElement rootElement;
    private Editor editor;

    public void OnEnable()
    {
      rootElement = new VisualElement();

      var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/RFG/Animation/Editor/AnimationMapEditor/AnimationMapEditor.uss");
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

      var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RFG/Animation/Editor/AnimationMapEditor/AnimationMapEditor.uxml");
      visualTree.CloneTree(rootElement);

      VisualElement mainContainer = rootElement.Q<VisualElement>("container");

      Button sliceButton = new Button()
      {
        text = "Slice"
      };
      sliceButton.clicked += () =>
      {
        AnimationMapSpriteSlicer.Slice((AnimationMap)target);
      };
      mainContainer.Add(sliceButton);

      TextField animatorControllerPath = new TextField()
      {
        label = "Animator Controller Path"
      };

      mainContainer.Add(animatorControllerPath);

      Button generateClipsButton = new Button()
      {
        text = "Generate Clips"
      };
      generateClipsButton.clicked += () =>
      {
        AnimationMapCreateClips.Create((AnimationMap)target, animatorControllerPath.value);
      };
      mainContainer.Add(generateClipsButton);

      return rootElement;
    }

  }
}