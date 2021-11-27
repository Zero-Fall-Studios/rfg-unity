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

    #region GUI
    public virtual void CreateGUI()
    {
      VisualElement root = rootVisualElement;

      VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RFG/Platformer/Editor/PlatformerEditor/PlatformerEditorWindow.uxml");
      visualTree.CloneTree(root);

      StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/RFG/Platformer/Editor/PlatformerEditor/PlatformerEditorWindow.uss");
      root.styleSheets.Add(styleSheet);

      VisualElement mainContainer = root.Q<VisualElement>("container");

      mainContainer.Add(CreateManager());
      mainContainer.Add(CreateCharacterContainer());
    }

    protected VisualElement CreateButtonContainer(string name)
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

    protected VisualElement CreateControlsContainer(string name)
    {
      VisualElement container = new VisualElement();
      container.name = name;
      container.AddToClassList("container");

      Label label = new Label();
      label.name = $"{name}-label";
      label.AddToClassList("container-label");

      VisualElement controls = new VisualElement();
      controls.name = $"{name}-controls";
      controls.AddToClassList("cotrols");

      container.Add(label);
      container.Add(controls);

      return container;
    }
    #endregion

    #region Manager
    private VisualElement CreateManager()
    {
      VisualElement platformerManager = CreateButtonContainer("platformer-manager");

      VisualElement platformerManagerButtons = platformerManager.Q<VisualElement>("platformer-manager-buttons");

      Button addTagsButton = new Button(() =>
      {
        EditorUtils.AddTags(new string[] { "Player" });
      })
      {
        name = "tags-button",
        text = "Add Tags"
      };

      Button addLayersButton = new Button(() =>
      {
        EditorUtils.AddLayers(new string[] { "Player", "Platforms", "OneWayPlatforms", "MovingPlatforms", "OneWayMovingPlatforms", "Stairs", "PhysicsVolume" });
      })
      {
        name = "layers-button",
        text = "Add Layers"
      };

      Button addSortingLayersButton = new Button(() =>
      {
        EditorUtils.AddSortingLayers(new string[] { "Background 1", "Background 2", "Background 3", "Background 4", "Background 5", "Background 6", "Player", "Foreground 1", "Foreground 2", "Foreground 3", "Foreground 4", "Foreground 5", "Foreground 6" });
      })
      {
        name = "layers-button",
        text = "Add Sorting Layers"
      };

      platformerManagerButtons.Add(addTagsButton);
      platformerManagerButtons.Add(addLayersButton);
      platformerManagerButtons.Add(addSortingLayersButton);

      return platformerManager;
    }
    #endregion

    #region Create Character
    private VisualElement CreateCharacterContainer()
    {
      VisualElement container = CreateControlsContainer("platformer-Character");

      VisualElement controls = container.Q<VisualElement>("platformer-Character-controls");

      TextField textField = new TextField()
      {
        label = "Character Name"
      };

      Button createCharacterButton = new Button(() =>
      {
        CreateCharacter(textField.value);
      })
      {
        name = "character-button",
        text = "Create Character"
      };

      Button createAiButton = new Button(() =>
      {
        CreateAICharacter(textField.value);
      })
      {
        name = "ai-character-button",
        text = "Create AI Character"
      };

      controls.Add(textField);
      controls.Add(createCharacterButton);
      controls.Add(createAiButton);

      return container;
    }

    private void CreateCharacter(string name)
    {
      GameObject activeGameObject = Selection.activeGameObject;

      if (activeGameObject == null)
      {
        activeGameObject = new GameObject();
        EditorGUIUtility.PingObject(activeGameObject);
        Selection.activeGameObject = activeGameObject;
      }

      activeGameObject.name = name;

      Character character = activeGameObject.GetOrAddComponent<Character>();
      character.CharacterType = CharacterType.Player;
      character.gameObject.layer = LayerMask.NameToLayer("Player");
      character.gameObject.tag = "Player";

      Rigidbody2D rigidbody = activeGameObject.GetOrAddComponent<Rigidbody2D>();
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

      BoxCollider2D collider = activeGameObject.GetOrAddComponent<BoxCollider2D>();
      collider.isTrigger = true;

      CharacterController2D controller = activeGameObject.GetOrAddComponent<CharacterController2D>();
      controller.PlatformMask = LayerMask.GetMask("Platforms");
      controller.OneWayPlatformMask = LayerMask.GetMask("OneWayPlatforms");
      controller.MovingPlatformMask = LayerMask.GetMask("MovingPlatforms");
      controller.OneWayMovingPlatformMask = LayerMask.GetMask("OneWayMovingPlatforms");
      controller.StairsMask = LayerMask.GetMask("Stairs");

      activeGameObject.GetOrAddComponent<SpriteRenderer>();
      activeGameObject.GetOrAddComponent<Animator>();

      activeGameObject.GetOrAddComponent<AttackAbility>();
      activeGameObject.GetOrAddComponent<DashAbility>();
      activeGameObject.GetOrAddComponent<JumpAbility>();
      activeGameObject.GetOrAddComponent<LadderClimbingAbility>();
      activeGameObject.GetOrAddComponent<LedgeGrabAbility>();
      activeGameObject.GetOrAddComponent<MovementAbility>();
      activeGameObject.GetOrAddComponent<PauseAbility>();
      activeGameObject.GetOrAddComponent<PushAbility>();
      activeGameObject.GetOrAddComponent<SlideAbility>();
      activeGameObject.GetOrAddComponent<SmashDownAbility>();
      activeGameObject.GetOrAddComponent<StairsAbility>();
      activeGameObject.GetOrAddComponent<SwimAbility>();
      activeGameObject.GetOrAddComponent<WallClingingAbility>();
      activeGameObject.GetOrAddComponent<WallJumpAbility>();

      activeGameObject.GetOrAddComponent<DanglingBehaviour>();
      activeGameObject.GetOrAddComponent<HealthBehaviour>();
      activeGameObject.GetOrAddComponent<SceneBoundsBehaviour>();

      string newFolderPath = CreateFolderStructure(name);
      CreatePacks(activeGameObject, newFolderPath + "/Settings");
      CreateParams(activeGameObject, newFolderPath + "/Settings");
      CreateInventory(activeGameObject, newFolderPath + "/Items");
    }

    private void CreateAICharacter(string name)
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

    #region Create
    private string CreateFolderStructure(string name)
    {
      string path;
      if (EditorUtils.TryGetActiveFolderPath(out path))
      {
        string guid = AssetDatabase.CreateFolder(path, name);
        string newFolderPath = AssetDatabase.GUIDToAssetPath(guid);

        AssetDatabase.CreateFolder(newFolderPath, "Animations");
        AssetDatabase.CreateFolder(newFolderPath, "Settings");
        AssetDatabase.CreateFolder(newFolderPath, "Items");
        AssetDatabase.CreateFolder(newFolderPath, "Sprites");

        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(this);
        return newFolderPath;
      }

      return null;
    }

    private void CreatePacks(GameObject activeGameObject, string path)
    {
      InputPack inputPack = EditorUtils.CreateScriptableObject<InputPack>(path);
      EditorUtility.SetDirty(inputPack);

      SettingsPack settingsPack = EditorUtils.CreateScriptableObject<SettingsPack>(path);
      EditorUtility.SetDirty(settingsPack);

      StatePack characterStatePack = EditorUtils.CreateScriptableObject<StatePack>(path, "CharacterStatePack");
      StatePack movementStatePack = EditorUtils.CreateScriptableObject<StatePack>(path, "MovementStatePack");

      if (activeGameObject != null)
      {
        Character character = activeGameObject.GetOrAddComponent<Character>();
        if (character != null)
        {
          character.InputPack = inputPack;
          character.SettingsPack = settingsPack;
          character.CharacterState.StatePack = characterStatePack;
          character.MovementState.StatePack = movementStatePack;

          characterStatePack.GenerateCharacterStates();
          movementStatePack.GenerateMovementStates();
        }
      }
    }

    private void CreateParams(GameObject activeGameObject, string path)
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



    private void CreateInventory(GameObject activeGameObject, string path)
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
    #endregion

  }
}