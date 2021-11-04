using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using MyBox;

namespace RFG
{
  [AddComponentMenu("RFG/Tilemap/Place Tiles Timed")]
  public class PlaceTilesTimed : MonoBehaviour
  {
    [Header("Settings")]
    public Tilemap tilemap;
    public float speed;
    public bool IsPlacing { get; set; }

    [Header("Tile Info")]
    public TileData[] tileData;

    [Header("Effects")]
    public Vector3 effectsPostionOffset = Vector3.zero;
    public string[] placeEffects;
    public string[] removeEffects;

    [HideInInspector]
    private Transform _transform;
    private float _timeElapsed = 0f;
    private int _tileIndex = 0;

    private void Awake()
    {
      _transform = transform;
    }

    private void Update()
    {
      if (IsPlacing)
      {
        _timeElapsed += Time.deltaTime;
        if (_timeElapsed > speed)
        {
          _timeElapsed = 0;
          PlaceTile();
        }
      }
    }

    private void PlaceTile()
    {
      TileData data = tileData[_tileIndex];
      if (_transform != null)
      {
        _transform.position = tilemap.CellToWorld(data.coordinates) + effectsPostionOffset;
        _transform.SpawnFromPool(placeEffects);
      }
      tilemap.SetTile(data.coordinates, data.tile);
      _tileIndex++;
      if (_tileIndex >= tileData.Length)
      {
        IsPlacing = false;
        _tileIndex = 0;
      }
    }

    private void RemoveTile()
    {
      TileData data = tileData[_tileIndex];
      if (_transform != null)
      {
        _transform.position = tilemap.CellToWorld(data.coordinates);
        _transform.SpawnFromPool(removeEffects);
      }
      tilemap.SetTile(data.coordinates, null);
      _tileIndex++;
      if (_tileIndex >= tileData.Length)
      {
        IsPlacing = false;
        _tileIndex = 0;
      }
    }

#if UNITY_EDITOR

    private static Tile GetTileByName(string name)
    {
      return (Tile)Resources.Load(name, typeof(Tile));
    }

    [ButtonMethod]
    public void AddTiles()
    {
      IsPlacing = true;
      while (IsPlacing)
      {
        PlaceTile();
      }
    }

    [ButtonMethod]
    public void RemoveTiles()
    {
      IsPlacing = true;
      while (IsPlacing)
      {
        RemoveTile();
      }
    }

#endif

  }
}