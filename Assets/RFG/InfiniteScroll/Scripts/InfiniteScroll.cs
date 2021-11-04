using UnityEngine;

/**
 * Notes:
 * 
 * Need to change sprite to be tiled and have at least 3 times the width to see no seams
 */

namespace RFG
{
  [AddComponentMenu("RFG/Infinite Scroll/Infinite Scroll")]
  public class InfiniteScroll : MonoBehaviour
  {
    [field: SerializeField] private bool InfiniteHorizontal { get; set; }
    [field: SerializeField] private bool InfiniteVertical { get; set; }

    private Transform _cameraTransform;
    private float _textureUnitSizeX;
    private float _textureUnitSizeY;

    private void Start()
    {
      _cameraTransform = Camera.main.transform;
      Sprite sprite = GetComponent<SpriteRenderer>().sprite;
      Texture2D texture = sprite.texture;
      _textureUnitSizeX = (texture.width / sprite.pixelsPerUnit) * transform.localScale.x;
      _textureUnitSizeY = (texture.height / sprite.pixelsPerUnit) * transform.localScale.y;
    }

    private void LateUpdate()
    {
      if (InfiniteHorizontal)
      {
        if (Mathf.Abs(_cameraTransform.position.x - transform.position.x) >= _textureUnitSizeX)
        {
          float offsetPositionX = (_cameraTransform.position.x - transform.position.x) % _textureUnitSizeX;
          transform.position = new Vector3(_cameraTransform.position.x + offsetPositionX, transform.position.y);
        }
      }

      if (InfiniteVertical)
      {
        if (Mathf.Abs(_cameraTransform.position.y - transform.position.y) >= _textureUnitSizeY)
        {
          float offsetPositionY = (_cameraTransform.position.y - transform.position.y) % _textureUnitSizeY;
          transform.position = new Vector3(transform.position.x, _cameraTransform.position.y + offsetPositionY);
        }
      }
    }
  }
}