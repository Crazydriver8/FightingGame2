using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/* Defines both a move and its associated skill tree
 */
public class SkillTree : MonoBehaviour {
    /* Fields */

    // The root node
    SkillTreeNode root = null;

    // Which move is this skill tree for?
    public string move;

    // How much weight can the skill tree hold?
    public int maxWeight,
               currWeight;

    // Defines hitbox
    public Vector3 center;
    public Vector3 range;

    // Defines DPS
    public int speed,
               minRawDamage,
               maxRawDamage;

    // Modifier for numerical stats
    public struct Modifier
    {
        public Vector3 center;
        public Vector3 range;
        public int speed,
                   minRawDamage,
                   maxRawDamage;
    }
    private Modifier mods;

    // Special effects that apply in the order they are added
    // Move designer should add default effects on Awake()
    private List<string> effects = new List<string>();


    /* Constructing a skill tree
     */
    // Add a node to the tree
    public bool AddRoot(SkillTreeNode node)
    {
        if (root == null && node.parent == Constants.ROOT_NODE)
        {
            root = node;
            return true;
        }

        return false;
    }


    /* Changing moves using the skill tree
     */
    // Sets modifier
    public void SetMods(Modifier mod)
    {
        mods = mod;
    }

    // Add effect
    public void AddEffect(string effect)
    {
        effects.Add(effect);
    }
    
    // Remove effect
    public bool RemoveEffect(string effect)
    {
        return effects.Remove(effect);
    }
    
    // Resolve all skill tree nodes to produce a modified move
    public virtual void Resolve ()
    {
        List<SkillTreeNode> frontier = new List<SkillTreeNode>();

        // Starting from root, resolve all the tree nodes
        SkillTreeNode temp = root;
        while (temp != null)
        {
            // Add children to frontier
            frontier.AddRange(temp.GetChildren());

            // Add effects and modifiers
            root.Resolve(this);

            // Get the next child
            if (frontier.Count > 0)
            {
                temp = frontier[0];
                frontier.RemoveAt(0);
            }
            else
                temp = null;
        }
    }
}
