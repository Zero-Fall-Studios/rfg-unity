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
  }
}