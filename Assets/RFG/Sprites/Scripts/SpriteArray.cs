using UnityEngine;
using UnityEngine.UI;

namespace RFG
{
  [AddComponentMenu("RFG/Sprites/Sprite Array")]
  public class SpriteArray : MonoBehaviour
  {
    [Header("Settings")]
    public Sprite[] sprites;
    public int startingIndex = 0;

    [HideInInspector]
    private Image _image;
    private SpriteRenderer _spriteRenderer;
    private int _currentIndex = 0;

    private void Awake()
    {
      _image = GetComponent<Image>();
      _spriteRenderer = GetComponent<SpriteRenderer>();
      if (startingIndex != 0)
      {
        SetImageAtIndex(startingIndex);
      }
    }

    public void SetImageAtIndex(int index)
    {
      _currentIndex = index;
      if (_currentIndex >= 0 && _currentIndex < sprites.Length)
      {
        if (_image != null)
        {
          _image.sprite = sprites[index];
        }
        if (_spriteRenderer != null)
        {
          _spriteRenderer.sprite = sprites[index];
        }
      }
      else
      {
        LogExt.Warn<SpriteArray>($"Index {_currentIndex} does not exist");
      }
    }

    public void Next()
    {
      int newIndex = _currentIndex + 1;
      if (newIndex >= sprites.Length)
      {
        newIndex = 0;
      }
      _currentIndex = newIndex;
      SetImageAtIndex(_currentIndex);
    }

    public void Previous()
    {
      int newIndex = _currentIndex - 1;
      if (newIndex < 0)
      {
        newIndex = sprites.Length - 1;
      }
      _currentIndex = newIndex;
      SetImageAtIndex(_currentIndex);
    }
  }
}