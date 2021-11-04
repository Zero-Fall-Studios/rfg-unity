using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace RFG.Transitions
{
  [CustomEditor(typeof(Transition))]
  public class TransitionControls : Editor
  {
    private VisualElement rootElement;
    private Editor editor;

    public void OnEnable()
    {
      rootElement = new VisualElement();

      var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/RFG/Transitions/Editor/TransitionControls/TransitionControls.uss");
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

      var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RFG/Transitions/Editor/TransitionControls/TransitionControls.uxml");
      visualTree.CloneTree(rootElement);

      Transition TransitionTarget = (Transition)target;

      Button stopButton = rootElement.Q<Button>("stop");
      stopButton.clicked += () =>
      {
        TransitionTarget.Cancel();
      };
      Button playButton = rootElement.Q<Button>("play");
      playButton.clicked += () =>
      {
        TransitionTarget.Play();
      };
      Button pauseButton = rootElement.Q<Button>("pause");
      pauseButton.clicked += () =>
      {
        TransitionTarget.TogglePause();
      };
      Button repeatButton = rootElement.Q<Button>("repeat");
      repeatButton.clicked += () =>
      {
        TransitionTarget.Play(true);
      };

      return rootElement;

    }
  }
}