using System.Collections.Generic;
using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Interactions/Warp")]
  [RequireComponent(typeof(BoxCollider2D))]
  public class Warp : MonoBehaviour
  {
    [Header("Settings")]
    public int index = 0;
    public int warpToIndex = 0;

    [Header("Effects")]
    public string[] Effects;

    [HideInInspector]
    private bool JustWarped { get; set; }
    private Dictionary<int, Warp> _warps = new Dictionary<int, Warp>();
    private Transform _transform;

    private void Awake()
    {
      _transform = transform;
      GameObject[] warps = GameObject.FindGameObjectsWithTag("Warp");
      foreach (GameObject warp in warps)
      {
        Warp _warp = warp.GetComponent<Warp>();
        if (_warp)
        {
          int index = _warp.index;
          _warps.Add(index, _warp);
        }
      }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
      if (col.gameObject.CompareTag("Player"))
      {
        if (!JustWarped && index != warpToIndex && warpToIndex >= 0 && warpToIndex < _warps.Count)
        {
          Warp warpTo = _warps[warpToIndex];
          warpTo.JustWarped = true;
          JustWarped = true;
          col.gameObject.transform.position = warpTo.transform.position;
          warpTo.transform.SpawnFromPool(Effects);
        }
      }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
      JustWarped = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
      UnityEditor.Handles.color = Color.yellow;
      UnityEditor.Handles.Label(transform.position, $"Warp {index}");
    }
#endif 

  }
}