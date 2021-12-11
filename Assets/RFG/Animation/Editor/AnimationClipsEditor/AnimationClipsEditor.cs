using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace RFG
{
  public class AnimationClipsEditor : EditorWindow
  {
    private int _pixelsPerUnit = 16;
    private Vector2 _cellSize = new Vector2(16f, 16f);

    [MenuItem("RFG/Animation Clips Window")]
    public static void ShowWindow()
    {
      GetWindow<AnimationClipsEditor>("AnimationClipsEditorWindow");
    }
    public virtual void CreateGUI()
    {
      VisualElement root = rootVisualElement;
      root.CloneRootTree();
      root.LoadRootStyles();

      Label title = root.Q<Label>("title");
      title.text = "Animation Clips";

      VisualElement mainContainer = root.Q<VisualElement>("container");

      mainContainer.Add(CreateManager());
    }

    private VisualElement CreateManager()
    {
      VisualElement manager = VisualElementUtils.CreateButtonContainer("platformer-manager");

      IMGUIContainer container = new IMGUIContainer(() =>
      {
        _pixelsPerUnit = EditorGUILayout.IntField("Pixels Per Unit:", _pixelsPerUnit);
        _cellSize = EditorGUILayout.Vector2Field("Cell Size:", _cellSize);
      });
      manager.Add(container);

      TextField animatorControllerName = new TextField()
      {
        label = "Animator Controller Name"
      };

      manager.Add(animatorControllerName);

      Button generateClipsButton = new Button()
      {
        text = "Generate Clips"
      };
      generateClipsButton.clicked += () =>
      {
        Slice();
        CreateClips(animatorControllerName.value);
      };
      manager.Add(generateClipsButton);

      return manager;
    }

    private void CreateClips(string name)
    {
      Texture2D texture = Selection.activeObject as Texture2D;

      if (texture == null)
      {
        LogExt.Warn<AnimationClipsEditor>("Please select a Texture2D asset");
        return;
      }

      string path = AssetDatabase.GetAssetPath(texture);
      string animationsPath = path.RemoveLast("/") + "/Animations";
      string animationName = texture.name.RemoveFirst("-");

      UnityEditor.Animations.AnimatorController animatorController = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>($"{animationsPath}/{name}.controller");
      if (animatorController == null)
      {
        LogExt.Warn<AnimationClipsEditor>($"Animation Controller {name} not found");
        return;
      }

      Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();

      float timeStep = .1f;
      bool newClip = false;
      AnimationClip animClip = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{animationsPath}/{animationName}.anim");

      if (animClip == null)
      {
        animClip = new AnimationClip();
        animClip.name = animationName;
        newClip = true;
      }

      if (!newClip)
      {
        animClip.ClearCurves();
      }

      List<ObjectReferenceKeyframe> spriteKeyFrames = new List<ObjectReferenceKeyframe>();
      float time = 0;
      for (int i = 0; i < sprites.Length; i++)
      {
        ObjectReferenceKeyframe spriteKeyFrame = new ObjectReferenceKeyframe();
        spriteKeyFrame.time = time;
        spriteKeyFrame.value = sprites[i];
        time += timeStep;
        spriteKeyFrames.Add(spriteKeyFrame);
      }

      EditorCurveBinding spriteBinding = new EditorCurveBinding();
      spriteBinding.type = typeof(SpriteRenderer);
      spriteBinding.path = "";
      spriteBinding.propertyName = "m_Sprite";
      AnimationUtility.SetObjectReferenceCurve(animClip, spriteBinding, spriteKeyFrames.ToArray());

      AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(animClip);
      AnimationUtility.SetAnimationClipSettings(animClip, settings);

      if (newClip)
      {
        AssetDatabase.CreateAsset(animClip, $"{animationsPath}/{animationName}.anim");
        animatorController.AddMotion(animClip);
      }

      EditorUtility.SetDirty(animatorController);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
    }

    public void Slice()
    {
      Texture2D texture = Selection.activeObject as Texture2D;

      if (texture == null)
      {
        LogExt.Warn<AnimationSliceEditor>("Please select a Texture2D asset");
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
      importer.spritePixelsPerUnit = _pixelsPerUnit;

      var textureSettings = new TextureImporterSettings();
      importer.ReadTextureSettings(textureSettings);
      textureSettings.spriteMeshType = SpriteMeshType.Tight;
      textureSettings.spriteExtrude = 0;

      importer.SetTextureSettings(textureSettings);

      Vector2 offset = Vector2.zero;
      Vector2 padding = Vector2.zero;

      Rect[] rects = InternalSpriteUtility.GenerateGridSpriteRectangles(texture, offset, _cellSize, padding, false);
      var rectsList = new List<Rect>(rects);

      string filenameNoExtension = Path.GetFileNameWithoutExtension(path);
      var metaList = new List<SpriteMetaData>();
      int rectNum = 0;

      foreach (Rect rect in rectsList)
      {
        var meta = new SpriteMetaData();
        meta.pivot = Vector2.down;
        meta.alignment = (int)SpriteAlignment.Center;
        meta.rect = rect;
        meta.name = filenameNoExtension + "_" + rectNum;
        rectNum++;
        metaList.Add(meta);
      }

      importer.spritesheet = metaList.ToArray();

      AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }

  }
}