using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace RFG
{
  [AddComponentMenu("RFG/Effects/Effect Trigger")]
  public class EffectTrigger : MonoBehaviour
  {
    [field: SerializeField] private string[] EnterEffects { get; set; }
    [field: SerializeField] private string[] ExitEffects { get; set; }
    [field: SerializeField] private bool OnlyOnce { get; set; } = false;
    [field: SerializeField] private string[] Tags { get; set; }
    [field: SerializeField] private float OnTriggerEnterWaitTime { get; set; } = 0f;
    [field: SerializeField] private float OnTriggerExitWaitTime { get; set; } = 0f;
    [field: SerializeField] private UnityEvent OnTriggerEnter;
    [field: SerializeField] private UnityEvent OnTriggerExit;

    private bool _triggered = false;

    public void Disable()
    {
      gameObject.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
      if (col.gameObject.CompareTags(Tags))
      {
        if (!_triggered)
        {
          col.transform.SpawnFromPool(EnterEffects, col.gameObject);
          _triggered = true;
          StartCoroutine(InvokeEvent(OnTriggerEnter, OnTriggerEnterWaitTime));
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
            col.transform.SpawnFromPool(ExitEffects, col.gameObject);
            StartCoroutine(InvokeEvent(OnTriggerExit, OnTriggerEnterWaitTime));
          }
        }
      }
    }

    private IEnumerator InvokeEvent(UnityEvent unityEvent, float waitTime)
    {
      yield return new WaitForSeconds(waitTime);
      unityEvent?.Invoke();
    }

  }
}