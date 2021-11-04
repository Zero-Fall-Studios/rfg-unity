using System.Collections;
using UnityEngine;

namespace RFG
{
  public class DissolveShaderController : MonoBehaviour
  {
    public float speed;
    [ColorUsageAttribute(true, true)]
    public Color inColor;
    [ColorUsageAttribute(true, true)]
    public Color outColor;
    private float fade;

    public IEnumerator FadeIn(Material mat)
    {
      float currentTime = 0;
      float fade = 0;
      float start = fade;
      mat.SetColor("_Color", inColor);
      while (fade < 1f)
      {
        currentTime += Time.deltaTime;
        fade = Mathf.Lerp(start, 1, currentTime * speed);
        mat.SetFloat("_Fade", fade);
        yield return null;
      }
      mat.SetFloat("_Fade", 1);
      yield break;
    }

    public IEnumerator FadeOut(Material mat)
    {
      float currentTime = 0;
      float fade = 1;
      float end = fade;
      mat.SetColor("_Color", outColor);
      while (fade > 0.1f)
      {
        currentTime += Time.deltaTime;
        fade = Mathf.Lerp(fade, 0.1f, currentTime * speed);
        mat.SetFloat("_Fade", fade);
        yield return null;
      }
      mat.SetFloat("_Fade", 0f);
      yield break;
    }
  }
}