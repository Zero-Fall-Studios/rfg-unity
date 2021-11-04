using System.Collections;
using UnityEngine;

namespace RFG.SceneGraph
{
  [AddComponentMenu("RFG/Scene Graph/Scene Auto Load")]
  public class SceneAutoLoad : MonoBehaviour
  {
    private Coroutine _autoLoadCo;
    private bool _hasSkipped = false;

    private void Start()
    {
      if (SceneGraphManager.Instance == null)
      {
        LogExt.Warn<SceneAutoLoad>($"There is no next Scene Graph Manager on this scene");
        return;
      }

      if (SceneGraphManager.Instance.CurrentScene == null)
        return;

      if (SceneGraphManager.Instance.CurrentScene.sceneTransitions.Count == 0)
        return;

      if (SceneGraphManager.Instance.NextSceneTransition == null)
      {
        LogExt.Warn<SceneAutoLoad>($"There is no next scene transistion for this scene");
        return;
      }

      if (SceneGraphManager.Instance.NextSceneTransition.autoLoadWaitTime > 0)
      {
        _autoLoadCo = StartCoroutine(AutoLoad());
      }
    }

    private void LateUpdate()
    {
      if (SceneGraphManager.Instance.NextSceneTransition == null || !SceneGraphManager.Instance.NextSceneTransition.anyInput || _hasSkipped)
        return;

      bool anyInput = InputEx.HasAnyInput();

      if (anyInput)
      {
        _hasSkipped = true;
        StopCoroutine(_autoLoadCo);
        SceneGraphManager.Instance.LoadNextScene();
      }
    }

    private IEnumerator AutoLoad()
    {
      yield return new WaitForSecondsRealtime(SceneGraphManager.Instance.NextSceneTransition.autoLoadWaitTime);
      SceneGraphManager.Instance.LoadNextScene();
    }
  }
}