using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;

namespace RFG
{
  public class ItemEditorWindow : EditorWindow
  {
    [MenuItem("RFG/Item Editor Window")]
    public static void ShowWindow()
    {
      GetWindow<ItemEditorWindow>("ItemEditorWindow");
    }

    public virtual void CreateGUI()
    {
      VisualElement root = rootVisualElement;

      VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RFG/Items/Editor/ItemEditor/ItemEditorWindow.uxml");
      visualTree.CloneTree(root);

      StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/RFG/Items/Editor/ItemEditor/ItemEditorWindow.uss");
      root.styleSheets.Add(styleSheet);

      VisualElement mainContainer = root.Q<VisualElement>("container");

      mainContainer.Add(CreatePickUpManager());
      mainContainer.Add(CreateGenerateItems());
    }

    private VisualElement CreateGenerateItems()
    {
      VisualElement inventoryGenerateItems = CreateContainer("inventory-generate-items");

      Label label = new Label()
      {
        text = "Generate Items"
      };
      label.AddToClassList("container-label");

      TextField createPath = new TextField()
      {
        label = "Path"
      };

      TextField className = new TextField()
      {
        label = "Class Name"
      };

      Button generateButton = new Button();
      generateButton.text = "Generate Items";
      generateButton.clicked += () =>
      {
        GenerateItems(createPath.value, className.value);
      };

      inventoryGenerateItems.Add(label);
      inventoryGenerateItems.Add(createPath);
      inventoryGenerateItems.Add(className);
      inventoryGenerateItems.Add(generateButton);

      return inventoryGenerateItems;
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

    private VisualElement CreatePickUpManager()
    {
      VisualElement manager = CreateContainer("pickup-manager");

      VisualElement managerButtons = manager.Q<VisualElement>("pickup-manager-buttons");

      Button addPickUpButton = new Button(() =>
      {
        EditorUtils.CreatePrefab("Assets/RFG/Items/Prefabs", "PickUp");
      })
      {
        name = "add-pickup-button",
        text = "Add PickUp"
      };

      managerButtons.Add(addPickUpButton);

      return manager;
    }

    private void GenerateItems(string createPath, string className)
    {

      Texture2D texture2D = Selection.activeObject as Texture2D;

      if (texture2D == null)
      {
        LogExt.Warn<ItemEditorWindow>("Selected object is not texture 2d");
        return;
      }

      GameObject pickups = GameObject.Find("PickUps");
      if (pickups == null)
      {
        pickups = new GameObject();
        pickups.name = "PickUps";
      }

      Camera cam = Camera.main;

      Vector3 p = cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight, 0));
      float originalX = p.x;
      p += new Vector3(1f, -1f, 0);
      float height = cam.orthographicSize;
      float width = cam.aspect * height;

      string spriteSheet = AssetDatabase.GetAssetPath(texture2D);
      Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheet).OfType<Sprite>().ToArray();
      foreach (Sprite sprite in sprites)
      {
        Item item = ScriptableObject.CreateInstance(className) as Item;
        item.Guid = System.Guid.NewGuid().ToString();
        item.PickUpSprite = sprite;
        item.Description = sprite.name;
        item.PickUpText = sprite.name;
        string path = $"{createPath}/{sprite.name}.asset";
        AssetDatabase.CreateAsset(item, path);
        AssetDatabase.SaveAssets();

        GameObject obj = new GameObject();
        obj.name = sprite.name;
        obj.transform.position = new Vector3(p.x, p.y, 0);
        PickUp pickup = obj.AddComponent<PickUp>();
        pickup.Item = item;
        pickup.GeneratePickup();
        pickup.transform.SetParent(pickups.transform);
        p.x += 1f;
        if (p.x >= width - 1f)
        {
          p.x = originalX + 1f;
          p.y -= 1f;
        }
      }
      AssetDatabase.Refresh();
      EditorUtility.FocusProjectWindow();
    }

  }
}