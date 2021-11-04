using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Effects/Effect Spawner")]
  public class EffectSpawner : MonoBehaviour
  {
    [Tooltip("Spawn effects at these transform locations")]
    public List<Transform> transforms;

    [Tooltip("Wait time before spawning begins")]
    public float waitTime = 0f;

    [Tooltip("String names of effects to spawn")]
    public List<string> effects;

    private Transform _transform;

    private void Awake()
    {
      _transform = transform;
    }

    public void Spawn()
    {
      if (transforms.Count > 0)
      {
        StartCoroutine(SpawnEffectsCo());
      }
    }

    private IEnumerator SpawnEffectsCo()
    {
      for (int i = 0; i < transforms.Count; i++)
      {
        yield return new WaitForSeconds(waitTime);
        Transform currentTransform = transforms[i];
        currentTransform.SpawnFromPool(effects.ToArray());
      }
    }

    public void Stop()
    {
      _transform.DeactivatePoolByTag(effects.ToArray());
    }
  }
}

