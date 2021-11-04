using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace RFG
{
  public class AudioEditorWindow : EditorWindow
  {
    [MenuItem("RFG/Audio Editor Window")]
    public static void ShowWindow()
    {
      GetWindow<AudioEditorWindow>("AudioEditorWindow");
    }

    public virtual void CreateGUI()
    {
      VisualElement root = rootVisualElement;

      VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RFG/Audio/Editor/AudioEditor/AudioEditorWindow.uxml");
      visualTree.CloneTree(root);

      StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/RFG/Audio/Editor/AudioEditor/AudioEditorWindow.uss");
      root.styleSheets.Add(styleSheet);

      VisualElement mainContainer = root.Q<VisualElement>("container");

      mainContainer.Add(CreateManager());
    }

    private VisualElement CreateManager()
    {
      VisualElement audioManager = CreateContainer("audio-manager");

      VisualElement audioManagerButtons = audioManager.Q<VisualElement>("audio-manager-buttons");

      Button addAudioManagerButton = new Button(() =>
      {
        EditorUtils.CreatePrefab("Assets/RFG/Audio/Prefabs", "AudioManager");
      })
      {
        name = "audio-manager-button",
        text = "Add Audio Manager"
      };

      Button addAudioTagsButton = new Button(() =>
      {
        EditorUtils.AddTags(new string[] { "Audio" });
      })
      {
        name = "audio-tags-button",
        text = "Add Audio Tags"
      };

      audioManagerButtons.Add(addAudioManagerButton);
      audioManagerButtons.Add(addAudioTagsButton);

      return audioManager;
    }

    protected VisualElement CreateContainer(string name)
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

  }
}