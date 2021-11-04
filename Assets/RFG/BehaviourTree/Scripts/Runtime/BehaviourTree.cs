using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RFG.BehaviourTree
{
  [CreateAssetMenu(fileName = "New Behaviour Tree", menuName = "RFG/Behaviour Tree/Behaviour Tree")]
  public class BehaviourTree : ScriptableObject
  {
    public Node rootNode;
    public Node.State treeState = Node.State.Running;
    public List<Node> nodes = new List<Node>();
    public Blackboard blackboard = new Blackboard();

    public Node.State Update()
    {
      if (rootNode.state == Node.State.Running)
      {
        treeState = rootNode.Update();
      }
      return treeState;
    }

    public static List<Node> GetChildren(Node parent)
    {
      List<Node> children = new List<Node>();
      DecoratorNode decorator = parent as DecoratorNode;
      if (decorator && decorator.child != null)
      {
        children.Add(decorator.child);
      }

      RootNode root = parent as RootNode;
      if (root && root.child != null)
      {
        children.Add(root.child);
      }

      CompositeNode composite = parent as CompositeNode;
      if (composite)
      {
        return composite.children;
      }

      return children;
    }

    public static void Traverse(Node node, System.Action<Node> visiter)
    {
      if (node)
      {
        visiter?.Invoke(node);
        var children = GetChildren(node);
        children.ForEach((n) => Traverse(n, visiter));
      }
    }

    public BehaviourTree Clone()
    {
      BehaviourTree tree = Instantiate(this);
      tree.rootNode = tree.rootNode.Clone();
      tree.nodes = new List<Node>();
      Traverse(tree.rootNode, (n) =>
      {
        tree.nodes.Add(n);
      });
      return tree;
    }

    public void Bind(INodeContext context)
    {
      Traverse(rootNode, node =>
      {
        node.context = context;
      });
    }


#if UNITY_EDITOR
    public Node CreateNode(System.Type type)
    {
      Node node = ScriptableObject.CreateInstance(type) as Node;
      node.name = type.Name;
      node.guid = GUID.Generate().ToString();

      Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
      nodes.Add(node);

      if (!Application.isPlaying)
      {
        AssetDatabase.AddObjectToAsset(node, this);
      }
      Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");
      AssetDatabase.SaveAssets();

      return node;
    }

    public void DeleteNode(Node node)
    {
      Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");
      nodes.Remove(node);
      // AssetDatabase.RemoveObjectFromAsset(node);
      Undo.DestroyObjectImmediate(node);
      AssetDatabase.SaveAssets();
    }

    public void AddChild(Node parent, Node child)
    {
      DecoratorNode decorator = parent as DecoratorNode;
      if (decorator)
      {
        Undo.RecordObject(decorator, "Behaviour Tree (Add Child)");
        decorator.child = child;
        EditorUtility.SetDirty(decorator);
      }

      RootNode root = parent as RootNode;
      if (root)
      {
        Undo.RecordObject(root, "Behaviour Tree (Add Child)");
        root.child = child;
        EditorUtility.SetDirty(root);
      }

      CompositeNode composite = parent as CompositeNode;
      if (composite)
      {
        Undo.RecordObject(composite, "Behaviour Tree (Add Child)");
        composite.children.Add(child);
        EditorUtility.SetDirty(composite);
      }
    }

    public void RemoveChild(Node parent, Node child)
    {
      DecoratorNode decorator = parent as DecoratorNode;
      if (decorator)
      {
        Undo.RecordObject(decorator, "Behaviour Tree (Remove Child)");
        decorator.child = null;
        EditorUtility.SetDirty(decorator);
      }

      RootNode root = parent as RootNode;
      if (root)
      {
        Undo.RecordObject(root, "Behaviour Tree (Remove Child)");
        root.child = null;
        EditorUtility.SetDirty(root);
      }

      CompositeNode composite = parent as CompositeNode;
      if (composite)
      {
        Undo.RecordObject(composite, "Behaviour Tree (Remove Child)");
        composite.children.Remove(child);
        EditorUtility.SetDirty(composite);
      }
    }

#endif

  }
}