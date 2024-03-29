using UnityEngine;


namespace RFG
{
  public class BreakableLight : MonoBehaviour, IBreakable
  {
    private UnityEngine.Rendering.Universal.Light2D light2D;
    private void Awake()
    {
      light2D = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
      if (light2D == null)
      {
        light2D = GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>();
      }
    }
    public void Break(Vector3 point, Vector3 normal)
    {
      if (light2D)
      {
        Destroy(light2D);
      }
    }
  }
}