using System.Collections;
using UnityEngine;

namespace RFG
{
  public static class ColorEx
  {
    public static void SetAlpha(this Color color, float a)
    {
      color = new Color(color.r, color.g, color.b, a);
    }

    public static IEnumerator FadeIn(this Color color, float timeSpeed)
    {
      color = new Color(color.r, color.g, color.b, 0);
      while (color.a < 1.0f)
      {
        color = new Color(color.r, color.g, color.b, color.a + (Time.deltaTime * timeSpeed));
        yield return null;
      }
    }

    public static IEnumerator FadeOut(this Color color, float timeSpeed)
    {
      color = new Color(color.r, color.g, color.b, 1);
      while (color.a > 0.0f)
      {
        color = new Color(color.r, color.g, color.b, color.a - (Time.deltaTime * timeSpeed));
        yield return null;
      }
    }

  }
}