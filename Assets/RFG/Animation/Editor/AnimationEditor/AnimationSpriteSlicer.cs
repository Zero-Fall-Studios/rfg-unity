using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace RFG
{
  public class AnimationSpriteSlicer
  {
    public static void Slice(AnimationMap animationMap)
    {
      if (animationMap.animations.Count == 0)
      {
        LogExt.Warn<AnimationSpriteSlicer>("This animation map contains no animations");
        return;
      }
      Texture2D texture = Selection.activeObject as Texture2D;
      ProcessTexture(texture, animationMap);
    }

    static void ProcessTexture(Texture2D texture, AnimationMap animationMap)
    {
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

      Rect[] rects = InternalSpriteUtility.GenerateGridSpriteRectangles(texture, offset, animationMap.cellSize, padding, false);

      string filenameNoExtension = Path.GetFileNameWithoutExtension(path);
      var metas = new List<SpriteMetaData>();
      int rectNum = 0;

      foreach (AnimationItem animationItem in animationMap.animations)
      {
        for (int i = 0; i < animationItem.frames; i++)
        {
          var meta = new SpriteMetaData();
          meta.pivot = Vector2.down;
          meta.alignment = (int)animationMap.alignment;
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