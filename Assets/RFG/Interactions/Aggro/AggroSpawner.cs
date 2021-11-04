using UnityEngine;
using MyBox;

namespace RFG
{
  [AddComponentMenu("RFG/Interactions/Aggro Spawner")]
  public class AggroSpawner : MonoBehaviour
  {
    [Header("Object Pool")]
    public string Category = "";
    public string Tag = "";

    [Header("Controls")]
    public float SpawnSpeed = 0f;
    public int SpawnLimit = 10;
    public bool Separately = false;
    public bool SpawnFirstOnAggro = false;

    [HideInInspector]
    private Aggro _aggro;
    private float _spawnTimeElapsed = 0f;
    private int _spawnCount = 0;
    private bool _canSpawn = false;
    private GameObject _currentInstance = null;

    private void Awake()
    {
      _aggro = GetComponent<Aggro>();
    }

    private void Update()
    {
      if (_spawnCount >= SpawnLimit)
      {
        return;
      }

      if (_aggro.HasAggro && ((Separately && (_currentInstance == null || _currentInstance.gameObject == null || !_currentInstance.gameObject.activeSelf)) || !Separately))
      {
        if (!_canSpawn)
        {
          _spawnTimeElapsed += Time.deltaTime;
          if (_spawnTimeElapsed >= SpawnSpeed)
          {
            _spawnTimeElapsed = 0;
            _canSpawn = true;
          }
          else if (SpawnFirstOnAggro)
          {
            _canSpawn = true;
            SpawnFirstOnAggro = false;
          }
        }
        else
        {
          _canSpawn = false;
          Spawn();
        }
      }
    }

    private void Spawn()
    {
      if (!Tag.Equals(""))
      {
        _currentInstance = ObjectPool.Instance.SpawnFromPool(Tag, transform.position, Quaternion.identity);
      }
      _spawnCount++;
    }

#if UNITY_EDITOR
    [ButtonMethod]
    private void AddAggro()
    {
      Aggro aggro = gameObject.AddComponent<Aggro>();
      aggro.target1 = transform;
    }
#endif
  }
}