using UnityEngine;
using Cinemachine;

namespace RFG
{
  [AddComponentMenu("RFG/Effects/Cinemachine Shake")]
  public class CinemachineShake : Singleton<CinemachineShake>
  {
    private CinemachineVirtualCamera _camera;
    private CinemachineBasicMultiChannelPerlin _perlin;
    private float _shakeTimer;
    private float _shakeTimerTotal;
    private float _startingIntensity;
    private bool _fade = false;

    protected override void Awake()
    {
      base.Awake();
      _camera = GetComponent<CinemachineVirtualCamera>();
      _perlin = _camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float intensity, float time, bool fade = false)
    {
      _perlin.m_AmplitudeGain = intensity;
      _shakeTimer = time;
      _shakeTimerTotal = time;
      _startingIntensity = intensity;
      _fade = fade;
    }

    private void Update()
    {
      if (_shakeTimer > 0)
      {
        _shakeTimer -= Time.deltaTime;
        if (_shakeTimer < 0f)
        {
          _perlin.m_AmplitudeGain = _fade ? Mathf.Lerp(_startingIntensity, 0f, (1 - (_shakeTimer / _shakeTimerTotal))) : 0;
        }
      }
    }
  }
}