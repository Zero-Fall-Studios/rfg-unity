using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/References/Float Reference Watcher")]
  public class FloatReferenceWatcher : MonoBehaviour
  {
    public enum WatchType { Equals, LessThanOrEqual, GreaterThanOrEqual };

    [Header("Reference")]
    public FloatReference reference;

    [Header("Settings")]
    public float WatchValue;
    public WatchType watchType;

    [Header("Event")]
    public GameEvent GameEvent;

    private void Update()
    {
      switch (watchType)
      {
        case WatchType.Equals:
          if (reference.Value == WatchValue)
          {
            RaiseEvent();
          }
          break;
        case WatchType.LessThanOrEqual:
          if (reference.Value <= WatchValue)
          {
            RaiseEvent();
          }
          break;
        case WatchType.GreaterThanOrEqual:
          if (reference.Value >= WatchValue)
          {
            RaiseEvent();
          }
          break;
      }
    }

    private void RaiseEvent()
    {
      GameEvent.Raise();
    }
  }
}