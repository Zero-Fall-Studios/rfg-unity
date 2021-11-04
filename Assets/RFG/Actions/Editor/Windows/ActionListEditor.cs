using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RFG.Actions
{
  public class ActionListEditor : EditorWindow
  {
    private ActionList actionList;
    private ActionListView actionListView;

    [MenuItem("RFG/Action List Editor")]
    public static void OpenWindow()
    {
      ActionListEditor wnd = GetWindow<ActionListEditor>();
      wnd.titleContent = new GUIContent("ActionListEditor");
    }

    public void CreateGUI()
    {
      AddStyles();
      AddActionListView();
    }

    private void AddStyles()
    {
      rootVisualElement.AddStyleSheets("ActionListEditor.uss");
    }

    private void AddActionListView()
    {
      var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RFG/Actions/UIBuilder/ActionListEditor.uxml");
      visualTree.CloneTree(rootVisualElement);
      actionListView = rootVisualElement.Q<ActionListView>();
      if (actionList == null)
      {
        OnSelectionChange();
      }
      else
      {
        SelectActionList(actionList);
      }
    }

    private void OnSelectionChange()
    {
      EditorApplication.delayCall += () =>
      {
        GameObject gameObject = Selection.activeObject as GameObject;
        if (gameObject == null)
        {
          return;
        }
        ActionList actionList = gameObject.GetComponent<ActionList>();
        if (!actionList)
        {
          return;
        }
        SelectActionList(actionList);
      };
    }

    private void SelectActionList(ActionList actionList)
    {
      if (actionListView == null)
      {
        return;
      }

      if (!actionList)
      {
        return;
      }

      this.actionList = actionList;

      if (Application.isPlaying)
      {
        actionListView.PopulateView(actionList);
      }
      else
      {
        actionListView.PopulateView(actionList);
      }

      EditorApplication.delayCall += () =>
      {
        actionListView.FrameAll();
      };
    }
  }
}