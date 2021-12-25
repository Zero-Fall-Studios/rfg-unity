using System.Collections;
using UnityEngine;
using MyBox;

namespace RFG
{
  [AddComponentMenu("RFG/Projectiles/Projectile")]
  public class Projectile : MonoBehaviour, IPooledObject
  {
    [field: SerializeField] private float Speed { get; set; } = 5f;
    [field: SerializeField] private float Damage { get; set; } = 10f;
    [field: SerializeField] private string SpawnAtName { get; set; }
    [field: SerializeField] private Vector3 SpawnOffset { get; set; }
    [field: SerializeField] private Transform Target { get; set; }
    [field: SerializeField] private string TargetTag { get; set; }

    [field: SerializeField] private LayerMask LayerMask { get; set; }

    [field: SerializeField] private string[] SpawnEffects { get; set; }
    [field: SerializeField] private string[] KillEffects { get; set; }

    private Rigidbody2D _rb;
    private BoxCollider2D _collider;
    private Animator _animator;

    private void Awake()
    {
      _rb = GetComponent<Rigidbody2D>();
      _collider = GetComponent<BoxCollider2D>();
      _animator = GetComponent<Animator>();
    }

    public void OnObjectSpawn(params object[] objects)
    {
      if (_animator != null)
      {
        _animator.ResetCurrentClip();
      }
      if (!string.IsNullOrEmpty(TargetTag))
      {
        StartCoroutine(WaitForTargetTag());
      }
      else if (Target != null)
      {
        CalculateVelocity(Target.position - transform.position);
      }
      else
      {
        CalculateVelocity(transform.right);
      }
      if (!string.IsNullOrEmpty(SpawnAtName))
      {
        GameObject spawnAtName = GameObject.Find(SpawnAtName);
        if (spawnAtName != null)
        {
          transform.position = spawnAtName.transform.position;
          transform.rotation = spawnAtName.transform.rotation;
        }
      }
      transform.position += SpawnOffset;
      transform.SpawnFromPool(SpawnEffects, Quaternion.identity);
    }

    private IEnumerator WaitForTargetTag()
    {
      yield return new WaitUntil(() => GameObject.FindGameObjectWithTag(TargetTag) != null);
      GameObject targetTag = GameObject.FindGameObjectWithTag(TargetTag);
      if (targetTag != null)
      {
        Target = targetTag.transform;
      }
      CalculateVelocity(Target.position - transform.position);
    }

    private void CalculateVelocity(Vector3 velocity)
    {
      _rb.velocity = velocity.normalized * Speed;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
      if (LayerMask.Contains(col.gameObject.layer))
      {
        HealthBehaviour health = col.gameObject.GetComponent<HealthBehaviour>();
        if (health != null)
        {
          health.TakeDamage(Damage);
        }

        transform.SpawnFromPool(KillEffects, Quaternion.identity);
        gameObject.SetActive(false);
      }
    }

  }
}