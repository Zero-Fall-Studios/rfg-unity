using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RFG
{
  [CustomEditor(typeof(Character))]
  public class CharacterEditorWindow : Editor
  {
    public enum AddComponentType { Select, AttackAbility }
    private VisualElement rootElement;
    private Editor editor;
    private AddComponentType addComponentType;

    public void OnEnable()
    {
      rootElement = new VisualElement();

      rootElement.LoadRootStyles();
    }

    public override VisualElement CreateInspectorGUI()
    {
      rootElement.Clear();

      UnityEngine.Object.DestroyImmediate(editor);
      editor = Editor.CreateEditor(this);
      IMGUIContainer container = new IMGUIContainer(() =>
      {
        if (target)
        {
          OnInspectorGUI();
          AddComponentType newAddComponentType = (AddComponentType)EditorGUILayout.EnumPopup("Add Component: ", addComponentType);

          if (!newAddComponentType.Equals(addComponentType))
          {
            addComponentType = newAddComponentType;
            AddNewComponents();
            addComponentType = AddComponentType.Select;
          }
        }
      });
      rootElement.Add(container);

      return rootElement;
    }

    private void AddNewComponents()
    {
      Character character = (Character)target;
      switch (addComponentType)
      {
        case AddComponentType.AttackAbility:
          character.MovementState.StatePack.GenerateAttackAbilityStates();
          break;
      }
    }
  }
}