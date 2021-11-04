using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


namespace RFG
{
  [CustomEditor(typeof(RandomAudio))]
  public class RandomAudioControls : Editor
  {
    private VisualElement rootElement;
    private Editor editor;

    public void OnEnable()
    {
      rootElement = new VisualElement();

      var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/RFG/Audio/Editor/RandomAudioControls/RandomAudioControls.uss");
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

      var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RFG/Audio/Editor/RandomAudioControls/RandomAudioControls.uxml");
      visualTree.CloneTree(rootElement);

      RandomAudio RandomAudioTarget = (RandomAudio)target;

      Button generateAudioSourceButton = rootElement.Q<Button>("generate-audio-source");
      generateAudioSourceButton.clicked += () =>
      {
        RandomAudioTarget.GenerateAudioSource();
      };

      Button playButton = rootElement.Q<Button>("play");
      playButton.clicked += () =>
      {
        RandomAudioTarget.PlayRandom();
      };

      return rootElement;

    }
  }
}