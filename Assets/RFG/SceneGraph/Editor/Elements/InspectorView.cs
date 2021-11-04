using UnityEngine.UIElements;
using UnityEditor;

namespace RFG.SceneGraph
{
  public class InspectorView : VisualElement
  {
    public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }
    private Editor editor;

    public InspectorView()
    {
    }

    internal void UpdateSelection(SceneGroup SceneGroup)
    {
      Clear();
      UnityEngine.Object.DestroyImmediate(editor);
      editor = Editor.CreateEditor(SceneGroup.scene);
      IMGUIContainer container = new IMGUIContainer(() =>
      {
        if (editor && editor.target)
        {
          editor.OnInspectorGUI();
        }
        if (!SceneGroup.title.Equals(SceneGroup.scene.scenePath))
        {
          SceneGroup.title = System.IO.Path.GetFileNameWithoutExtension(SceneGroup.scene.scenePath);
        }
      });
      Add(container);
    }

    internal void UpdateTransitionSelection(SceneTransitionNode SceneTransitionNode)
    {
      Clear();
      UnityEngine.Object.DestroyImmediate(editor);
      editor = Editor.CreateEditor(SceneTransitionNode.sceneTransition);
      IMGUIContainer container = new IMGUIContainer(() =>
      {
        if (editor && editor.target)
        {
          editor.OnInspectorGUI();
        }
      });
      Add(container);
    }
  }
}