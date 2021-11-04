using UnityEngine;
using UnityEngine.Tilemaps;

namespace RFG
{
  public class BreakableTile : MonoBehaviour, IBreakable
  {
    private Tilemap tilemap;
    private void Awake()
    {
      tilemap = GetComponent<Tilemap>();
    }

    public void Break(RaycastHit2D hit)
    {
      Vector3 hitPosition = Vector3.zero;
      hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
      hitPosition.y = hit.point.y - 0.01f * hit.normal.y;
      var tilePos = tilemap.WorldToCell(hitPosition);
      tilemap.SetTile(tilePos, null);
    }
  }
}