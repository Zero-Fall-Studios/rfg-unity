using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RFG
{
  [CustomEditor(typeof(Character))]
  public class CharacterEditorWindow : Editor
  {
    public enum AddAbilityType { Select, AttackAbility, DashAbility, JumpAbility, LadderClimbingAbility, LedgeGrabAbility, PushAbility, SlideAbility, SmashDownAbility, SwimAbility, WallClingingAbility, WallJumpAbility }
    public enum AddBehaviourType { Select, DanglingBehaviour, HealthBehaviour, SceneBoundsBehaviour }
    public enum AddMovementStateType { Select, Crouch, Damage, Dangling, Dash, DoubleJump, Fall, Jump, JumpFlip, Ladder, LedgeGrab, Movement, PrimaryAttack, Push, SecondaryAttack, Slide, SmashDown, Swim, WallClinging, WallJump }
    private VisualElement rootElement;
    private Editor editor;
    private AddAbilityType addAbilityType;
    private AddBehaviourType addBehaviourType;
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

            EditorGUILayout.LabelField("Behaviours", boldtext);
            AddBehaviourType newAddBehaviourType = (AddBehaviourType)EditorGUILayout.EnumPopup("Add Behaviour: ", addBehaviourType);

            if (!newAddBehaviourType.Equals(addBehaviourType))
            {
              addBehaviourType = newAddBehaviourType;
              AddNewBehaviour();
              addBehaviourType = AddBehaviourType.Select;
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
        case AddMovementStateType.Crouch:
          character.MovementState.StatePack.GenerateCrouchStates();
          break;
        case AddMovementStateType.Damage:
          character.MovementState.StatePack.GenerateDamageState();
          break;
        case AddMovementStateType.Dangling:
          character.MovementState.StatePack.GenerateDanglingBehaviourStates();
          break;
        case AddMovementStateType.Dash:
          character.MovementState.StatePack.GenerateDashAbilityStates();
          break;
        case AddMovementStateType.DoubleJump:
          character.MovementState.StatePack.GenerateDoubleJumpState();
          break;
        case AddMovementStateType.Fall:
          character.MovementState.StatePack.GenerateFallState();
          break;
        case AddMovementStateType.Jump:
          character.MovementState.StatePack.GenerateJumpState();
          break;
        case AddMovementStateType.JumpFlip:
          character.MovementState.StatePack.GenerateJumpFlipState();
          break;
        case AddMovementStateType.Ladder:
          character.MovementState.StatePack.GenerateLadderClimbingAbilityStates();
          break;
        case AddMovementStateType.LedgeGrab:
          character.MovementState.StatePack.GenerateLedgeGrabAbilityStates();
          break;
        case AddMovementStateType.Movement:
          character.MovementState.StatePack.GenerateMovementStates();
          break;
        case AddMovementStateType.PrimaryAttack:
          character.MovementState.StatePack.GeneratePrimaryAttackState();
          break;
        case AddMovementStateType.Push:
          character.MovementState.StatePack.GeneratePushAbilityStates();
          break;
        case AddMovementStateType.SecondaryAttack:
          character.MovementState.StatePack.GenerateSecondaryAttackState();
          break;
        case AddMovementStateType.Slide:
          character.MovementState.StatePack.GenerateSlideAbilityStates();
          break;
        case AddMovementStateType.SmashDown:
          character.MovementState.StatePack.GenerateSmashDownAbilityStates();
          break;
        case AddMovementStateType.Swim:
          character.MovementState.StatePack.GenerateSwimAbilityStates();
          break;
        case AddMovementStateType.WallClinging:
          character.MovementState.StatePack.GenerateWallClingingAbilityStates();
          break;
        case AddMovementStateType.WallJump:
          character.MovementState.StatePack.GenerateWallJumpAbilityStates();
          break;
      }
    }

    private void AddNewAbility()
    {
      Character character = (Character)target;
      switch (addAbilityType)
      {
        case AddAbilityType.AttackAbility:
          character.gameObject.GetOrAddComponent<AttackAbility>();
          character.MovementState.StatePack.GenerateAttackAbilityStates();
          break;
        case AddAbilityType.DashAbility:
          character.gameObject.GetOrAddComponent<DashAbility>();
          character.MovementState.StatePack.GenerateDashAbilityStates();
          break;
        case AddAbilityType.JumpAbility:
          character.gameObject.GetOrAddComponent<JumpAbility>();
          character.MovementState.StatePack.GenerateJumpState();
          break;
        case AddAbilityType.LadderClimbingAbility:
          character.gameObject.GetOrAddComponent<LadderClimbingAbility>();
          character.MovementState.StatePack.GenerateLadderClimbingAbilityStates();
          break;
        case AddAbilityType.LedgeGrabAbility:
          character.gameObject.GetOrAddComponent<LedgeGrabAbility>();
          character.MovementState.StatePack.GenerateLedgeGrabAbilityStates();
          break;
        case AddAbilityType.PushAbility:
          character.gameObject.GetOrAddComponent<PushAbility>();
          character.MovementState.StatePack.GeneratePushAbilityStates();
          break;
        case AddAbilityType.SlideAbility:
          character.gameObject.GetOrAddComponent<SlideAbility>();
          character.MovementState.StatePack.GenerateSlideAbilityStates();
          break;
        case AddAbilityType.SmashDownAbility:
          character.gameObject.GetOrAddComponent<SmashDownAbility>();
          character.MovementState.StatePack.GenerateSmashDownAbilityStates();
          break;
        case AddAbilityType.SwimAbility:
          character.gameObject.GetOrAddComponent<SwimAbility>();
          character.MovementState.StatePack.GenerateSwimAbilityStates();
          break;
        case AddAbilityType.WallClingingAbility:
          character.gameObject.GetOrAddComponent<WallClingingAbility>();
          character.MovementState.StatePack.GenerateWallClingingAbilityStates();
          break;
        case AddAbilityType.WallJumpAbility:
          character.gameObject.GetOrAddComponent<WallJumpAbility>();
          character.MovementState.StatePack.GenerateWallJumpAbilityStates();
          break;
      }
    }

    private void AddNewBehaviour()
    {
      Character character = (Character)target;
      switch (addBehaviourType)
      {
        case AddBehaviourType.DanglingBehaviour:
          character.gameObject.GetOrAddComponent<DanglingBehaviour>();
          character.MovementState.StatePack.GenerateDanglingBehaviourStates();
          break;
        case AddBehaviourType.HealthBehaviour:
          character.gameObject.GetOrAddComponent<HealthBehaviour>();
          break;
        case AddBehaviourType.SceneBoundsBehaviour:
          character.gameObject.GetOrAddComponent<SceneBoundsBehaviour>();
          break;
      }
    }
  }
}