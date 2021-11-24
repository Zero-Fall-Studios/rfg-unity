using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace RFG
{
  [CustomEditor(typeof(Effect))]
  public class EffectControls : Editor
  {
    private VisualElement rootElement;
    private Editor editor;

    public void OnEnable()
    {
      rootElement = new VisualElement();

      var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/RFG/Effects/Editor/EffectControls/EffectControls.uss");
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

      var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RFG/Effects/Editor/EffectControls/EffectControls.uxml");
      visualTree.CloneTree(rootElement);

      Effect audioTarget = (Effect)target;

      Button generateAudioSourceButton = rootElement.Q<Button>("generate-audio-source");
      generateAudioSourceButton.clicked += () =>
      {
        if (audioTarget.EffectData.soundEffects == null || audioTarget.EffectData.soundEffects.Count == 0)
        {
          LogExt.Warn<EffectControls>("Effect Data does not have any sound effects.");
          return;
        }
        List<AudioSource> audioSources = new List<AudioSource>(audioTarget.gameObject.GetComponents<AudioSource>());
        for (int i = 0; i < audioTarget.EffectData.soundEffects.Count; i++)
        {
          if (audioSources.Count > 0)
          {
            DestroyImmediate(audioSources[i]);
          }
          AudioData audioData = audioTarget.EffectData.soundEffects[i];
          AudioSource audioSource = audioTarget.gameObject.AddComponent<AudioSource>();
          audioData.ConfigureAudioSource(audioSource);
        }
        EditorUtility.SetDirty(audioTarget);
      };

      return rootElement;

    }
  }
}