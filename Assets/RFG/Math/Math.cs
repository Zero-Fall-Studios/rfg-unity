using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RFG
{
  public static class Math
  {
    public static float DistanceBetweenPointAndLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
      return Vector3.Magnitude(ProjectPointOnLine(point, lineStart, lineEnd) - point);
    }

    public static Vector3 ProjectPointOnLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
      Vector3 rhs = point - lineStart;
      Vector3 vector = lineEnd - lineStart;
      float magnitude = vector.magnitude;
      Vector3 lhs = vector;
      if (magnitude > 1E-06f)
      {
        lhs = (Vector3)(lhs / magnitude);
      }
      float num2 = Mathf.Clamp(Vector3.Dot(lhs, rhs), 0f, magnitude);
      return (lineStart + ((Vector3)(lhs * num2)));
    }

    public static float RoundToClosest(float value, float[] possibleValues, bool pickSmallestDistance = false)
    {
      if (possibleValues.Length == 0)
      {
        return 0f;
      }

      float closestValue = possibleValues[0];

      foreach (float possibleValue in possibleValues)
      {
        float closestDistance = Mathf.Abs(closestValue - value);
        float possibleDistance = Mathf.Abs(possibleValue - value);

        if (closestDistance > possibleDistance)
        {
          closestValue = possibleValue;
        }
        else if (closestDistance == possibleDistance)
        {
          if ((pickSmallestDistance && closestDistance > possibleDistance) || (!pickSmallestDistance && closestValue < possibleValue))
          {
            closestValue = (value < 0) ? closestValue : possibleValue;
          }
        }
      }

      return closestValue;
    }

    public static Vector2 RotateVector2(Vector2 vector, float angle)
    {
      if (angle == 0)
      {
        return vector;
      }
      float sinus = Mathf.Sin(angle * Mathf.Deg2Rad);
      float cosinus = Mathf.Cos(angle * Mathf.Deg2Rad);

      float oldX = vector.x;
      float oldY = vector.y;

      vector.x = (cosinus * oldX) - (sinus * oldY);
      vector.y = (sinus * oldX) + (cosinus * oldY);
      return vector;
    }
  }
}