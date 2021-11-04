using UnityEngine;
using UnityEngine.Events;

namespace RFG.Transitions
{
  public enum TweenLoopType { Clamp, Once, PingPong };

  [AddComponentMenu("RFG/Transitions/Transition")]
  public class Transition : MonoBehaviour
  {

    [Header("Material Settings")]
    [field: SerializeField] private string materialProperty;
    [field: SerializeField] private float startingValue = 0f;

    [Header("Tween Settings")]
    [field: SerializeField] private TweenLoopType tweenLoopType = TweenLoopType.Once;
    [field: SerializeField] private LeanTweenType easeType = LeanTweenType.linear;
    [field: SerializeField] private float from = 0f;
    [field: SerializeField] private float to = 1f;
    [field: SerializeField] private float time = 1f;
    [field: SerializeField] private float delay = 1f;
    [field: SerializeField] private int loops = 1;
    [field: SerializeField] private bool playOnAwake = false;
    [field: SerializeField] private bool ignoreTimescale = false;

    [Header("Events")]
    public UnityEvent onPlay;
    public UnityEvent onComplete;
    public UnityEvent onPause;
    public UnityEvent onResume;

    public bool IsPlaying { get; private set; }
    public bool IsCompleted { get; private set; }

    private SpriteRenderer _sr;
    private LTDescr _desc;
    private int _id;
    private bool _isPaused = false;

    private void Awake()
    {
      _sr = GetComponent<SpriteRenderer>();
      OnUpdate(startingValue);
      if (playOnAwake)
      {
        Play();
      }
    }

    public void Play(bool reverse = false)
    {
      LeanTween.cancel(gameObject);

      IsCompleted = false;
      IsPlaying = true;
      _isPaused = false;

      if (reverse)
      {
        _desc = LeanTween.value(gameObject, to, from, time);
      }
      else
      {
        _desc = LeanTween.value(gameObject, from, to, time);
      }

      if (ignoreTimescale)
      {
        _desc.setIgnoreTimeScale(ignoreTimescale);
      }

      _desc.setEase(easeType)
      .setDelay(delay)
      .setOnUpdate(OnUpdate)
      .setOnComplete(OnComplete);

      _id = _desc.id;

      SetLoopType(tweenLoopType);

      onPlay?.Invoke();
    }

    public void SetLoopType(TweenLoopType loopType)
    {
      switch (loopType)
      {
        case TweenLoopType.Clamp:
          _desc.setLoopClamp(loops);
          break;
        case TweenLoopType.Once:
          _desc.setLoopOnce();
          break;
        case TweenLoopType.PingPong:
          _desc.setLoopPingPong(loops);
          break;
        default:
          _desc.setLoopOnce();
          break;
      }
    }

    public void TogglePause()
    {
      LTDescr d = LeanTween.descr(_id);
      if (d == null)
      {
        return;
      }
      if (_isPaused)
      {
        _isPaused = false;
        d.resume();
        onResume?.Invoke();
      }
      else
      {
        _isPaused = true;
        d.pause();
        onPause?.Invoke();
      }
    }

    public void Cancel()
    {
      LeanTween.cancel(gameObject);
    }

    private void OnUpdate(float val)
    {
      _sr.material.SetFloat(materialProperty, val);
    }

    private void OnComplete()
    {
      IsCompleted = true;
      IsPlaying = false;
      onComplete?.Invoke();
    }
  }
}