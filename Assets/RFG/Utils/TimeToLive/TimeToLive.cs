
using System.Collections;
using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Utils/Time To Live/Time To Live")]
  public class TimeToLive : MonoBehaviour
  {
    [Header("Settings")]
    public float ttl = 5f;
    public bool destroyOnDeath = false;

    [Header("Animation Clip")]
    public Animator animator;
    public string animationClip;

    [Header("Animation Event")]
    public bool destroyOnAnimationEvent = false;

    private void Start()
    {
      if (!destroyOnAnimationEvent)
      {
        StartCoroutine(StartCo());
      }
    }

    private IEnumerator StartCo()
    {
      if (animationClip != null)
      {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
          if (clip.name.Equals(animationClip))
          {
            yield return new WaitForSeconds(clip.length);
          }
        }
      }
      else
      {
        yield return new WaitForSeconds(ttl);
      }
      if (destroyOnDeath)
      {
        gameObject.SetActive(false);
        StartCoroutine(DestroyCo());
      }
      else
      {
        gameObject.SetActive(false);
      }
    }

    private IEnumerator DestroyCo()
    {
      yield return new WaitForEndOfFrame();
      Destroy(gameObject);
    }

    public void DestroyOnAnimationEvent()
    {
      if (destroyOnDeath)
      {
        Destroy(gameObject);
      }
      else
      {
        gameObject.SetActive(false);
      }
    }

  }
}