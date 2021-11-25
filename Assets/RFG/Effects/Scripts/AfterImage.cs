using System.Collections;
using UnityEngine;

namespace RFG
{
  public class AfterImage : MonoBehaviour
  {
    [SerializeField] private float _alphaSet = 0.8f;
    [SerializeField] private float _fadeTime = 1f;
    [SerializeField] private bool _findPlayerTransform = false;

    private Transform _followTransform;
    private SpriteRenderer _sr;
    private SpriteRenderer _followSr;
    private Color _color;

    private void Awake()
    {
      _sr = GetComponent<SpriteRenderer>();

      if (_findPlayerTransform)
      {
        _followTransform = GameObject.FindGameObjectWithTag("Player").transform;
      }
      _followSr = _followTransform.GetComponent<SpriteRenderer>();
    }

    public void OnEnable()
    {
      _sr.sprite = _followSr.sprite;
      transform.position = _followTransform.position;
      transform.rotation = _followTransform.rotation;
      _color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, _alphaSet);
      _sr.color = _color;
      StartCoroutine(FadeOut());
    }

    public IEnumerator FadeOut()
    {
      while (_sr.color.a > 0.0f)
      {
        _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, _sr.color.a - (Time.deltaTime * _fadeTime));
        yield return null;
      }
    }
  }
}