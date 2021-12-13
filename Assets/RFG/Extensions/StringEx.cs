namespace RFG
{
  public static class StringEx
  {
    public static string BeforeLast(this string str, string delimiter = ".")
    {
      return str.Substring(0, str.LastIndexOf(delimiter));
    }

    public static string Last(this string str, string delimiter = ".")
    {
      return str.Substring(str.LastIndexOf(delimiter) + 1);
    }

    public static string RemoveFirst(this string str, string delimiter = ".")
    {
      int index = str.IndexOf(delimiter);
      if (index == -1)
      {
        index = 0;
      }
      return str.Substring(index + 1);
    }

    public static string RemoveLast(this string str, string delimiter = ".")
    {
      int index = str.LastIndexOf(delimiter);
      if (index == -1)
      {
        index = 0;
      }
      return str.Substring(0, index);
    }
  }
}