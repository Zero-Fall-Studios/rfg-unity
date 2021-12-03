using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace RFG
{
  [CustomEditor(typeof(AnimationSlice))]
  public class AnimationSliceEditor : Editor
  {
    private VisualElement rootElement;
    private Editor editor;

    public void OnEnable()
    {
      rootElement = new VisualElement();

      var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/RFG/Animation/Editor/AnimationSliceEditor/AnimationSliceEditor.uss");
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

      var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RFG/Animation/Editor/AnimationSliceEditor/AnimationSliceEditor.uxml");
      visualTree.CloneTree(rootElement);

      VisualElement mainContainer = rootElement.Q<VisualElement>("container");

      Button sliceButton = new Button()
      {
        text = "Slice"
      };
      sliceButton.clicked += () =>
      {
        Slice();
      };
      mainContainer.Add(sliceButton);

      return rootElement;
    }

    public void Slice()
    {
      AnimationSlice animationSlice = (AnimationSlice)target;
      if (animationSlice.animations.Count == 0)
      {
        LogExt.Warn<AnimationSliceEditor>("This animation slice contains no animations");
        return;
      }
      Texture2D texture = Selection.activeObject as Texture2D;

      if (texture == null)
      {
        LogExt.Warn<AnimationSliceEditor>("Please select a texture2d first before slicing");
        return;
      }

      string path = AssetDatabase.GetAssetPath(texture);
      var importer = AssetImporter.GetAtPath(path) as TextureImporter;

      importer.textureType = TextureImporterType.Sprite;
      importer.spriteImportMode = SpriteImportMode.Multiple;
      importer.mipmapEnabled = false;
      importer.filterMode = FilterMode.Point;
      importer.spritePivot = Vector2.down;
      importer.textureCompression = TextureImporterCompression.Uncompressed;

      var textureSettings = new TextureImporterSettings();
      importer.ReadTextureSettings(textureSettings);
      textureSettings.spriteMeshType = SpriteMeshType.Tight;
      textureSettings.spriteExtrude = 0;

      importer.SetTextureSettings(textureSettings);

      Vector2 offset = Vector2.zero;
      Vector2 padding = Vector2.zero;

      Rect[] rects = InternalSpriteUtility.GenerateGridSpriteRectangles(texture, offset, animationSlice.cellSize, padding, false);

      string filenameNoExtension = Path.GetFileNameWithoutExtension(path);
      var metas = new List<SpriteMetaData>();
      int rectNum = 0;

      foreach (AnimationItem animationItem in animationSlice.animations)
      {
        for (int i = 0; i < animationItem.frames; i++)
        {
          var meta = new SpriteMetaData();
          meta.pivot = Vector2.down;
          meta.alignment = (int)animationSlice.alignment;
          meta.name = rectNum + "_" + filenameNoExtension + "_" + animationItem.name + "_" + i;
          meta.rect = rects[rectNum++];
          metas.Add(meta);
        }
      }

      importer.spritesheet = metas.ToArray();

      AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }

  }
}