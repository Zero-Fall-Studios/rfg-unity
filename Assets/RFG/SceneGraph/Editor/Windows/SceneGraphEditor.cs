using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Callbacks;

namespace RFG.SceneGraph
{
  public class SceneGraphEditor : EditorWindow
  {
    private SceneGraph graph;
    private SceneGraphView graphView;
    private VisualElement overlay;
    private InspectorView inspectorView;

    [MenuItem("RFG/Scene Graph Editor")]
    public static void OpenWindow()
    {
      SceneGraphEditor wnd = GetWindow<SceneGraphEditor>();
      wnd.titleContent = new GUIContent("SceneGraphEditor");
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
      if (Selection.activeObject is SceneGraph)
      {
        OpenWindow();
        return true;
      }
      return false;
    }

    public void CreateGUI()
    {
      AddToolbar();
      AddStyles();
      AddGraphView();
    }

    #region Elements
    private void AddStyles()
    {
      rootVisualElement.AddStyleSheets("Variables.uss");
      rootVisualElement.AddStyleSheets("SceneGraphEditor.uss");
    }

    private void AddGraphView()
    {
      var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RFG/SceneGraph/UIBuilder/SceneGraphEditor.uxml");
      visualTree.CloneTree(rootVisualElement);

      graphView = rootVisualElement.Q<SceneGraphView>();
      graphView.OnSceneSelected = OnSceneSelectionChanged;
      graphView.OnSceneTransitionSelected = OnSceneTransitionSelectionChanged;

      inspectorView = rootVisualElement.Q<InspectorView>();

      overlay = rootVisualElement.Q<VisualElement>("Overlay");

      if (graph == null)
      {
        OnSelectionChange();
      }
      else
      {
        SelectGraph(graph);
      }
    }

    private void AddToolbar()
    {
      Toolbar toolbar = new Toolbar();

      Button createSceneManagerButton = new Button(() =>
      {
        if (graph != null)
        {
          GameObject sceneManager = GameObject.Find("SceneGraphManager");
          if (sceneManager == null)
          {
            sceneManager = new GameObject();
            sceneManager.name = "SceneGraphManager";
            SceneGraphManager manager = sceneManager.AddComponent<SceneGraphManager>();
            manager.SceneGraph = graph;
          }
          else
          {
            LogExt.Warn<SceneGraphEditor>("SceneGraphManager already exists in scene");
          }
        }
        else
        {
          LogExt.Warn<SceneGraphEditor>("Graph not selected.");
        }
      })
      {
        text = "Add Scene Manager To Scene"
      };

      toolbar.Add(createSceneManagerButton);
      toolbar.AddStyleSheets("ToolbarStyles.uss");

      rootVisualElement.Add(toolbar);
    }
    #endregion

    #region Events
    private void OnSelectionChange()
    {
      EditorApplication.delayCall += () =>
      {
        SceneGraph sceneGraph = Selection.activeObject as SceneGraph;
        if (!sceneGraph)
        {
          return;
        }
        SelectGraph(sceneGraph);
      };
    }

    private void SelectGraph(SceneGraph sceneGraph)
    {
      if (graphView == null)
      {
        return;
      }

      if (!sceneGraph)
      {
        return;
      }

      this.graph = sceneGraph;

      if (Application.isPlaying)
      {
        graphView.PopulateView(graph);
      }
      else
      {
        graphView.PopulateView(graph);
      }

      EditorApplication.delayCall += () =>
      {
        graphView.FrameAll();
      };
    }

    private void OnSceneSelectionChanged(SceneGroup SceneGroup)
    {
      inspectorView.UpdateSelection(SceneGroup);
    }

    private void OnSceneTransitionSelectionChanged(SceneTransitionNode SceneTransitionNode)
    {
      inspectorView.UpdateTransitionSelection(SceneTransitionNode);
    }
    #endregion
  }
}