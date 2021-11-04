using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Audio/Audio")]
  public class Audio : MonoBehaviour, IAudio
  {
    [field: SerializeField] public AudioData AudioData { get; set; }
    private AudioSource _audioSource;

    private void Awake()
    {
      _audioSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
      if (AudioData.randomPitch)
      {
        _audioSource.pitch = UnityEngine.Random.Range(AudioData.minPitch, AudioData.maxPitch);
      }
      if (AudioData.fadeTime != 0f)
      {
        StartCoroutine(_audioSource.FadeIn(AudioData.fadeTime));
      }
      else
      {
        _audioSource.Play();
      }
    }

    public void Stop()
    {
      if (AudioData.fadeTime != 0f)
      {
        StartCoroutine(_audioSource.FadeOut(AudioData.fadeTime));
      }
      else
      {
        _audioSource.Stop();
      }
    }

    public void Pause()
    {
      _audioSource.Pause();
    }

    public void Persist(bool persist)
    {
      if (persist)
      {
        DontDestroyOnLoad(gameObject);
      }
      else
      {
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(gameObject, UnityEngine.SceneManagement.SceneManager.GetActiveScene());
      }
    }

    public void GenerateAudioSource()
    {
      AudioData.GenerateAudioSource(gameObject);
    }
  }
}