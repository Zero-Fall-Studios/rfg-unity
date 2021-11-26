using UnityEngine;
using RFG;

namespace Game
{
  public class PauseMenu : MonoBehaviour
  {
    [SerializeField] private Animator _animator;

    public void ToggleMenu()
    {
      if (_animator != null)
      {
        if (GameManager.Instance.IsPaused)
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