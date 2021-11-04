using UnityEngine;

namespace RFG.SceneGraph
{
  public class SceneTransition : ScriptableObject
  {
    [HideInInspector] public string guid;
    [HideInInspector] public Vector2 position;
    [HideInInspector] public Scene parent;
    [HideInInspector] public SceneTransition from;
    [HideInInspector] public SceneTransition to;

    [Header("Auto Loading")]
    public float autoLoadWaitTime = 0f;
    public bool anyInput = false;

    [Header("Transition")]
    public float waitTime = 0f;
  }
}