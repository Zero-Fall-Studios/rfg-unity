using UnityEngine;
using System;

namespace RFG
{
  public static class Epoch
  {

    public static long Current()
    {
      DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
      long currentEpochTime = (long)(DateTime.UtcNow - epochStart).TotalSeconds;

      return currentEpochTime;
    }

    public static long SecondsElapsed(long t1)
    {
      long difference = Current() - t1;

      return (long)Mathf.Abs(difference);
    }

    public static long SecondsElapsed(long t1, long t2)
    {
      long difference = t1 - t2;

      return (long)Mathf.Abs(difference);
    }

    public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
      System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
      dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp);
      return dtDateTime;
    }

  }
}