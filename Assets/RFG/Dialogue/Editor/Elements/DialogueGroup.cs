using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace RFG.Dialogue
{
  public class DialogueGroup : Group
  {
    public string ID { get; set; }
    public string oldTitle;
    private Color defaultBorderColor;
    private float defaultBorderWidth;

    public DialogueGroup(string groupTitle, Vector2 position)
    {
      ID = Guid.NewGuid().ToString();
      title = groupTitle;
      oldTitle = groupTitle;
      SetPosition(new Rect(position, Vector2.zero));
      defaultBorderColor = contentContainer.style.borderBottomColor.value;
      defaultBorderWidth = contentContainer.style.borderBottomWidth.value;
    }

    public void SetErrorStyle(Color color)
    {
      contentContainer.style.borderBottomColor = color;
      contentContainer.style.borderBottomWidth = 2f;
    }

    public void ResetStyle()
    {
      contentContainer.style.borderBottomColor = defaultBorderColor;
      contentContainer.style.borderBottomWidth = defaultBorderWidth;
    }
  }
}