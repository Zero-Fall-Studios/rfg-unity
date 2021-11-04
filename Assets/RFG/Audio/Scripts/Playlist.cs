using System.Collections;
using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Audio/Playlist")]
  public class Playlist : MonoBehaviour, IAudio
  {
    [field: SerializeField] public PlaylistData PlaylistData { get; set; }
    [field: SerializeField] public bool PersistOnAwake { get; set; }
    [field: SerializeField] public bool OnlyOne { get; set; }

    [HideInInspector] public bool dontDestroy = false;

    private AudioSource _audioSource;
    private bool _isPlaying;
    private bool _isPaused;
    private IEnumerator _playingCoroutine;


    private void Awake()
    {
      _audioSource = GetComponent<AudioSource>();
      PlaylistData.Initialize();

      if (PersistOnAwake)
      {
        Persist(PersistOnAwake);
      }

      if (PlaylistData.playOnAwake)
      {
        Play();
      }
    }

    public void Play()
    {
      if (PlaylistData.audioList.Count == 0)
        return;

      _playingCoroutine = PlayCo();
      StartCoroutine(_playingCoroutine);
    }

    private IEnumerator PlayCo()
    {
      _isPlaying = true;
      _isPaused = false;

      yield return PlayCurrentAudio();

      while (_isPlaying)
      {
        if (Application.isFocused && !_audioSource.isPlaying && !_isPaused)
        {
          if (PlaylistData.IsLast() && !PlaylistData.loop)
          {
            _isPlaying = false;
            StopCoroutine(_playingCoroutine);
            continue;
          }

          if (_isPlaying)
          {
            PlaylistData.Next();
            yield return PlayCurrentAudio();
          }
        }
        yield return null;
      }
    }

    private IEnumerator PlayCurrentAudio()
    {
      AudioData audioData = PlaylistData.GetCurrent();
      audioData.GenerateAudioSource(gameObject);
      yield return new WaitForSecondsRealtime(PlaylistData.waitForSeconds);
      yield return _audioSource.FadeIn(PlaylistData.fadeTime);
    }

    public void TogglePause()
    {
      if (_isPaused)
      {
        _isPaused = false;
        _audioSource.UnPause();
      }
      else
      {
        _isPaused = true;
        _audioSource.Pause();
      }
    }

    public void Stop()
    {
      StartCoroutine(StopCo());
    }

    private IEnumerator StopCo()
    {
      if (_playingCoroutine != null)
      {
        StopCoroutine(_playingCoroutine);
      }
      _isPlaying = false;
      yield return _audioSource.FadeOut(PlaylistData.fadeTime);
      yield return null;
    }

    public void Next()
    {
      StartCoroutine(NextCo());
    }

    private IEnumerator NextCo()
    {
      yield return StopCo();
      PlaylistData.Next();
      Play();
    }

    public void Previous()
    {
      StartCoroutine(PreviousCo());
    }

    private IEnumerator PreviousCo()
    {
      yield return StopCo();
      PlaylistData.Previous();
      Play();
    }

    public void Persist(bool persist)
    {
      if (persist)
      {
        if (OnlyOne && !dontDestroy)
        {
          Playlist[] playlists = FindObjectsOfType<Playlist>();
          if (playlists.Length > 1)
          {
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(gameObject, UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            Destroy(gameObject);
            return;
          }
        }
        dontDestroy = true;
        DontDestroyOnLoad(gameObject);
      }
      else
      {
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(gameObject, UnityEngine.SceneManagement.SceneManager.GetActiveScene());
      }
    }

    public void GenerateAudioSource()
    {
      PlaylistData.audioList[0].GenerateAudioSource(gameObject);
    }
  }
}