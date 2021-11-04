using System.Collections;
using UnityEngine;
using TMPro;

namespace RFG
{
  public static class TMP_TextEx
  {
    public static void SetAlpha(this TMP_Text text, float a)
    {
      text.color = new Color(text.color.r, text.color.g, text.color.b, a);
    }

    public static void Clear(this TMP_Text text)
    {
      text.SetText("");
    }

    public static IEnumerator FadeIn(this TMP_Text text, float timeSpeed)
    {
      text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
      while (text.color.a < 1.0f)
      {
        text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime * timeSpeed));
        yield return null;
      }
    }

    public static IEnumerator FadeOut(this TMP_Text text, float timeSpeed)
    {
      text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
      while (text.color.a > 0.0f)
      {
        text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime * timeSpeed));
        yield return null;
      }
    }

  }
}