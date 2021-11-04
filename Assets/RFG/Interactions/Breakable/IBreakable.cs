using UnityEngine;

namespace RFG
{
  interface IBreakable
  {
    void Break(RaycastHit2D hit);
  }
}