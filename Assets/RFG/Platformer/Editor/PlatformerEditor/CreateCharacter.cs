using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

namespace RFG
{
  public static class CreateCharacter
  {
    public static VisualElement CreateCharacterContainer()
    {
      VisualElement container = VisualElementUtils.CreateControlsContainer("platformer-Character");
      Label title = VisualElementUtils.CreateTitle("Character");
      VisualElement controls = container.Q<VisualElement>("platformer-Character-controls");

      VisualElement manager = VisualElementUtils.CreateButtonContainer("platformer-Character-manager");
      VisualElement buttons = manager.Q<VisualElement>("platformer-Character-manager-buttons");

      TextField textField = new TextField()
      {
        label = "Character Name"
      };

      Button createCharacterButton = new Button(() =>
      {
        CreatePlayer(textField.value);
      })
      {
        name = "character-button",
        text = "Create Player Character"
      };

      Button createAiButton = new Button(() =>
      {
        CreateAI(textField.value);
      })
      {
        name = "ai-character-button",
        text = "Create AI Character"
      };

      controls.Add(title);
      controls.Add(textField);
      buttons.Add(createCharacterButton);
      buttons.Add(createAiButton);

      controls.Add(manager);

      return container;
    }

    private static void CreatePlayer(string name)
    {
      string newFolderPath = EditorUtils.CreateFolderStructure(name, "Prefabs", "Sprites", "Settings");
      AssetDatabase.CreateFolder(newFolderPath + "/Sprites", "Animations");

      GameObject gameObject = new GameObject();
      gameObject.name = name;

      Character character = gameObject.GetOrAddComponent<Character>();
      character.CharacterType = CharacterType.Player;
      gameObject.layer = LayerMask.NameToLayer("Player");
      gameObject.tag = "Player";

      Rigidbody2D rigidbody = gameObject.GetOrAddComponent<Rigidbody2D>();
      rigidbody.useAutoMass = false;
      rigidbody.mass = 1;
      rigidbody.drag = 0;
      rigidbody.angularDrag = 0.05f;
      rigidbody.gravityScale = 1;
      rigidbody.interpolation = RigidbodyInterpolation2D.None;
      rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
      rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
      rigidbody.isKinematic = true;
      rigidbody.simulated = true;

      BoxCollider2D collider = gameObject.GetOrAddComponent<BoxCollider2D>();
      collider.isTrigger = true;

      CharacterController2D controller = gameObject.GetOrAddComponent<CharacterController2D>();
      controller.PlatformMask = LayerMask.GetMask("Platforms");
      controller.OneWayPlatformMask = LayerMask.GetMask("OneWayPlatforms");
      controller.MovingPlatformMask = LayerMask.GetMask("MovingPlatforms");
      controller.OneWayMovingPlatformMask = LayerMask.GetMask("OneWayMovingPlatforms");
      controller.StairsMask = LayerMask.GetMask("Stairs");

      gameObject.GetOrAddComponent<SpriteRenderer>();
      Animator animator = gameObject.GetOrAddComponent<Animator>();
      UnityEditor.Animations.AnimatorController animatorController = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath($"{newFolderPath}/Sprites/Animations/{name}.controller");
      animator.runtimeAnimatorController = animatorController;

      gameObject.GetOrAddComponent<PlayerInput>();
      gameObject.GetOrAddComponent<PauseAbility>();
      gameObject.GetOrAddComponent<MovementAbility>();

      // gameObject.GetOrAddComponent<AttackAbility>();
      // gameObject.GetOrAddComponent<DashAbility>();
      // gameObject.GetOrAddComponent<JumpAbility>();
      // gameObject.GetOrAddComponent<LadderClimbingAbility>();
      // gameObject.GetOrAddComponent<LedgeGrabAbility>();
      // gameObject.GetOrAddComponent<PushAbility>();
      // gameObject.GetOrAddComponent<SlideAbility>();
      // gameObject.GetOrAddComponent<SmashDownAbility>();
      // gameObject.GetOrAddComponent<StairsAbility>();
      // gameObject.GetOrAddComponent<SwimAbility>();
      // gameObject.GetOrAddComponent<WallClingingAbility>();
      // gameObject.GetOrAddComponent<WallJumpAbility>();

      // gameObject.GetOrAddComponent<DanglingBehaviour>();
      // gameObject.GetOrAddComponent<HealthBehaviour>();
      // gameObject.GetOrAddComponent<SceneBoundsBehaviour>();

      CreatePacks(gameObject, newFolderPath + "/Settings");
      CreateParams(gameObject, newFolderPath + "/Settings");
      // CreateInventory(gameObject, newFolderPath + "/Items");

      // Create Prefab
      EditorUtils.SaveAsPrefabAsset(gameObject, newFolderPath, name);

      GameObject.DestroyImmediate(gameObject);
    }

