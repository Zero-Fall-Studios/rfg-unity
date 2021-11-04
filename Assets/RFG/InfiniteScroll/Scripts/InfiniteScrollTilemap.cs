using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RFG
{
  [AddComponentMenu("RFG/Infinite Scroll/Infinite Scroll Tilemap")]
  public class InfiniteScrollTilemap : MonoBehaviour
  {
    [field: SerializeField] private bool InfiniteHorizontal { get; set; }
    [field: SerializeField] private bool InfiniteVertical { get; set; }
    [field: SerializeField] private float Choke { get; set; } = 16f;

    private Transform _cameraTransform;
    private Vector3 _lastCameraPosition;
    private Tilemap _tilemap;
    private List<TileData> _tiles;
    private List<TileData> _maxHorizontalTiles;
    private List<TileData> _minHorizontalTiles;
    private List<TileData> _maxVerticalTiles;
    private List<TileData> _minVerticalTiles;
    private float _minX;
    private float _maxX;
    private float _minY;
    private float _maxY;
    private int _minCoordX;
    private int _maxCoordX;
    private int _minCoordY;
    private int _maxCoordY;
    private int _width;
    private int _height;
    private Vector2 _screenBounds;

    private void Awake()
    {
      Camera mainCamera = Camera.main;
      _cameraTransform = mainCamera.transform;
      _lastCameraPosition = _cameraTransform.position;

      _tilemap = GetComponent<Tilemap>();
      _tilemap.CompressBounds();
      _tiles = _tilemap.GetTiles();

      if (_tiles.Count == 0)
      {
        LogExt.Warn<InfiniteScrollTilemap>("Tile count is 0");
        return;
      }

      _screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));

      _maxHorizontalTiles = new List<TileData>();
      _minHorizontalTiles = new List<TileData>();
      _maxVerticalTiles = new List<TileData>();
      _minVerticalTiles = new List<TileData>();

      CalculateSize();

      _width = _maxCoordX - _minCoordX;
      _height = _maxCoordY - _minCoordY;
    }

    private void CalculateSize()
    {
      _maxHorizontalTiles.Clear();
      _minHorizontalTiles.Clear();
      _maxVerticalTiles.Clear();
      _minVerticalTiles.Clear();
      _minX = _tiles[0].worldPoint.x;
      _maxX = _tiles[_tiles.Count - 1].worldPoint.x;
      _minY = _tiles[0].worldPoint.y;
      _maxY = _tiles[_tiles.Count - 1].worldPoint.y;

      foreach (TileData tileData in _tiles)
      {
        if (tileData.worldPoint.x <= _minX)
        {
          _minX = tileData.worldPoint.x;
          _minCoordX = tileData.coordinates.x;
        }
        if (tileData.worldPoint.x >= _maxX)
        {
          _maxX = tileData.worldPoint.x;
          _maxCoordX = tileData.coordinates.x;
        }
        if (tileData.worldPoint.y <= _minY)
        {
          _minY = tileData.worldPoint.y;
          _minCoordY = tileData.coordinates.y;
        }
        if (tileData.worldPoint.y >= _maxY)
        {
          _maxY = tileData.worldPoint.y;
          _maxCoordY = tileData.coordinates.y;
        }
      }

      foreach (TileData tileData in _tiles)
      {
        if (tileData.worldPoint.x == _minX)
        {
          _minHorizontalTiles.Add(tileData);
        }
        if (tileData.worldPoint.x == _maxX)
        {
          _maxHorizontalTiles.Add(tileData);
        }
        if (tileData.worldPoint.y == _minY)
        {
          _minVerticalTiles.Add(tileData);
        }
        if (tileData.worldPoint.y == _maxY)
        {
          _maxVerticalTiles.Add(tileData);
        }
      }
    }

    private void LateUpdate()
    {
      Vector3 deltaMovement = _cameraTransform.position - _lastCameraPosition;

      if (deltaMovement.Equals(Vector3.zero))
        return;


      if (InfiniteHorizontal)
      {
        if (_cameraTransform.position.x + _screenBounds.x + Choke > _maxX)
        {
          MoveHorizontalTiles(_minHorizontalTiles, _width);
        }
        else if (_cameraTransform.position.x - _screenBounds.x - Choke < _minX)
        {
          MoveHorizontalTiles(_maxHorizontalTiles, -_width);
        }
      }

      if (InfiniteVertical)
      {
        if (_cameraTransform.position.y + _screenBounds.y + Choke > _maxY)
        {
          MoveVerticalTiles(_minVerticalTiles, _height);
        }
        else if (_cameraTransform.position.y - _screenBounds.y - Choke < _minY)
        {
          MoveVerticalTiles(_maxVerticalTiles, -_height);
        }
      }

      _lastCameraPosition = _cameraTransform.position;
    }

    private void MoveHorizontalTiles(List<TileData> tiles, int amount)
    {
      foreach (TileData tileData in tiles)
      {
        SetTile(tileData, new Vector3Int(tileData.coordinates.x + amount, tileData.coordinates.y, tileData.coordinates.z));
      }
      CalculateSize();
    }

    private void MoveVerticalTiles(List<TileData> tiles, int amount)
    {
      foreach (TileData tileData in tiles)
      {
        SetTile(tileData, new Vector3Int(tileData.coordinates.x, tileData.coordinates.y + amount, tileData.coordinates.z));
      }
      CalculateSize();
    }

    private void SetTile(TileData tile, Vector3Int newCoordinates)
    {
      _tilemap.SetTile(tile.coordinates, null);
      tile.coordinates = newCoordinates;
      _tilemap.SetTile(tile.coordinates, tile.tile);
      tile.worldPoint = _tilemap.CellToWorld(tile.coordinates);
    }
  }
}