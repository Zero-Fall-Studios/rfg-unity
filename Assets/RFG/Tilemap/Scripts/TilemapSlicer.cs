using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace RFG
{
  public class TilemapSlicer
  {
    public static void Slice(Vector2 offset, Vector2 size, Vector2 padding, string[] tileNames = null)
    {
      var textures = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);

      foreach (var texture in textures)
      {
        ProcessTexture(texture, offset, size, padding, tileNames);
      }
    }

    static void ProcessTexture(Texture2D texture, Vector2 offset, Vector2 size, Vector2 padding, string[] tileNames = null)
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

      Rect[] rects = InternalSpriteUtility.GenerateGridSpriteRectangles(texture, offset, size, padding, false);
      var rectsList = new List<Rect>(rects);
      rectsList = SortRects(rectsList, texture.width);

      string filenameNoExtension = Path.GetFileNameWithoutExtension(path);
      var metas = new List<SpriteMetaData>();
      int rectNum = 0;

      foreach (Rect rect in rectsList)
      {
        var meta = new SpriteMetaData();
        meta.pivot = Vector2.down;
        meta.alignment = (int)SpriteAlignment.Center;
        meta.rect = rect;
        if (tileNames != null && rectNum < tileNames.Length)
        {
          meta.name = filenameNoExtension + "_" + rectNum + "_" + tileNames[rectNum];
        }
        else
        {
          meta.name = filenameNoExtension + "_" + rectNum;
        }
        rectNum++;
        metas.Add(meta);
      }

      importer.spritesheet = metas.ToArray();

      AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }

    static List<Rect> SortRects(List<Rect> rects, float textureWidth)
    {
      List<Rect> list = new List<Rect>();
      while (rects.Count > 0)
      {
        Rect rect = rects[0];
        Rect sweepRect = new Rect(0f, rect.yMin, textureWidth, rect.height);
        List<Rect> list2 = RectSweep(rects, sweepRect);
        if (list2.Count <= 0)
        {
          list.AddRange(rects);
          break;
        }
        list.AddRange(list2);
      }
      return list;
    }

    static List<Rect> RectSweep(List<Rect> rects, Rect sweepRect)
    {
      List<Rect> result;
      if (rects == null || rects.Count == 0)
      {
        result = new List<Rect>();
      }
      else
      {
        List<Rect> list = new List<Rect>();
        foreach (Rect current in rects)
        {
          if (current.Overlaps(sweepRect))
          {
            list.Add(current);
          }
        }
        foreach (Rect current2 in list)
        {
          rects.Remove(current2);
        }
        list.Sort((a, b) => a.x.CompareTo(b.x));
        result = list;
      }
      return result;
    }
  }
}