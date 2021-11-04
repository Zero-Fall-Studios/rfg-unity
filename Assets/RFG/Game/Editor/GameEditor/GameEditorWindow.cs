using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace RFG
{
  public class GameEditorWindow : EditorWindow
  {
    [MenuItem("RFG/Game Editor Window")]
    public static void ShowWindow()
    {
      GetWindow<GameEditorWindow>("GameEditorWindow");
    }

    public virtual void CreateGUI()
    {
      VisualElement root = rootVisualElement;

      VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RFG/Game/Editor/GameEditor/GameEditorWindow.uxml");
      visualTree.CloneTree(root);

      StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/RFG/Game/Editor/GameEditor/GameEditorWindow.uss");
      root.styleSheets.Add(styleSheet);

      VisualElement mainContainer = root.Q<VisualElement>("container");

      mainContainer.Add(CreateGameManager());
    }

    private VisualElement CreateGameManager()
    {
      VisualElement gameManager = CreateContainer("game-manager");

      VisualElement gameManagerButtons = gameManager.Q<VisualElement>("game-manager-buttons");

      Button addGameManagerButton = new Button();
      addGameManagerButton.name = "game-manager-button";
      addGameManagerButton.text = "Add Game Manager";
      addGameManagerButton.clicked += () =>
      {
        EditorUtils.CreatePrefab("Assets/RFG/Game/Prefabs", "GameManager");
      };

      gameManagerButtons.Add(addGameManagerButton);

      return gameManager;
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

  }
}