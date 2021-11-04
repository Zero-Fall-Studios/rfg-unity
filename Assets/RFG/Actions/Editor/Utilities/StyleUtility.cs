using UnityEditor;
using UnityEngine.UIElements;

namespace RFG.Actions
{
  public static class StyleUtility
  {
    public static VisualElement AddClasses(this VisualElement element, params string[] classNames)
    {
      foreach (string className in classNames)
      {
        element.AddToClassList(className);
      }
      return element;
    }

    public static VisualElement AddStyleSheets(this VisualElement element, params string[] styleSheetNames)
    {
      foreach (string styleSheetName in styleSheetNames)
      {
        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"Assets/RFG/Actions/UIBuilder/{styleSheetName}");
        element.styleSheets.Add(styleSheet);
      }
      return element;
    }
  }
}