    private static void CreateAI(string name)
    {
      string newFolderPath = EditorUtils.CreateFolderStructure(name, "Prefabs", "Sprites", "Settings");
      AssetDatabase.CreateFolder(newFolderPath + "/Sprites", "Animations");

      GameObject gameObject = new GameObject();
      gameObject.name = name;

      Character character = gameObject.GetOrAddComponent<Character>();
      character.CharacterType = CharacterType.AI;
      gameObject.layer = LayerMask.NameToLayer("AI");
      gameObject.tag = "AI";

      Rigidbody2D _rigidbody = gameObject.GetOrAddComponent<Rigidbody2D>();
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

      BoxCollider2D _collider = gameObject.GetOrAddComponent<BoxCollider2D>();
      _collider.isTrigger = true;

      CharacterController2D _controller = gameObject.GetOrAddComponent<CharacterController2D>();
      _controller.PlatformMask = LayerMask.GetMask("Platforms");
      _controller.OneWayPlatformMask = LayerMask.GetMask("OneWayPlatforms");
      _controller.MovingPlatformMask = LayerMask.GetMask("MovingPlatforms");
      _controller.OneWayMovingPlatformMask = LayerMask.GetMask("OneWayMovingPlatforms");
      _controller.StairsMask = LayerMask.GetMask("Stairs");

      gameObject.GetOrAddComponent<SpriteRenderer>();
      Animator animator = gameObject.GetOrAddComponent<Animator>();
      UnityEditor.Animations.AnimatorController animatorController = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath($"{newFolderPath}/Sprites/Animations/{name}.controller");
      animator.runtimeAnimatorController = animatorController;

      //   Aggro _aggro = gameObject.GetOrAddComponent<Aggro>();
      //   _aggro.target1 = character.transform;
      //   _aggro.target2IsPlayer = true;
      //   _aggro.layerMask = LayerMask.GetMask("Player");
      //   _aggro.tags = new string[] { "Player" };

      //   gameObject.GetOrAddComponent<Knockback>();
      //   gameObject.GetOrAddComponent<HealthBehaviour>();

      CreatePacks(gameObject, newFolderPath + "/Settings");
      CreateParams(gameObject, newFolderPath + "/Settings");

      // Create Prefab
      EditorUtils.SaveAsPrefabAsset(gameObject, newFolderPath, name);

      GameObject.DestroyImmediate(gameObject);
    }

    private static void CreatePacks(GameObject activeGameObject, string path)
    {
      SettingsPack settingsPack = EditorUtils.CreateScriptableObject<SettingsPack>(path);
      EditorUtility.SetDirty(settingsPack);

      StatePack characterStatePack = EditorUtils.CreateScriptableObject<StatePack>(path, "CharacterStatePack");
      EditorUtility.SetDirty(characterStatePack);

      StatePack movementStatePack = EditorUtils.CreateScriptableObject<StatePack>(path, "MovementStatePack");
      EditorUtility.SetDirty(movementStatePack);

      if (activeGameObject != null)
      {
        Character character = activeGameObject.GetOrAddComponent<Character>();
        if (character != null)
        {
          character.SettingsPack = settingsPack;
          if (character.CharacterState == null)
          {
            character.CharacterState = new StateMachine();
          }
          character.CharacterState.StatePack = characterStatePack;
          if (character.MovementState == null)
          {
            character.MovementState = new StateMachine();
          }
          character.MovementState.StatePack = movementStatePack;

          characterStatePack.GenerateCharacterStates();
          movementStatePack.GenerateMovementStates();
        }
      }
    }

    private static void CreateParams(GameObject activeGameObject, string path)
    {
      CharacterControllerParameters2D controllerParams = EditorUtils.CreateScriptableObject<CharacterControllerParameters2D>(path);
      EditorUtility.SetDirty(controllerParams);
      if (activeGameObject != null)
      {
        CharacterController2D controller2D = activeGameObject.GetOrAddComponent<CharacterController2D>();
        if (controller2D != null)
        {
          controller2D.DefaultParameters = controllerParams;
        }
      }
    }

    private static void CreateInventory(GameObject activeGameObject, string path)
    {
      Inventory inventory = EditorUtils.CreateScriptableObject<Inventory>(path);
      EditorUtility.SetDirty(inventory);

      if (activeGameObject != null)
      {
        PlayerInventory playerInventory = activeGameObject.GetOrAddComponent<PlayerInventory>();
        if (playerInventory != null)
        {
          playerInventory.Inventory = inventory;
        }
      }
    }

  }
}
