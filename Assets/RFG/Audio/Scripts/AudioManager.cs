using System.Collections.Generic;
using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Audio/Audio Manager")]
  public class AudioManager : MonoBehaviour
  {
    public List<AudioMixerSettings> AudioMixerSettings;
    private Dictionary<string, IAudio> _audioList;

    private void Awake()
    {
      LoadAudio();
    }

    private void LoadAudio()
    {
      GameObject[] audioGameObjects = GameObject.FindGameObjectsWithTag("Audio");
      _audioList = new Dictionary<string, IAudio>();
      foreach (GameObject audioGameObject in audioGameObjects)
      {
        IAudio audio = audioGameObject.GetComponent(typeof(IAudio)) as IAudio;
        if (audio != null && !_audioList.ContainsKey(audioGameObject.name))
        {
          _audioList.Add(audioGameObject.name, audio);
        }
      }
    }

    private void Start()
    {
      foreach (AudioMixerSettings settings in AudioMixerSettings)
      {
        settings.Initialize();
      }
    }

    public void GenerateAllAudioSources()
    {
      LoadAudio();
      foreach (KeyValuePair<string, IAudio> kvp in _audioList)
      {
        IAudio audio = kvp.Value;
        audio.GenerateAudioSource();
      }
    }

    public void StartAll()
    {
      foreach (KeyValuePair<string, IAudio> kvp in _audioList)
      {
        IAudio audio = kvp.Value;
        audio.Play();
      }
    }

    public void StopAll()
    {
      foreach (KeyValuePair<string, IAudio> kvp in _audioList)
      {
        IAudio audio = kvp.Value;
        audio.Stop();
      }
    }

    public IAudio GetAudio(string name)
    {
      if (!_audioList.ContainsKey(name))
      {
        return null;
      }
      return _audioList[name];
    }

    public void PlayAudio(string name)
    {
      IAudio audio = GetAudio(name);
      if (audio != null)
      {
        audio.Play();
      }
    }

    public void StopAudio(string name)
    {
      IAudio audio = GetAudio(name);
      if (audio != null)
      {
        audio.Stop();
      }
    }

    public void PersistAllPlaylists(bool persist)
    {
      foreach (KeyValuePair<string, IAudio> kvp in _audioList)
      {
        IAudio audio = kvp.Value;
        if (audio is Playlist playlist)
        {
          playlist.Persist(persist);
        }
      }
    }

  }
}