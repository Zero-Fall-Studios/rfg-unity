using UnityEngine;

namespace RFG
{
  [CreateAssetMenu(fileName = "New Game Settings", menuName = "RFG/Game/Game Settings")]
  public class GameSettings : ScriptableObject
  {
    [Header("Runtime Settings")]
    public int targetFrameRate = 300;
    public float waitForSecondsToQuit = 3f;

    [Header("Debug Settings")]
    public bool drawRaycasts = true;

  }
}