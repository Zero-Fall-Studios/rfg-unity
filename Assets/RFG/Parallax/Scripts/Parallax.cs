using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Parallax/Parallax")]
  public class Parallax : MonoBehaviour
  {
    [field: SerializeField] private Vector2 ParallaxEffectMultiplier { get; set; }
    [field: SerializeField] private Vector2 AutoScroll { get; set; }

    private Transform _cameraTransform;
    private Vector3 _lastCameraPosition;

    private void Start()
    {
      _cameraTransform = Camera.main.transform;
      _lastCameraPosition = _cameraTransform.position;
    }

    private void LateUpdate()
    {
      Vector3 deltaMovement = _cameraTransform.position - _lastCameraPosition;
      Vector3 amount = new Vector3((deltaMovement.x + AutoScroll.x) * ParallaxEffectMultiplier.x, (deltaMovement.y + AutoScroll.y) * ParallaxEffectMultiplier.y, 0);
      transform.position += amount;
      _lastCameraPosition = _cameraTransform.position;
    }
  }
}