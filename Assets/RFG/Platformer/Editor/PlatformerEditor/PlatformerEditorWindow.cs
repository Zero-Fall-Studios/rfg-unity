using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RFG
{
  public class PlatformerEditorWindow : EditorWindow
  {
    [MenuItem("RFG/Platformer Editor Window")]
    public static void ShowWindow()
    {
      GetWindow<PlatformerEditorWindow>("PlatformerEditorWindow");
    }

    public virtual void CreateGUI()
    {
      VisualElement root = rootVisualElement;

      VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RFG/Platformer/Editor/PlatformerEditor/PlatformerEditorWindow.uxml");
      visualTree.CloneTree(root);

      StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/RFG/Platformer/Editor/PlatformerEditor/PlatformerEditorWindow.uss");
      root.styleSheets.Add(styleSheet);

      VisualElement mainContainer = root.Q<VisualElement>("container");

      mainContainer.Add(CreateManager());
      mainContainer.Add(CreateCharacter());
      mainContainer.Add(CreateCharacterStates());
      mainContainer.Add(CreateCharacterPacks());
      mainContainer.Add(CreateCharacterParams());
    }

    private VisualElement CreateManager()
    {
      VisualElement platformerManager = CreateContainer("platformer-manager");

      VisualElement platformerManagerButtons = platformerManager.Q<VisualElement>("platformer-manager-buttons");

      Button addTagsButton = new Button(() =>
      {
        EditorUtils.AddTags(new string[] { "Player" });
      })
      {
        name = "tags-button",
        text = "Add Platformer Tags"
      };

      Button addLayersButton = new Button(() =>
      {
        EditorUtils.AddLayers(new string[] { "Player", "Platforms", "OneWayPlatforms", "MovingPlatforms", "OneWayMovingPlatforms", "Stairs", "PhysicsVolume" });
      })
      {
        name = "layers-button",
        text = "Add Platformer Layers"
      };

      platformerManagerButtons.Add(addTagsButton);
      platformerManagerButtons.Add(addLayersButton);

      return platformerManager;
    }

    protected VisualElement CreateContainer(string name)
    {
      VisualElement container = new VisualElement();
      container.name = name;
      container.AddToClassList("container");

      Label label = new Label();
      label.name = $"{name}-label";
      label.AddToClassList("container-label");

      VisualElement buttons = new VisualElement();
      buttons.name = $"{name}-buttons";
      buttons.AddToClassList("buttons");

      container.Add(label);
      container.Add(buttons);

      return container;
    }

    #region Create Character
    private VisualElement CreateCharacter()
    {
      VisualElement platformerCharacter = CreateContainer("platformer-Character");

      VisualElement platformerCharacterButtons = platformerCharacter.Q<VisualElement>("platformer-Character-buttons");

      Button createPlayerButton = new Button(() =>
      {
        CreatePlayer();
      })
      {
        name = "player-button",
        text = "Create Player"
      };

      Button createAiButton = new Button(() =>
      {
        CreateAICharacter();
      })
      {
        name = "layers-button",
        text = "Create AI Character"
      };

      platformerCharacterButtons.Add(createPlayerButton);
      platformerCharacterButtons.Add(createAiButton);

      return platformerCharacter;
    }

    private void CreatePlayer()
    {
      GameObject activeGameObject = Selection.activeGameObject;

      if (activeGameObject == null)
      {
        LogExt.Warn<PlatformerEditorWindow>("Please select a game object first before clicking create player");
        return;
      }

      Character character = activeGameObject.GetOrAddComponent<Character>();
      character.CharacterType = CharacterType.Player;
      character.gameObject.layer = LayerMask.NameToLayer("Player");
      character.gameObject.tag = "Player";

      Rigidbody2D _rigidbody = activeGameObject.GetOrAddComponent<Rigidbody2D>();
      _rigidbody.useAutoMass = false;
      _rigidbody.mass = 1;
      _rigidbody.drag = 0;
      _rigidbody.angularDrag = 0.05f;
      _rigidbody.gravityScale = 1;
      _rigidbody.interpolation = RigidbodyInterpolation2D.None;
      _rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
      _rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
      _rigidbody.isKinematic = true;
      _rigidbody.simulated = true;

      BoxCollider2D _collider = activeGameObject.GetOrAddComponent<BoxCollider2D>();
      _collider.isTrigger = true;

      CharacterController2D _controller = activeGameObject.GetOrAddComponent<CharacterController2D>();
      _controller.PlatformMask = LayerMask.GetMask("Platforms");
      _controller.OneWayPlatformMask = LayerMask.GetMask("OneWayPlatforms");
      _controller.MovingPlatformMask = LayerMask.GetMask("MovingPlatforms");
      _controller.OneWayMovingPlatformMask = LayerMask.GetMask("OneWayMovingPlatforms");
      _controller.StairsMask = LayerMask.GetMask("Stairs");

      activeGameObject.GetOrAddComponent<Animator>();
      activeGameObject.GetOrAddComponent<HealthBehaviour>();
      activeGameObject.GetOrAddComponent<MovementAbility>();
      activeGameObject.GetOrAddComponent<JumpAbility>();
      activeGameObject.GetOrAddComponent<AttackAbility>();
      activeGameObject.GetOrAddComponent<DashAbility>();
      activeGameObject.GetOrAddComponent<PauseAbility>();
      activeGameObject.GetOrAddComponent<WallClingingAbility>();
      activeGameObject.GetOrAddComponent<WallJumpAbility>();
      activeGameObject.GetOrAddComponent<StairsAbility>();
      activeGameObject.GetOrAddComponent<LedgeGrabAbility>();
      activeGameObject.GetOrAddComponent<DanglingBehaviour>();
      activeGameObject.GetOrAddComponent<SlideAbility>();
    }

    private void CreateAICharacter()
    {
      GameObject activeGameObject = Selection.activeGameObject;

      if (activeGameObject == null)
      {
        LogExt.Warn<PlatformerEditorWindow>("Please select a game object first before clicking create ai character");
        return;
      }

      Character character = activeGameObject.GetOrAddComponent<Character>();
      character.CharacterType = CharacterType.AI;
      character.gameObject.layer = LayerMask.NameToLayer("AI Character");
      character.gameObject.tag = "AI Character";

      Rigidbody2D _rigidbody = activeGameObject.GetOrAddComponent<Rigidbody2D>();
      _rigidbody.useAutoMass = false;
      _rigidbody.mass = 1;
      _rigidbody.drag = 0;
      _rigidbody.angularDrag = 0.05f;
      _rigidbody.gravityScale = 1;
      _rigidbody.interpolation = RigidbodyInterpolation2D.None;
      _rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
      _rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
      _rigidbody.isKinematic = true;
      _rigidbody.simulated = true;

      BoxCollider2D _collider = activeGameObject.GetOrAddComponent<BoxCollider2D>();
      _collider.isTrigger = true;

      CharacterController2D _controller = activeGameObject.GetOrAddComponent<CharacterController2D>();
      _controller.PlatformMask = LayerMask.GetMask("Platforms") | LayerMask.GetMask("AI Edge Colliders");
      _controller.OneWayPlatformMask = LayerMask.GetMask("OneWayPlatforms");
      _controller.MovingPlatformMask = LayerMask.GetMask("MovingPlatforms");
      _controller.OneWayMovingPlatformMask = LayerMask.GetMask("OneWayMovingPlatforms");
      _controller.StairsMask = LayerMask.GetMask("Stairs");

      Aggro _aggro = activeGameObject.GetOrAddComponent<Aggro>();
      _aggro.target1 = character.transform;
      _aggro.target2IsPlayer = true;
      _aggro.layerMask = LayerMask.GetMask("Player");
      _aggro.tags = new string[] { "Player" };

      activeGameObject.GetOrAddComponent<Knockback>();
      activeGameObject.GetOrAddComponent<HealthBehaviour>();
    }
    #endregion

    #region Create States
    private VisualElement CreateCharacterStates()
    {
      VisualElement platformerCharacterStates = CreateContainer("platformer-CharacterStates");

      VisualElement platformerCharacterStatesButtons = platformerCharacterStates.Q<VisualElement>("platformer-CharacterStates-buttons");

      Button createStatesButton = new Button(() =>
      {
        CreateStates();
      })
      {
        name = "states-button",
        text = "Create Character States"
      };

      platformerCharacterStatesButtons.Add(createStatesButton);

      return platformerCharacterStates;
    }

    private void CreateStates()
    {
      GameObject activeGameObject = Selection.activeGameObject;

      if (activeGameObject == null)
      {
        LogExt.Warn<PlatformerEditorWindow>("Please select a game object first before clicking create states");
        return;
      }

      Character character = activeGameObject.GetOrAddComponent<Character>();

      string path;
      if (EditorUtils.TryGetActiveFolderPath(out path))
      {
        StatePack characterStatePack = EditorUtils.CreateScriptableObject<StatePack>(path, "CharacterStatePack");
        characterStatePack.DefaultState = EditorUtils.CreateScriptableObject<SpawnState>(path);
        characterStatePack.Add(characterStatePack.DefaultState);
        characterStatePack.Add(EditorUtils.CreateScriptableObject<AliveState>(path));
        characterStatePack.Add(EditorUtils.CreateScriptableObject<DeadState>(path));
        characterStatePack.Add(EditorUtils.CreateScriptableObject<DeathState>(path));
        EditorUtility.SetDirty(characterStatePack);

        StatePack movementStatePack = EditorUtils.CreateScriptableObject<StatePack>(path, "MovementStatePack");
        movementStatePack.DefaultState = EditorUtils.CreateScriptableObject<IdleState>(path);
        movementStatePack.Add(movementStatePack.DefaultState);
        movementStatePack.Add(EditorUtils.CreateScriptableObject<DanglingState>(path));
        movementStatePack.Add(EditorUtils.CreateScriptableObject<FallingState>(path));
        movementStatePack.Add(EditorUtils.CreateScriptableObject<JumpingState>(path));
        movementStatePack.Add(EditorUtils.CreateScriptableObject<JumpingFlipState>(path));
        movementStatePack.Add(EditorUtils.CreateScriptableObject<KnockbackState>(path));
        movementStatePack.Add(EditorUtils.CreateScriptableObject<LandedState>(path));
        movementStatePack.Add(EditorUtils.CreateScriptableObject<RunningState>(path));
        movementStatePack.Add(EditorUtils.CreateScriptableObject<SwimmingState>(path));
        movementStatePack.Add(EditorUtils.CreateScriptableObject<WalkingState>(path));
        movementStatePack.Add(EditorUtils.CreateScriptableObject<LedgeGrabState>(path));
        movementStatePack.Add(EditorUtils.CreateScriptableObject<LedgeClimbingState>(path));
        movementStatePack.Add(EditorUtils.CreateScriptableObject<CrouchIdleState>(path));
        movementStatePack.Add(EditorUtils.CreateScriptableObject<CrouchWalkingState>(path));
        movementStatePack.Add(EditorUtils.CreateScriptableObject<WalkingUpSlopeState>(path));
        movementStatePack.Add(EditorUtils.CreateScriptableObject<RunningUpSlopeState>(path));
        movementStatePack.Add(EditorUtils.CreateScriptableObject<WalkingDownSlopeState>(path));
        movementStatePack.Add(EditorUtils.CreateScriptableObject<RunningDownSlopeState>(path));
        movementStatePack.Add(EditorUtils.CreateScriptableObject<SlidingState>(path));

        movementStatePack.Add(EditorUtils.CreateScriptableObject<PrimaryAttackStartedState>(path));
        movementStatePack.Add(EditorUtils.CreateScriptableObject<PrimaryAttackPerformedState>(path));
        movementStatePack.Add(EditorUtils.CreateScriptableObject<PrimaryAttackCanceledState>(path));
        movementStatePack.Add(EditorUtils.CreateScriptableObject<SecondaryAttackStartedState>(path));
        movementStatePack.Add(EditorUtils.CreateScriptableObject<SecondaryAttackPerformedState>(path));
        movementStatePack.Add(EditorUtils.CreateScriptableObject<SecondaryAttackCanceledState>(path));

        movementStatePack.Add(EditorUtils.CreateScriptableObject<SmashDownStartedState>(path));
        movementStatePack.Add(EditorUtils.CreateScriptableObject<SmashDownPerformedState>(path));
        movementStatePack.Add(EditorUtils.CreateScriptableObject<SmashDownCollidedState>(path));
        EditorUtility.SetDirty(movementStatePack);

        character.CharacterState.StatePack = characterStatePack;
        character.MovementState.StatePack = movementStatePack;

      }
    }
    #endregion

    #region Create Packs
    private VisualElement CreateCharacterPacks()
    {
      VisualElement platformerCharacterPacks = CreateContainer("platformer-CharacterPacks");

      VisualElement platformerCharacterPacksButtons = platformerCharacterPacks.Q<VisualElement>("platformer-CharacterPacks-buttons");

      Button createPacksButton = new Button(() =>
      {
        CreatePacks();
      })
      {
        name = "Packs-button",
        text = "Create Character Packs"
      };

      platformerCharacterPacksButtons.Add(createPacksButton);

      return platformerCharacterPacks;
    }

    private void CreatePacks()
    {
      GameObject activeGameObject = Selection.activeGameObject;

      if (activeGameObject == null)
      {
        LogExt.Warn<PlatformerEditorWindow>("Please select a game object first before clicking create states");
        return;
      }

      Character character = activeGameObject.GetOrAddComponent<Character>();

      string path;
      if (EditorUtils.TryGetActiveFolderPath(out path))
      {
        InputPack inputPack = EditorUtils.CreateScriptableObject<InputPack>(path);
        EditorUtility.SetDirty(inputPack);

        SettingsPack settingsPack = EditorUtils.CreateScriptableObject<SettingsPack>(path);
        EditorUtility.SetDirty(settingsPack);

        character.InputPack = inputPack;
        character.SettingsPack = settingsPack;
      }
    }
    #endregion

    #region Create Params
    private VisualElement CreateCharacterParams()
    {
      VisualElement platformerCharacterParams = CreateContainer("platformer-CharacterParams");

      VisualElement platformerCharacterParamsButtons = platformerCharacterParams.Q<VisualElement>("platformer-CharacterParams-buttons");

      Button createParamsButton = new Button(() =>
      {
        CreateParams();
      })
      {
        name = "Params-button",
        text = "Create Character Controller Params"
      };

      platformerCharacterParamsButtons.Add(createParamsButton);

      return platformerCharacterParams;
    }

    private void CreateParams()
    {
      GameObject activeGameObject = Selection.activeGameObject;

      if (activeGameObject == null)
      {
        LogExt.Warn<PlatformerEditorWindow>("Please select a game object first before clicking create states");
        return;
      }

      CharacterController2D controller2D = activeGameObject.GetOrAddComponent<CharacterController2D>();

      string path;
      if (EditorUtils.TryGetActiveFolderPath(out path))
      {
        CharacterControllerParameters2D controllerParams = EditorUtils.CreateScriptableObject<CharacterControllerParameters2D>(path);
        EditorUtility.SetDirty(controllerParams);
        controller2D.DefaultParameters = controllerParams;
      }
    }
    #endregion

  }
}