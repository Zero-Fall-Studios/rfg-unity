using System;
using System.Collections;
using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Interactions/Aggro")]
  public class Aggro : MonoBehaviour
  {
    [Header("Transform Targets")]
    public Transform target1;
    public Transform target2;
    public bool target2IsPlayer;

    [Header("Controls")]
    public float minDistance = 5f;
    public float WarmUpTime = 2f;
    public bool HasAggro { get; private set; }
    public event Action<bool> OnAggroChange;

    [Header("Line Of Sight")]
    public LayerMask layerMask;
    public string[] tags;

    [HideInInspector]
    private float _warmupTimeElapsed = 0f;

    private void Start()
    {
      StartCoroutine(StartCo());
    }

    private IEnumerator StartCo()
    {
      if (target2IsPlayer)
      {
        yield return new WaitUntil(() => GameObject.FindGameObjectWithTag("Player") != null);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
          target2 = player.transform;
        }
      }
    }

    private void LateUpdate()
    {
      _warmupTimeElapsed += Time.deltaTime;
      if (_warmupTimeElapsed >= WarmUpTime)
      {
        _warmupTimeElapsed = 0;
        CheckAggro();
      }
    }

    private void CheckAggro()
    {
      if (target1 != null && target2 != null)
      {
        float distance = Vector2.Distance(target1.position, target2.position);
        bool inRange = distance < minDistance;
        bool hasLineOfSight = false;
        if (!HasAggro && inRange)
        {
          hasLineOfSight = LineOfSight();
          if (hasLineOfSight)
          {
            HasAggro = true;
            OnAggroChange?.Invoke(HasAggro);
          }
        }
        else if (HasAggro)
        {
          hasLineOfSight = LineOfSight();
          if (!hasLineOfSight || !inRange)
          {
            HasAggro = false;
            OnAggroChange?.Invoke(HasAggro);
          }
        }
      }
    }

    private bool LineOfSight()
    {
      Vector2 start = target1.position;
      Vector2 direction = (target2.position - target1.position).normalized;
      float distance = Vector2.Distance(target2.position, target1.position);
      RaycastHit2D hit = RFG.Physics2D.RayCast(start, direction, distance, layerMask, Color.green, true);
      if (hit)
      {
        return CheckTags(hit.transform.gameObject);
      }
      return hit;
    }

    private bool CheckTags(GameObject obj)
    {
      for (int i = 0; i < tags.Length; i++)
      {
        if (obj.CompareTag(tags[i]))
        {
          return true;
        }
      }
      return false;
    }

  }
}