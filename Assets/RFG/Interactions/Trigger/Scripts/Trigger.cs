using UnityEngine;
using UnityEngine.Events;

namespace RFG
{
  [AddComponentMenu("RFG/Interactions/Trigger")]
  public class Trigger : MonoBehaviour
  {
    public bool OnlyOnce = false;
    public string[] Tags;
    public UnityEvent OnTriggerEnter;
    public UnityEvent OnTriggerExit;
    private bool _triggered = false;

    public void OnTriggerEnter2D(Collider2D col)
    {
      if (col.gameObject.CompareTags(Tags))
      {
        if (!_triggered)
        {
          _triggered = true;
          OnTriggerEnter?.Invoke();
        }
      }
    }

    public void OnTriggerExit2D(Collider2D col)
    {
      if (col.gameObject.CompareTags(Tags))
      {
        if (_triggered)
        {
          if (!OnlyOnce)
          {
            _triggered = false;
            OnTriggerExit?.Invoke();
          }
        }
      }
    }

  }
}