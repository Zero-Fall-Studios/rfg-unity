using UnityEngine;
using UnityEngine.Events;

namespace RFG
{
  [AddComponentMenu("RFG/Events/Game Event Listener")]
  public class GameEventListener : MonoBehaviour
  {
    public GameEvent Event;
    public UnityEvent Response;

    private void OnEnable()
    {
      Event.RegisterListener(this);
    }

    private void OnDisable()
    {
      Event.UnregisterListener(this);
    }

    public void OnEventRaised()
    {
      Response.Invoke();
    }
  }
}