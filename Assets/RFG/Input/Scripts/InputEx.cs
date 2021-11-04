using UnityEngine.InputSystem;

namespace RFG
{
  public static class InputEx
  {
    public static bool HasAnyInput()
    {
      bool anyInput = false;
      var keyboard = Keyboard.current;
      if (keyboard != null && keyboard.anyKey.wasPressedThisFrame)
      {
        anyInput = true;
      }
      var mouse = Mouse.current;
      if (mouse != null && mouse.leftButton.wasPressedThisFrame || mouse.rightButton.wasPressedThisFrame)
      {
        anyInput = true;
      }
      // var gamepad = Gamepad.current;
      // if (gamepad != null && gamepad.wasUpdatedThisFrame)
      // {
      //   Debug.Log("Gamepad input");
      //   anyInput = true;
      // }
      var pointer = Pointer.current;
      if (pointer != null && pointer.press.isPressed)
      {
        anyInput = true;
      }
      return anyInput;
    }
  }
}