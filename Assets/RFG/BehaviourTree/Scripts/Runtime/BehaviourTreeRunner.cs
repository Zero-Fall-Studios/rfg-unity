using UnityEngine;

namespace RFG.BehaviourTree
{
  [AddComponentMenu("RFG/Behaviour Tree/Behaviour Tree Runner")]
  public class BehaviourTreeRunner : MonoBehaviour
  {
    public BehaviourTree tree;

    private INodeContext _context;
    private BehaviourTree _defaultTree;

    private void Awake()
    {
      _context = GetComponent(typeof(INodeContext)) as INodeContext;
    }

    private void Start()
    {
      _defaultTree = tree;
      ChangeTree(tree);
    }

    private void Update()
    {
      if (tree != null)
      {
        tree.Update();
      }
    }

    public void ChangeTree(BehaviourTree newTree)
    {
      tree = newTree;
      tree = tree.Clone();
      tree.Bind(_context);
    }

    public void ChangeToDefaultTree()
    {
      ChangeTree(_defaultTree);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
      if (!tree)
      {
        return;
      }

      BehaviourTree.Traverse(tree.rootNode, (n) =>
      {
        if (n.drawGizmos)
        {
          n.OnDrawGizmos();
        }
      });
    }

#endif
  }
}