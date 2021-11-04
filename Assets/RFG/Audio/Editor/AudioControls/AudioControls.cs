using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


namespace RFG
{
  [CustomEditor(typeof(Audio))]
  public class AudioControls : Editor
  {
    private VisualElement rootElement;
    private Editor editor;

    public void OnEnable()
    {
      rootElement = new VisualElement();

      var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/RFG/Audio/Editor/AudioControls/AudioControls.uss");
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

      var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RFG/Audio/Editor/AudioControls/AudioControls.uxml");
      visualTree.CloneTree(rootElement);

      Audio audioTarget = (Audio)target;

      Button generateAudioSourceButton = rootElement.Q<Button>("generate-audio-source");
      generateAudioSourceButton.clicked += () =>
      {
        audioTarget.AudioData.GenerateAudioSource(audioTarget.gameObject);
      };

      Button stopButton = rootElement.Q<Button>("stop");
      stopButton.clicked += () =>
      {
        audioTarget.Stop();
      };
      Button playButton = rootElement.Q<Button>("play");
      playButton.clicked += () =>
      {
        audioTarget.Play();
      };
      Button pauseButton = rootElement.Q<Button>("pause");
      pauseButton.clicked += () =>
      {
        audioTarget.Pause();
      };

      return rootElement;

    }
  }
}