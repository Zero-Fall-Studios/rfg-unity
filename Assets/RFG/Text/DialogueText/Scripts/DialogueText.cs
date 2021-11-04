using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace RFG
{
  public class DialogueText : MonoBehaviour
  {
    [field: SerializeField] private TMP_Text Text { get; set; }
    [field: SerializeField] public float Speed { get; set; } = 0.05f;
    [field: SerializeField] public float FadeInTime { get; set; } = 1f;
    [field: SerializeField] public float FadeOutTime { get; set; } = 1f;
    [field: SerializeField] public bool CanSkip { get; set; } = false;
    [field: SerializeField] private List<string> Effects { get; set; }

    [Header("Events")]
    public UnityEvent onStart;
    public UnityEvent onComplete;

    private IEnumerator _coroutine;
    private Transform _transform;
    private bool _isSaying = false;

    private void Awake()
    {
      _transform = transform;
    }

    private void Update()
    {
      if (_isSaying && CanSkip)
      {
        if (InputEx.HasAnyInput())
        {
          Cancel();
          StartCoroutine(CompleteCo());
        }
      }
    }

    public void SetText(string text)
    {
      Text.SetText(text);
    }

    public void ClearText()
    {
      Text.Clear();
    }

    public void Cancel()
    {
      StopCoroutine(_coroutine);
    }

    public void Say(string dialogue, float waitAfter = 0f, bool canSkip = false)
    {
      CanSkip = canSkip;
      _coroutine = SayCo(dialogue, waitAfter);
      StartCoroutine(_coroutine);
    }

    private IEnumerator SayCo(string dialogue, float waitAfter = 0f)
    {
      ClearText();
      onStart?.Invoke();
      _transform.SpawnFromPool(Effects.ToArray());
      string fullDialog = "";
      if (FadeInTime > 0)
      {
        yield return Text.FadeIn(FadeInTime);
      }
      for (int i = 0; i < dialogue.Length; i++)
      {
        yield return new WaitForSeconds(Speed);
        fullDialog = fullDialog + dialogue[i];
        SetText(fullDialog);
      }
      yield return new WaitForSeconds(waitAfter);
      yield return CompleteCo();
    }

    private IEnumerator CompleteCo()
    {
      if (FadeOutTime > 0)
      {
        yield return Text.FadeOut(FadeOutTime);
      }
      SetText("");
      onComplete?.Invoke();
    }
  }

}
