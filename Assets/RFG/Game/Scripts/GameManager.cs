using System.Collections;
using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Game/Game Manager")]
  public class GameManager : Singleton<GameManager>
  {
    public GameSettings GameSettings;
    public bool IsPaused { get; set; }

    private void Start()
    {
      Application.targetFrameRate = GameSettings.targetFrameRate;
      // Application.logMessageReceived += HandleException;
      IsPaused = false;
    }

    public void Pause()
    {
      IsPaused = true;
      Time.timeScale = 0f;
    }

    public void UnPause()
    {
      IsPaused = false;
      Time.timeScale = 1f;
    }

    public void TogglePause()
    {
      if (IsPaused)
      {
        UnPause();
      }
      else
      {
        Pause();
      }
    }

    public void Quit()
    {
      StartCoroutine(StartQuitProcess());
    }

    private IEnumerator StartQuitProcess()
    {
      yield return new WaitForSecondsRealtime(GameSettings.waitForSecondsToQuit);
      Application.Quit();
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // private void HandleException(string logString, string stackTrace, LogType type)
    // {
    //   if (type == LogType.Exception)
    //   {
    //     Debug.Log(logString);
    //   }
    // }

  }
}