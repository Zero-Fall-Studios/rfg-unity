using UnityEngine;
using RFG;

namespace Game
{
  public class PauseMenu : MonoBehaviour
  {
    [SerializeField] private Animator _animator;
    private bool _show = false;
    private Character _character;
    private PauseAbility _pauseAbility;

    private void Awake()
    {
      GameObject player = GameObject.FindGameObjectWithTag("Player");
      _character = player.GetComponent<Character>();
      _pauseAbility = player.GetComponent<PauseAbility>();
    }

    public void EnablePauseAbility()
    {
      _pauseAbility.enabled = true;
      _character.EnableAllInput(true);
    }

    public void ToggleMenu()
    {
      if (_animator != null)
      {
        _show = !_show;
        _pauseAbility.enabled = false;
        if (_show)
        {
          _animator.Play("SlideLeft");
        }
        else
        {
          _animator.Play("SlideRight");
        }
      }
    }

  }
}