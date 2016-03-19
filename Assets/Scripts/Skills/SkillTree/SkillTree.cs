using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using SimpleJSON;


/* Defines both a move and its associated skill tree
 */
public class SkillTree : MonoBehaviour {
    /* Fields */

    // Move properties
    SkillTreeStructure root; // The skill tree is represented as strings
    public string filename = "skilltree.json"; // Where the skill tree data is stored
    public string move; // Which move is this skill tree for
    public bool defensive = false; // Is this a defensive move?
    public int maxWeight,
               currWeight; // How much weight can the skill tree hold?

    // Looks up the node to resolve in all possible nodes
    Dictionary<string, Action<bool>> possibleNodes = new Dictionary<string, Action<bool>>();

    // Changes skills
    public int speed,
               minRawDamage,
               maxRawDamage; // Defines DPS
    public struct Modifier
    {
        public int speed,
                   minRawDamage,
                   maxRawDamage;
    }
    private Modifier mods; // Modifier for numerical stats

    // Special effects that apply in the order they are added
    // Move designer should add default effects on Awake()
    private List<string> effects = new List<string>();

    // Somewhere out there, there's a file that loaded all of the nodes
    public GameObject loader;
    private NodeLoader nodeLoader;


    // Use this on initialization
    void Start()
    {
        nodeLoader = loader.GetComponent<NodeLoader>();
    }


    /* Constructing a skill tree
     */
    // Rebuild a skill tree given the file that it's stored in
    void GetTree()
    {
        // Grab raw text
        string path = Constants.SKILL_TREE_DIR + filename.Replace(".json", "");
        string content = Resources.Load<TextAsset>(path).text;

        // Parse as JSON
        var skillTree = JSON.Parse(content);

        // Build the tree by converting the JSON to the summary struct
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
    public virtual void Resolve (bool passive = true)
    {
        /*List<string> frontier = new List<string>();
        frontier.Add(root.name);

        // Starting from root, resolve all the tree nodes
        SkillTreeStructure temp = root;
        while (frontier.Count > 0)
        {
            // Add children to frontier
            frontier.AddRange(temp.GetChildren());

            // Add effects and modifiers
            root.Resolve(this, passive);

            // Get the next child
            if (frontier.Count > 0)
            {
                temp = frontier[0];
                frontier.RemoveAt(0);
            }
            else
                temp = null;
        }*/
    }
}

public struct SkillTreeStructure
{
    public string name,
                  up,
                  left,
                  right,
                  down;

    public int parent;

    public List<string> GetChildren()
    {
        List<string> children = new List<string>();

        // Resolution order for BFS is left, right, down
        if (left != null)
            children.Add(left);
        if (right != null)
            children.Add(right);
        if (down != null)
            children.Add(down);

        return children;
    }
}