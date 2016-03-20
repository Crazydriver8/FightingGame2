using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SkillTreeNode : AmbientObject {
    /* Fields */
    // Parent node location
    public int parent = 0;

    // Skill trees have up to 4 possible branches for attaching children
    public bool[] childPossible = new bool[4] { false, true, true, true };

    // Depth of the node in the tree
    public int minDepth = -1,
               maxDepth = -1;

    // Which attack is this node bound to (which move is it modifying)?
    public string attack;

    // Is this node a defensive mod or an offensive mod?
    public bool defensive = false;

    // Give this node a name and description so that it can be easily identified
    public string skillName,
                  skillDescription;


    /* Resolve the effects that this node causes
     */
    // Changes BlackBoard on passive (passive also implies that Resolve() was called from OnHit())
    // Changes parameters of move on active
    public virtual Modifier Resolve(SkillTree move, MoveInfo ufeMove, bool p1UsedMove, bool passive)
    {
        return new Modifier();
    }
}
