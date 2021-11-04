using UnityEngine;
using RFG.SceneGraph;

public class MyCharacter : MonoBehaviour
{
  void Start()
  {
    SceneDoor toSceneDoor = SceneGraphManager.Instance.FindToSceneDoor();
    if (toSceneDoor != null)
    {
      Debug.Log("Scene door found");
    }
    else
    {
      Debug.Log("Scene door not found");
    }
  }
}
