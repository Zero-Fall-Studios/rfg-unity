using System;
using UnityEngine;
using MyBox;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RFG
{
  [Serializable]
  public class ItemSave
  {
    public string Guid;
  }

  public abstract class Item : ScriptableObject, IItem
  {
    [Header("Settings")]
    public string Guid;
    public string Description;
    public bool OnlyOne = false;
    public bool IsStackable = false;
    public int StackableLimit = 1;

    [Header("Pick Up")]
    public Sprite PickUpSprite;
    public string PickUpText;
    public string[] PickUpEffects;

    public ItemSave GetSave()
    {
      ItemSave save = new ItemSave();
      save.Guid = Guid;
      return save;
    }

#if UNITY_EDITOR
    [ButtonMethod]
    protected void GenerateGuid()
    {
      if (Guid == null || Guid.Equals(""))
      {
        Guid = System.Guid.NewGuid().ToString();
        EditorUtility.SetDirty(this);
      }
    }
#endif
  }
}