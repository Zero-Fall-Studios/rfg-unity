using UnityEngine;
using UnityEngine.UI;

namespace RFG
{
  [AddComponentMenu("RFG/Audio/Audio UI")]
  public class AudioUI : MonoBehaviour
  {
    public Slider SoundTrackSlider;
    public AudioMixerSettings SoundTrackMixer;

    public Slider AmbienceSlider;
    public AudioMixerSettings AmbienceMixer;

    public Slider EffectsSlider;
    public AudioMixerSettings EffectsMixer;

    private void Awake()
    {
      SoundTrackSlider.value = SoundTrackMixer.GetVolume();
      AmbienceSlider.value = AmbienceMixer.GetVolume();
      EffectsSlider.value = EffectsMixer.GetVolume();
    }

    public void ChangeScene(string name)
    {
      UnityEngine.SceneManagement.SceneManager.LoadScene("Audio2");
    }
  }
}