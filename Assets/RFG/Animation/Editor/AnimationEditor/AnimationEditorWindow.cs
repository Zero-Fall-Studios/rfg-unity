using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace RFG
{
  public class AnimationsEditorWindow : EditorWindow
  {

    [MenuItem("RFG/Animation Editor Window")]
    public static void ShowWindow()
    {
      GetWindow<AnimationsEditorWindow>("AnimationEditorWindow");
    }

    public virtual void CreateGUI()
    {
      VisualElement root = rootVisualElement;

      VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RFG/Animation/Editor/AnimationEditor/AnimationEditorWindow.uxml");
      visualTree.CloneTree(root);

      StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/RFG/Animation/Editor/AnimationEditor/AnimationEditorWindow.uss");
      root.styleSheets.Add(styleSheet);

      VisualElement mainContainer = root.Q<VisualElement>("container");

      mainContainer.Add(CreateManager());
    }

    private VisualElement CreateManager()
    {
      VisualElement manager = CreateContainer("manager");

      VisualElement buttons = manager.Q<VisualElement>("manager-buttons");

      Button generateTilemapButton = new Button(() =>
      {
      })
      {
        name = "tilemap-button",
        text = "Generate Tilemap"
      };

      buttons.Add(generateTilemapButton);

      return manager;
    }

    protected VisualElement CreateContainer(string name)
    {
      VisualElement container = new VisualElement();
      container.name = name;
      container.AddToClassList("container");

      Label label = new Label();
      label.name = $"{name}-label";
      label.AddToClassList("container-label");

      VisualElement buttons = new VisualElement();
      buttons.name = $"{name}-buttons";
      buttons.AddToClassList("buttons");

      container.Add(label);
      container.Add(buttons);

      return container;
    }

  }
}