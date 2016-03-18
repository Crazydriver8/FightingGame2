using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillTreeNode : AmbientObject {
    /* Fields */
    // Parent node location
    public int parent = 0;

    // Skill trees have up to 4 possible branches for attaching children
    private SkillTreeNode[] children = new SkillTreeNode[4] { null, null, null, null }; // It's acceptable to put a reference to the parent node in this array too
    public bool[] childPossible = new bool[4] { false, true, true, true };

    // Depth of the node in the tree
    public int minDepth = -1,
               maxDepth = -1;
    private int depth = -1;

    // Which attack is this node bound to (which move is it modifying)?
    public string attack;


    /* Tree construction
     */
    // Get the depth
    public int GetDepth()
    {
        return depth;
    }
    
    // Add a child
    public bool AddChild(SkillTreeNode child, int direction)
    {
        // Set child node in the correct slot
        if (childPossible[direction] && direction != parent // Is it possible to place a child here in the first place?
            && children[direction] == null // Is there already a child here?
           )
        {
            children[direction] = child;
            
            // Set this node as the parent of the child
            // Reset children[] on failure
            switch(direction)
            {
                case (int)Constants.Branch.DOWN:
                    if (!child.SetParent(this, (int)Constants.Branch.UP))
                    {
                        children[direction] = null;
                        return false;
                    }
                    
                    break;

                case (int)Constants.Branch.LEFT:
                    if (!child.SetParent(this, (int)Constants.Branch.RIGHT))
                    {
                        children[direction] = null;
                        return false;
                    }

                    break;

                case (int)Constants.Branch.RIGHT:
                    if (!child.SetParent(this, (int)Constants.Branch.LEFT))
                    {
                        children[direction] = null;
                        return false;
                    }

                    break;

                default:
                    break;
            }
            
            // Success
            return true;
        }

        // Failure
        return false;
    }

    // Set the parent of this node
    public bool SetParent(SkillTreeNode parentNode, int direction)
    {
        if (direction > -1 && children[direction] == null)
        {
            children[direction] = parentNode;
            parent = direction;
            depth = parentNode.GetDepth() + 1;

            // Is the depth in the correct range?
            if ((minDepth > -1 && depth < minDepth) // Falls below a set minimum
                || (maxDepth > -1 && depth > maxDepth) // Exceeds a set maximum
               )
            {
                // Rewind
                children[direction] = null;
                parent = 0;
                depth = -1;

                return false;
            }

            // Success
            return true;
        }

        // Overall failure
        return false;
    }

    
    /* Apply the state to the player when PASSIVE and the player's move when ACTIVE
     */
    // Get the children of this node for traversal
    public List<SkillTreeNode> GetChildren()
    {
        List<SkillTreeNode> result = new List<SkillTreeNode>();

        // It is impossible for UP to contain a child
        for (int i = (int)Constants.Branch.RIGHT; i > 0; i--)
        {
            // Add children, ignoring the origin
            if (i != parent)
                result.Add(children[i]);
        }

        return result;
    }
    
    // Changes parameters of move
    public virtual void Resolve(SkillTree move)
    {
        
    }
}
