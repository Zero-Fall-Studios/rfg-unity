using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RFG
{
  [AddComponentMenu("RFG/Audio/Audio Trigger")]
  public class AudioTrigger : MonoBehaviour
  {
    public bool OnlyOnce = false;
    public List<string> Tags;
    public UnityEvent OnTriggerEnter;
    public UnityEvent OnTriggerExit;

    private bool _triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (HasTag(other) && !_triggered)
      {
        _triggered = true;
        OnTriggerEnter?.Invoke();
      }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
      if (HasTag(other) && _triggered && !OnlyOnce)
      {
        _triggered = false;
        OnTriggerExit?.Invoke();
      }
    }

    private bool HasTag(Collider2D other)
    {
      foreach (string tag in Tags)
      {
        if (other.gameObject.CompareTag(tag))
        {
          return true;
        }
      }
      return false;
    }
  }
}