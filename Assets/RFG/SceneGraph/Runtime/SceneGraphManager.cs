using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RFG.SceneGraph
{
  using Transitions;
  [AddComponentMenu("RFG/Scene Graph/Scene Graph Manager")]
  public class SceneGraphManager : Singleton<SceneGraphManager>
  {
    [field: SerializeField] public SceneGraph SceneGraph { get; set; }
    [field: SerializeField] public Transition Transition { get; set; }
    [field: SerializeField] private float WaitTime { get; set; } = 0f;

    public Scene CurrentScene
    {
      get
      {
        if (_currentScene == null && SceneGraph != null)
        {
          _currentScenePath = GetCurrentScene().path;
          _currentScene = SceneGraph.FindScene(_currentScenePath);
        }
        return _currentScene;
      }
    }

    public SceneTransition NextSceneTransition
    {
      get
      {
        if (CurrentScene == null)
        {
          return null;
        }
        if (_nextSceneTransition == null)
        {
          SceneTransition sceneTransition = CurrentScene.sceneTransitions.First();
          if (sceneTransition == null)
          {
            LogExt.Warn<SceneGraphManager>($"There is no next Scene transition on this scene");
            return null;
          }
          _nextSceneTransition = sceneTransition;
        }
        return _nextSceneTransition;
      }
    }

    private Scene _currentScene;
    private string _currentScenePath;
    private SceneTransition _nextSceneTransition;
    private bool _hasTransitionCompleted = false;
    private List<SceneDoor> _sceneDoors;

    #region Unity Methods
    protected override void Awake()
    {
      base.Awake();

      if (CurrentScene == null)
      {
        LogExt.Warn<SceneGraphManager>($"Didn't find {_currentScenePath} in the scene graph");
        return;
      }

      if (_currentScene.sceneTransitions.Count == 0)
      {
        LogExt.Warn<SceneGraphManager>($"The {_currentScenePath} scene doesn't have any transitions in the scene graph");
      }

      _sceneDoors = new List<SceneDoor>(FindObjectsOfType<SceneDoor>());
    }

    private void OnEnable()
    {
      if (Transition != null)
      {
        Transition.onComplete.AddListener(OnCompleteTransition);
      }
    }

    private void OnDisable()
    {
      if (Transition != null)
      {
        Transition.onComplete.RemoveListener(OnCompleteTransition);
      }
    }
    #endregion

    #region Utilities
    public string GetLastScene()
    {
      return PlayerPrefs.GetString("lastScene");
    }

    public SceneTransition GetFromSceneTransition()
    {
      return SceneGraph.FindSceneTransitionByGuid(PlayerPrefs.GetString("sceneTransition:from:guid"));
    }

    public SceneTransition GetToSceneTransition()
    {
      return SceneGraph.FindSceneTransitionByGuid(PlayerPrefs.GetString("sceneTransition:to:guid"));
    }

    public SceneDoor FindSceneDoorBySceneTransition(SceneTransition sceneTransition)
    {
      if (sceneTransition == null)
      {
        return null;
      }
      foreach (SceneDoor sceneDoor in _sceneDoors)
      {
        if (sceneDoor.sceneTransition.Equals(sceneTransition))
        {
          return sceneDoor;
        }
      }
      return null;
    }

    public SceneDoor FindFromSceneDoor()
    {
      return FindSceneDoorBySceneTransition(GetFromSceneTransition());
    }

    public SceneDoor FindToSceneDoor()
    {
      return FindSceneDoorBySceneTransition(GetToSceneTransition());
    }
    #endregion

    #region Load Scene
    public UnityEngine.SceneManagement.Scene GetCurrentScene()
    {
      return UnityEngine.SceneManagement.SceneManager.GetActiveScene();
    }

    public void LoadSceneFrom(SceneTransition sceneTransition)
    {
      PlayerPrefs.SetString("sceneTransition:from:guid", sceneTransition.guid);
      PlayerPrefs.SetString("sceneTransition:to:guid", sceneTransition.from.guid);
      StartCoroutine(LoadSceneCo(sceneTransition, sceneTransition.from.parent.scenePath));
    }

    public void LoadSceneTo(SceneTransition sceneTransition)
    {
      PlayerPrefs.SetString("sceneTransition:from:guid", sceneTransition.guid);
      PlayerPrefs.SetString("sceneTransition:to:guid", sceneTransition.to.guid);
      StartCoroutine(LoadSceneCo(sceneTransition, sceneTransition.to.parent.scenePath));
    }

    public void LoadSceneByPath(string scenePath)
    {
      StartCoroutine(LoadSceneCo(null, scenePath));
    }

    public void LoadNextScene()
    {
      PlayerPrefs.SetString("sceneTransition:guid", NextSceneTransition.to.guid);
      StartCoroutine(LoadSceneCo(NextSceneTransition, NextSceneTransition.to.parent.scenePath));
    }

    private IEnumerator LoadSceneCo(SceneTransition sceneTransition, string scenePath)
    {
      _hasTransitionCompleted = false;

      if (Transition != null)
      {
        Transition.Play();
        yield return new WaitUntil(() => _hasTransitionCompleted);
      }

      if (sceneTransition != null)
      {
        yield return new WaitForSecondsRealtime(sceneTransition.waitTime);
      }

      if (WaitTime > 0)
      {
        yield return new WaitForSecondsRealtime(WaitTime);
      }

      PlayerPrefs.SetString("lastScene", GetCurrentScene().path);

      // The Application loads the Scene in the background as the current Scene runs.
      // This is particularly good for creating loading screens.
      AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scenePath);

      // Wait until the asynchronous scene fully loads
      while (!asyncLoad.isDone)
      {
        yield return null;
      }
    }
    #endregion

    #region Events
    private void OnCompleteTransition()
    {
      _hasTransitionCompleted = true;
    }
    #endregion

  }
}