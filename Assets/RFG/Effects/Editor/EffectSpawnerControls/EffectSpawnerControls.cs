using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace RFG
{
  [CustomEditor(typeof(EffectSpawner))]
  public class EffectSpawnerControls : Editor
  {
    private VisualElement rootElement;
    private Editor editor;

    public void OnEnable()
    {
      rootElement = new VisualElement();

      var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/RFG/Effects/Editor/EffectSpawnerControls/EffectSpawnerControls.uss");
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

      var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RFG/Effects/Editor/EffectSpawnerControls/EffectSpawnerControls.uxml");
      visualTree.CloneTree(rootElement);

      EffectSpawner spawner = (EffectSpawner)target;

      Button playButton = rootElement.Q<Button>("play");
      playButton.clicked += () =>
      {
        spawner.Spawn();
      };

      return rootElement;

    }
  }
}