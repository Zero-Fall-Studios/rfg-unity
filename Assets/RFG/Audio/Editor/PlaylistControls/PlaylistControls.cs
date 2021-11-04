using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


namespace RFG
{
  [CustomEditor(typeof(Playlist))]
  public class PlaylistControls : Editor
  {
    private VisualElement rootElement;
    private Editor editor;

    public void OnEnable()
    {
      rootElement = new VisualElement();

      var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/RFG/Audio/Editor/PlaylistControls/PlaylistControls.uss");
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

      var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RFG/Audio/Editor/PlaylistControls/PlaylistControls.uxml");
      visualTree.CloneTree(rootElement);

      Playlist playlistTarget = (Playlist)target;

      Button generateAudioSourceButton = rootElement.Q<Button>("generate-audio-source");
      generateAudioSourceButton.clicked += () =>
      {
        playlistTarget.GenerateAudioSource();
      };

      Button previousButton = rootElement.Q<Button>("previous");
      previousButton.clicked += () =>
      {
        playlistTarget.Previous();
      };
      Button stopButton = rootElement.Q<Button>("stop");
      stopButton.clicked += () =>
      {
        playlistTarget.Stop();
      };
      Button playButton = rootElement.Q<Button>("play");
      playButton.clicked += () =>
      {
        playlistTarget.Play();
      };
      Button pauseButton = rootElement.Q<Button>("pause");
      pauseButton.clicked += () =>
      {
        playlistTarget.TogglePause();
      };
      Button nextButton = rootElement.Q<Button>("next");
      nextButton.clicked += () =>
      {
        playlistTarget.Next();
      };

      return rootElement;

    }
  }
}