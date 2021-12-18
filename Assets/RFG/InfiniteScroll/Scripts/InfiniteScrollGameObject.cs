using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RFG
{
  public class InfiniteScrollGameObject : MonoBehaviour
  {
    [field: SerializeField] private bool InfiniteHorizontal { get; set; }
    [field: SerializeField] private bool InfiniteVertical { get; set; }
    [field: SerializeField] private float Choke { get; set; } = 16f;
    [field: SerializeField] private float Width { get; set; }
    [field: SerializeField] private float Height { get; set; }
    [field: SerializeField] private List<GameObject> GameObjects { get; set; }

    private Transform _cameraTransform;
    private Vector3 _lastCameraPosition;
    private float _minX;
    private float _maxX;
    private float _minY;
    private float _maxY;
    private Vector2 _screenBounds;
    private List<GameObject> _horizontalGameObjects;
    private List<GameObject> _verticalGameObjects;
    private GameObject _firstHorizontal;
    private GameObject _middleHorizontal;
    private GameObject _lastHorizontal;
    private GameObject _firstVertical;
    private GameObject _middleVertical;
    private GameObject _lastVertical;

    private void Awake()
    {
      Camera mainCamera = Camera.main;
      _cameraTransform = mainCamera.transform;
      _lastCameraPosition = _cameraTransform.position;

      _screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));

      CalculateCoords();
    }

    private void LateUpdate()
    {
      Vector3 deltaMovement = _cameraTransform.position - _lastCameraPosition;

      if (deltaMovement.Equals(Vector3.zero))
        return;

      if (InfiniteHorizontal)
      {
        if (deltaMovement.x > 0 && _cameraTransform.position.x + _screenBounds.x + Choke > _maxX)
        {
          MoveHorizontal(_firstHorizontal, _maxX + Width);
        }
        else if (deltaMovement.x < 0 && _cameraTransform.position.x - _screenBounds.x - Choke < _minX)
        {
          MoveHorizontal(_lastHorizontal, _minX - (Width * 2));
        }
      }

      if (InfiniteVertical)
      {
        if (deltaMovement.y > 0 && _cameraTransform.position.y + _screenBounds.y + Choke > _maxY)
        {
          MoveVertical(_firstVertical, _maxY + Height);
        }
        else if (deltaMovement.y < 0 && _cameraTransform.position.y - _screenBounds.y - Choke < _minY)
        {
          MoveVertical(_lastVertical, _minY - (Height * 2));
        }
      }

      _lastCameraPosition = _cameraTransform.position;
    }

    private void MoveHorizontal(GameObject ps, float x)
    {
      ps.gameObject.transform.position = new Vector3(x, ps.gameObject.transform.position.y, ps.gameObject.transform.position.z);
      CalculateCoords();
    }

    private void MoveVertical(GameObject ps, float y)
    {
      ps.gameObject.transform.position = new Vector3(ps.gameObject.transform.position.x, y, ps.gameObject.transform.position.z);
      CalculateCoords();
    }

    private void CalculateCoords()
    {
      _horizontalGameObjects = GameObjects.OrderBy(o => o.transform.position.x).ToList();
      _verticalGameObjects = GameObjects.OrderBy(o => o.transform.position.y).ToList();

      _firstHorizontal = _horizontalGameObjects[0];
      _middleHorizontal = _horizontalGameObjects[1];
      _lastHorizontal = _horizontalGameObjects[2];
      _firstVertical = _verticalGameObjects[0];
      _middleVertical = _verticalGameObjects[1];
      _lastVertical = _verticalGameObjects[2];

      _minX = _middleHorizontal.transform.position.x;
      _maxX = _middleHorizontal.transform.position.x + Width;
      _minY = _middleVertical.transform.position.y;
      _maxY = _middleVertical.transform.position.y + Height;
    }
  }
}
