using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace RFG
{
  public static class MouseOverUILayerObject
  {
    public static bool IsPointerOverUIObject()
    {
      PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
      eventDataCurrentPosition.position = Pointer.current.position.ReadValue();
      List<RaycastResult> results = new List<RaycastResult>();
      EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

      for (int i = 0; i < results.Count; i++)
      {
        if (results[i].gameObject.layer == 5) //5 = UI layer
        {
          return true;
        }
      }

      return false;
    }
  }
}