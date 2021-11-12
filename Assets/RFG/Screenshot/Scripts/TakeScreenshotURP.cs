using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace RFG
{
  public class TakeScreenshotURP : MonoBehaviour
  {
    private bool _takeScreenshot = false;

    private void OnEnable()
    {
      RenderPipelineManager.endCameraRendering += endCameraRendering;
    }

    private void OnDisable()
    {
      RenderPipelineManager.endCameraRendering -= endCameraRendering;
    }

    private void endCameraRendering(ScriptableRenderContext arg1, Camera arg2)
    {
      if (!_takeScreenshot)
      {
        return;
      }
      _takeScreenshot = false;
      int width = Screen.width;
      int height = Screen.height;
      Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
      Rect rect = new Rect(0, 0, width, height);
      texture.ReadPixels(rect, 0, 0);
      texture.Apply();

      byte[] byteArray = texture.EncodeToPNG();
      System.IO.File.WriteAllBytes(Application.dataPath + "/GameScreenshot.png", byteArray);
    }

    public void Take()
    {
      // ScreenCapture.CaptureScreenshot("GameScreenshot.png");
      // StartCoroutine(TakeCo());
      _takeScreenshot = true;
    }

    private IEnumerator TakeCo()
    {
      yield return new WaitForEndOfFrame();

      int width = Screen.width;
      int height = Screen.height;
      Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
      Rect rect = new Rect(0, 0, width, height);
      texture.ReadPixels(rect, 0, 0);
      texture.Apply();

      byte[] byteArray = texture.EncodeToPNG();
      System.IO.File.WriteAllBytes(Application.dataPath + "/GameScreenshot.png", byteArray);
    }

  }
}