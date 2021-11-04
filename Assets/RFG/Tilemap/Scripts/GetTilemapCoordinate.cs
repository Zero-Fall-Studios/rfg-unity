#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace RFG
{
  [AddComponentMenu("RFG/Tilemap/Get Tilemap Coordinate")]
  [ExecuteInEditMode]
  public class GetTilemapCoordinate : MonoBehaviour
  {
    public Grid grid;

    public GetTilemapCoordinate()
    {
      SceneView.duringSceneGui += GetMousePosition;
    }

    public void GetMousePosition(SceneView scene)
    {
      Event e = Event.current;
      if (e != null)
      {
        if (Event.current.type == EventType.MouseDown)
        {

          Vector3Int position = Vector3Int.FloorToInt(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin);
          Vector3Int cellPos = grid.WorldToCell(position);

          Debug.Log(cellPos);
        }
      }
    }
  }
}
#endif