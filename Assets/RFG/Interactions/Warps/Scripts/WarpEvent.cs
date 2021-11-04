
namespace RFG
{
  public struct WarpEvent
  {
    public int warpFromIndex;
    public int warpToIndex;
    public WarpEvent(int from, int to)
    {
      warpFromIndex = from;
      warpToIndex = to;
    }
  }
}