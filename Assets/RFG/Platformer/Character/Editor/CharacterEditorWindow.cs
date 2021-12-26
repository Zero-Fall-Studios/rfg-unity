using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RFG
{
  [CustomEditor(typeof(Character))]
  public class CharacterEditorWindow : Editor
  {
    public enum AddAbilityType { Select, JumpAbility, AttackAbility, DashAbility, LadderClimbingAbility, SlideAbility }
    public enum AddMovementStateType { Select, Movement, Jump, DoubleJump, JumpFlip, Fall, PrimaryAttack, SecondaryAttack }
    private VisualElement rootElement;
    private Editor editor;
    private AddAbilityType addAbilityType;
    private AddMovementStateType addMovementStateType;

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

          var boldtext = new GUIStyle(GUI.skin.label);
          boldtext.fontStyle = FontStyle.Bold;

          EditorGUILayout.Space();

          Character character = (Character)target;

          EditorGUILayout.LabelField("Movement States", boldtext);
          AddMovementStateType newAddMovementStateType = (AddMovementStateType)EditorGUILayout.EnumPopup("Add Movement State: ", addMovementStateType);

          if (!newAddMovementStateType.Equals(addMovementStateType))
          {
            addMovementStateType = newAddMovementStateType;
            AddNewMovementState();
            addMovementStateType = AddMovementStateType.Select;
            EditorUtility.SetDirty(character);
          }

          if (character.CharacterType == CharacterType.Player)
          {
            EditorGUILayout.LabelField("Abilities", boldtext);
            AddAbilityType newAddAbilityType = (AddAbilityType)EditorGUILayout.EnumPopup("Add Ability: ", addAbilityType);

            if (!newAddAbilityType.Equals(addAbilityType))
            {
              addAbilityType = newAddAbilityType;
              AddNewAbility();
              addAbilityType = AddAbilityType.Select;
              EditorUtility.SetDirty(character);
            }
          }
          else if (character.CharacterType == CharacterType.AI)
          {
            if (GUILayout.Button("Add Brain"))
            {
              character.gameObject.GetOrAddComponent<AIBrainBehaviour>();
              character.gameObject.GetOrAddComponent<RFG.BehaviourTree.BehaviourTreeRunner>();
              EditorUtility.SetDirty(character);
            }
            if (GUILayout.Button("Add Aggro"))
            {
              character.gameObject.GetOrAddComponent<Aggro>();
              EditorUtility.SetDirty(character);
            }

          }


        }
      });
      rootElement.Add(container);

      return rootElement;
    }

    private void AddNewMovementState()
    {
      Character character = (Character)target;
      switch (addMovementStateType)
      {
        case AddMovementStateType.Movement:
          character.MovementState.StatePack.GenerateMovementStates();
          break;
        case AddMovementStateType.Jump:
          character.MovementState.StatePack.GenerateJumpState();
          break;
        case AddMovementStateType.DoubleJump:
          character.MovementState.StatePack.GenerateDoubleJumpState();
          break;
        case AddMovementStateType.JumpFlip:
          character.MovementState.StatePack.GenerateJumpFlipState();
          break;
        case AddMovementStateType.Fall:
          character.MovementState.StatePack.GenerateFallState();
          break;
        case AddMovementStateType.PrimaryAttack:
          character.MovementState.StatePack.GeneratePrimaryAttackState();
          break;
        case AddMovementStateType.SecondaryAttack:
          character.MovementState.StatePack.GenerateSecondaryAttackState();
          break;
      }
    }

    private void AddNewAbility()
    {
      Character character = (Character)target;
      switch (addAbilityType)
      {
        case AddAbilityType.JumpAbility:
          character.gameObject.GetOrAddComponent<JumpAbility>();
          character.MovementState.StatePack.GenerateJumpState();
          break;
        case AddAbilityType.AttackAbility:
          character.gameObject.GetOrAddComponent<AttackAbility>();
          character.MovementState.StatePack.GenerateAttackAbilityStates();
          break;
        case AddAbilityType.DashAbility:
          character.gameObject.GetOrAddComponent<DashAbility>();
          character.MovementState.StatePack.GenerateDashAbilityStates();
          break;
        case AddAbilityType.LadderClimbingAbility:
          character.gameObject.GetOrAddComponent<LadderClimbingAbility>();
          character.MovementState.StatePack.GenerateLadderClimbingAbilityStates();
          break;
        case AddAbilityType.SlideAbility:
          character.gameObject.GetOrAddComponent<SlideAbility>();
          character.MovementState.StatePack.GenerateSlideAbilityStates();
          break;
      }
    }
  }
}