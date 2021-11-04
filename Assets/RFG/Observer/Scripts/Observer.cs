using System;
using UnityEngine;

namespace RFG
{
  public class Observer<T> : ScriptableObject
  {
    public event Action<T> OnRaise;

    public void Raise(T param)
    {
      OnRaise?.Invoke(param);
    }
  }
}