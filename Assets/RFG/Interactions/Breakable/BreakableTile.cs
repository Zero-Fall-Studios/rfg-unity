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

    public void Break(Vector3 point, Vector3 normal)
    {
      Vector3 hitPosition = Vector3.zero;
      hitPosition.x = point.x - 0.01f * normal.x;
      hitPosition.y = point.y - 0.01f * normal.y;
      var tilePos = tilemap.WorldToCell(hitPosition);
      tilemap.SetTile(tilePos, null);
    }
  }
}