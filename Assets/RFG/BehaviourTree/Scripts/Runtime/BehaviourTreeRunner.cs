using UnityEngine;

namespace RFG.BehaviourTree
{
  [AddComponentMenu("RFG/Behaviour Tree/Behaviour Tree Runner")]
  public class BehaviourTreeRunner : MonoBehaviour
  {
    public BehaviourTree tree;

    [HideInInspector]
    private INodeContext _context;

    private void Awake()
    {
      _context = GetComponent(typeof(INodeContext)) as INodeContext;
    }

    private void Start()
    {
      tree = tree.Clone();
      tree.Bind(_context);
    }

    private void Update()
    {
      if (tree)
      {
        tree.Update();
      }
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