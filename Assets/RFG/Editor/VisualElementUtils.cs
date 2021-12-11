using UnityEditor;
using UnityEngine.UIElements;

namespace RFG
{
  public static class VisualElementUtils
  {
    public static void CloneRootTree(this VisualElement root)
    {
      VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RFG/Editor/EditorWindow.uxml");
      visualTree.CloneTree(root);
    }

    public static StyleSheet LoadRootStyles(this VisualElement root)
    {
      StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/RFG/Editor/Styles.uss");
      root.styleSheets.Add(styleSheet);
      return styleSheet;
    }

    public static Label CreateTitle(string text)
    {
      Label label = new Label()
      {
        text = text
      };
      label.AddToClassList("title");
      return label;
    }

    public static VisualElement CreateButtonContainer(string name)
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

    public static VisualElement CreateControlsContainer(string name)
    {
      VisualElement container = new VisualElement();
      container.name = name;
      container.AddToClassList("container");

      Label label = new Label();
      label.name = $"{name}-label";
      label.AddToClassList("container-label");

      VisualElement controls = new VisualElement();
      controls.name = $"{name}-controls";
      controls.AddToClassList("cotrols");

      container.Add(label);
      container.Add(controls);

      return container;
    }
  }
}