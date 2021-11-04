using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace RFG
{
  public class BreakableLight : MonoBehaviour, IBreakable
  {
    private Light2D light2D;
    private void Awake()
    {
      light2D = GetComponent<Light2D>();
      if (light2D == null)
      {
        light2D = GetComponentInChildren<Light2D>();
      }
    }
    public void Break(RaycastHit2D hit)
    {
      if (light2D)
      {
        Destroy(light2D);
      }
    }
  }
}