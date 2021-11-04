using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace RFG
{
  [Serializable]
  public struct Tile
  {
    public string tileName;
  }

  public class TilemapSlicerEditorWindow : EditorWindow
  {
    private Vector2 offset = new Vector2(16, 16);
    private Vector2 size = new Vector2(16, 16);
    private Vector2 padding = new Vector2(16, 16);
    private List<Tile> tiles = new List<Tile>();
    private bool isExpanded = false;
    private Vector2 scrollPos;

    [MenuItem("RFG/Tilemap Slicer Editor Window")]
    public static void ShowWindow()
    {
      GetWindow<TilemapSlicerEditorWindow>("TilemapSlicerEditorWindow");
    }

    public virtual void CreateGUI()
    {
      VisualElement root = rootVisualElement;

      VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RFG/Tilemap/Editor/TilemapSlicerEditor/TilemapSlicerEditorWindow.uxml");
      visualTree.CloneTree(root);

      StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/RFG/Tilemap/Editor/TilemapSlicerEditor/TilemapSlicerEditorWindow.uss");
      root.styleSheets.Add(styleSheet);

      VisualElement mainContainer = root.Q<VisualElement>("container");

      mainContainer.Add(CreateManager());
    }

    private VisualElement CreateManager()
    {
      VisualElement manager = CreateContainer("manager");

      IMGUIContainer guiContainer = new IMGUIContainer(() =>
      {
        // Create a gigantic working area. Why I cannot simply use the Screen rect ???
        Rect workArea = GUILayoutUtility.GetRect(0, 10000, 00, 10000);
        // Create scrollable area using GUI not GUILayout.BeginScrollView(). Why ???
        scrollPos = GUI.BeginScrollView(workArea, scrollPos, new Rect(0, 0, 500, 5000));
        // The areas are ready and scrollable - place controls now
        GUILayout.BeginArea(new Rect(0, 0, 500, 5000));

        offset = EditorGUILayout.Vector2Field("Offset", offset);
        size = EditorGUILayout.Vector2Field("Size", size);
        padding = EditorGUILayout.Vector2Field("Padding", padding);

        if (tiles.Count == 0)
        {
          FillTileList();
        }

        var list = new ReorderableList(tiles, typeof(Tile), true, true, true, true);
        list.drawElementCallback =
        (Rect rect, int index, bool isActive, bool isFocused) =>
        {
          Tile element = (Tile)list.list[index];
          rect.y += 2;
          EditorGUI.LabelField(new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight), "Tile Name");
          element.tileName = EditorGUI.TextField(
              new Rect(rect.x + 100, rect.y, 100, EditorGUIUtility.singleLineHeight),
                element.tileName);
          list.list[index] = element;
        };
        list.drawHeaderCallback = (Rect rect) =>
        {
          EditorGUI.LabelField(rect, "Tiles");
          var newRect = new Rect(rect.x + 100, rect.y, rect.width - 10, rect.height);
          isExpanded = EditorGUI.Foldout(newRect, isExpanded, "Expand");
        };
        list.elementHeightCallback = (int indexer) =>
        {
          if (!isExpanded)
            return 0;
          else
            return list.elementHeight;
        };
        list.DoLayoutList();

        GUILayout.EndArea();
        GUI.EndScrollView();
      });
      manager.Add(guiContainer);

      VisualElement buttons = manager.Q<VisualElement>("manager-buttons");

      Button generateTilemapButton = new Button(() =>
      {
        List<string> tileNames = new List<string>();
        foreach (Tile tile in tiles)
        {
          tileNames.Add(tile.tileName);
        }
        TilemapSlicer.Slice(offset, size, padding, tileNames.ToArray());
      })
      {
        name = "tilemap-button",
        text = "Generate Tilemap"
      };
      Button resetTilemapListButton = new Button(() =>
      {
        FillTileList();
      })
      {
        name = "tilemap-reset-list-button",
        text = "Reset Tilemap List"
      };

      buttons.Add(generateTilemapButton);
      buttons.Add(resetTilemapListButton);

      return manager;
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

    private void FillTileList()
    {
      tiles = new List<Tile>()
      {
        new Tile() { tileName = "CenterCenter" }, // 1
        new Tile() { tileName = "CenterTop" }, // 2
        new Tile() { tileName = "LeftTop" }, // 3
        new Tile() { tileName = "RightTop" }, // 4
        new Tile() { tileName = "LeftCenter" }, // 5  
        new Tile() { tileName = "RightCenter" }, // 6 
        new Tile() { tileName = "CenterBottom" }, // 7
        new Tile() { tileName = "LeftBottom" }, // 8
        new Tile() { tileName = "RightBottom" }, // 9 
        new Tile() { tileName = "InnerCenterTop" }, // 10 
        new Tile() { tileName = "InnerLeftTop" }, // 11 
        new Tile() { tileName = "InnerRightTop" }, // 12 
        new Tile() { tileName = "InnerLeftSideTop" }, // 13 
        new Tile() { tileName = "InnerRightSideTop" }, // 14 
        new Tile() { tileName = "InnerLeftSideCenter" }, // 15 
        new Tile() { tileName = "InnerRightSideCenter" }, // 16 
        new Tile() { tileName = "InnerLeftSideBottom" }, // 17 
        new Tile() { tileName = "InnerRightSideBottom" }, // 18 
        new Tile() { tileName = "InnerLeftBottom" }, // 19 
        new Tile() { tileName = "InnerRightBottom" }, // 20 
        new Tile() { tileName = "InnerCenterBottom" }, // 21 
        new Tile() { tileName = "InnerInnerLeftTop" }, // 22 
        new Tile() { tileName = "InnerInnerRightTop" }, // 23 
        new Tile() { tileName = "InnerInnerLeftBottom" }, // 24 
        new Tile() { tileName = "InnerInnerRightBottom" }, // 25 
        new Tile() { tileName = "Inner" }, // 26 
        new Tile() { tileName = "SlopeTopLeftRight" }, // 27 
        new Tile() { tileName = "SlopeTopLeftLeft" }, // 28 
        new Tile() { tileName = "SlopeTopRightLeft" }, // 29 
        new Tile() { tileName = "SlopeTopRightRight" }, // 30 
        new Tile() { tileName = "SlopeBottomLeftLeft" }, // 31 
        new Tile() { tileName = "SlopeBottomLeftRight" }, // 32 
        new Tile() { tileName = "SlopeBottomRightLeft" }, // 33 
        new Tile() { tileName = "SlopeBottomRightRight" }, // 34 
        new Tile() { tileName = "BackgroundCenter" }, // 35 
        new Tile() { tileName = "BackgroundTopLeft" }, // 36 
        new Tile() { tileName = "BackgroundTopCenter" }, // 37 
        new Tile() { tileName = "BackgroundTopRight" }, // 38 
        new Tile() { tileName = "BackgroundCenterLeft" }, // 39
        new Tile() { tileName = "BackgroundCenterRight" }, // 40 
        new Tile() { tileName = "BackgroundBottomLeft" }, // 41 
        new Tile() { tileName = "BackgroundBottomCenter" }, // 42 
        new Tile() { tileName = "BackgroundBottomRight" }, // 43 
        new Tile() { tileName = "BackgroundInner" }, // 44 
        new Tile() { tileName = "LongCenter" }, // 45 
        new Tile() { tileName = "LongLeft" }, // 46 
        new Tile() { tileName = "LongRight" }, // 47 
        new Tile() { tileName = "TallCenter" }, // 48 
        new Tile() { tileName = "TallTop" }, // 49 
        new Tile() { tileName = "TallBottom" }, // 50 
        new Tile() { tileName = "Block" }, // 51 
      };
    }

  }
}