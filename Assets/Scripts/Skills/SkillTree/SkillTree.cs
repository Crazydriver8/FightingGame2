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
    public string move; // Which move is this skill tree for
    public string[] moveUFE; // Just in case the move has other names in UFE
    public bool defensive = false; // Is this a defensive move?
    public int maxWeight,
               currWeight; // How much weight can the skill tree hold?
    
    // Special effects that apply in the order they are added
    // Move designer should add default effects on Awake()
    private List<string> effects = new List<string>();

    // Somewhere out there, there's a file that loaded all of the nodes
    public GameObject loader;
    public string loaderName = "SkillTreeNodes";
    private NodeLoader nodeLoader;

    // Information about the players and their changes to this move
    public CharacterInfo[] players = new CharacterInfo[2];
    public GameObject[] playerObjects = new GameObject[2];
    // Player1
    CharacterInfo p1;
    public SkillTreeStructure p1Root;
    public string p1Filename = "skilltree1.json";
    // Player2
    CharacterInfo p2;
    public SkillTreeStructure p2Root;
    public string p2Filename = "skilltree2.json";


    /* Use this on initialization
     */
    // Grabs the loader that contains all of the possible node information
    protected void InitLoader()
    {
        // Grab the node loader
        if (loader == null)
            loader = GameObject.Find(loaderName);
        nodeLoader = loader.GetComponent<NodeLoader>();
    }


    /* Constructing a skill tree
     */
    // Rebuild a skill tree given the file that it's stored in
    public void GetTree(CharacterInfo playerInfo, bool isP1)
    {
        if (isP1)
        {
            p1 = playerInfo;
            players[0] = p1;
            playerObjects[0] = GameObject.Find(Constants.p1Key);
            /*
            // Grab raw text
            string path = Constants.SKILL_TREE_DIR + p1Filename.Replace(".json", "");
            string content = Resources.Load<TextAsset>(path).text;

            // Parse as JSON
            var skillTree = JSON.Parse(content);
            */
            // Build the tree by converting the JSON to the summary struct

            // Default tree for demonstration purposes only
            p1Root = new SkillTreeStructure();
            p1Root.name = "Ghost";
            p1Root.connections = new SkillTreeStructure[4];
            p1Root.connections[(int)Constants.Branch.UP] = new SkillTreeStructure(); // Parent is null
            p1Root.connections[(int)Constants.Branch.LEFT] = new SkillTreeStructure();
            p1Root.connections[(int)Constants.Branch.RIGHT] = new SkillTreeStructure();
            p1Root.connections[(int)Constants.Branch.DOWN] = new SkillTreeStructure();
                p1Root.connections[(int)Constants.Branch.DOWN].name = "Surprise";
                p1Root.connections[(int)Constants.Branch.DOWN].connections = new SkillTreeStructure[4];
                p1Root.connections[(int)Constants.Branch.DOWN].connections[(int)Constants.Branch.UP] = p1Root; // Parent is null
                p1Root.connections[(int)Constants.Branch.DOWN].connections[(int)Constants.Branch.LEFT] = new SkillTreeStructure();
                    p1Root.connections[(int)Constants.Branch.DOWN].connections[(int)Constants.Branch.LEFT].name = "Applause";
                    p1Root.connections[(int)Constants.Branch.DOWN].connections[(int)Constants.Branch.LEFT].connections = null;
        }
        else
        {
            p2 = playerInfo;
            players[1] = p2;
            playerObjects[1] = GameObject.Find(Constants.p2Key);
            /*
            // Grab raw text
            string path = Constants.SKILL_TREE_DIR + p2Filename.Replace(".json", "");
            string content = Resources.Load<TextAsset>(path).text;

            // Parse as JSON
            var skillTree = JSON.Parse(content);
            */
            // Build the tree by converting the JSON to the summary struct

            // Default tree for demonstration purposes only
            p2Root = new SkillTreeStructure();
            p2Root.name = "Ghost";
            p2Root.connections = new SkillTreeStructure[4];
            p2Root.connections[(int)Constants.Branch.UP] = new SkillTreeStructure(); // Parent is null
            p2Root.connections[(int)Constants.Branch.LEFT] = new SkillTreeStructure();
            p2Root.connections[(int)Constants.Branch.RIGHT] = new SkillTreeStructure();
            p2Root.connections[(int)Constants.Branch.DOWN] = new SkillTreeStructure();
                p2Root.connections[(int)Constants.Branch.DOWN].name = "Surprise";
                p2Root.connections[(int)Constants.Branch.DOWN].connections = new SkillTreeStructure[4];
                p2Root.connections[(int)Constants.Branch.DOWN].connections[(int)Constants.Branch.UP] = p1Root; // Parent is null
                p2Root.connections[(int)Constants.Branch.DOWN].connections[(int)Constants.Branch.LEFT] = new SkillTreeStructure();
                    p2Root.connections[(int)Constants.Branch.DOWN].connections[(int)Constants.Branch.LEFT].name = "Applause";
                    p2Root.connections[(int)Constants.Branch.DOWN].connections[(int)Constants.Branch.LEFT].connections = null;
        }
    }


    /* Changing moves using the skill tree
     */
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
    public virtual Modifier Resolve (MoveInfo ufeMove, bool p1UsedMove, bool passive = true)
    {
        Modifier mod = new Modifier();
        List<SkillTreeStructure> frontier = new List<SkillTreeStructure>();

        if (p1UsedMove && !p1Root.IsNull())
            frontier.Add(p1Root);
        else if (!p1UsedMove && !p2Root.IsNull())
            frontier.Add(p2Root);

        // Starting from root, resolve all the tree nodes
        SkillTreeStructure temp;
        while (frontier.Count > 0)
        {
            // Get the next thing to process
            temp = frontier[0];
            
            // Add children to frontier
            frontier.AddRange(temp.GetChildren());

            // Add effects and modifiers
            Modifier modToAdd = nodeLoader.handlers[temp.name](this, ufeMove, p1UsedMove, passive);
            mod.Combine(modToAdd);

            // Remove the thing we are done with
            frontier.RemoveAt(0);
        }

        return mod;
    }


    /* Utilities
     */
    // Checks to see if a named move (from UFE) refers to this move
    public bool RefersToThis(string ufeName) {
        return move == ufeName || Array.IndexOf(moveUFE, ufeName) > -1;
    }
}


/* What does a skill tree look like (bare-bones)? */
public struct SkillTreeStructure
{
    public string name; // Set this to an empty string to denote "null"
    public SkillTreeStructure[] connections;

    public int parent;

    public List<SkillTreeStructure> GetChildren()
    {
        List<SkillTreeStructure> children = new List<SkillTreeStructure>();

        // Resolution order for BFS is left, right, down
        if (connections != null)
        {
            if (!connections[(int)Constants.Branch.LEFT].IsNull())
                children.Add(connections[(int)Constants.Branch.LEFT]);
            if (!connections[(int)Constants.Branch.RIGHT].IsNull())
                children.Add(connections[(int)Constants.Branch.RIGHT]);
            if (!connections[(int)Constants.Branch.DOWN].IsNull())
                children.Add(connections[(int)Constants.Branch.DOWN]);
        }

        return children;
    }

    public bool IsNull()
    {
        return name == "" || name == null;
    }
}


/* Parts of the skill tree that can be changed (selected fields from MoveInfo) */
public struct Modifier
{
    // The following 3 floats are delta values (changes in speed, minRawDamage, and maxRawDamage)
    public float speed,
                 minRawDamage,
                 maxRawDamage;

    // Special effects (stun, knockback, knockdown, invincible)
    public List<string> effects;

    // Complete overhaul of the move (replace with a new move)
    // Note that the newest replace is what happens
    public string replaceWithMove;


    /* Constructor */
    public Modifier(float s, float min, float max, string rwm, List<string> e = null)
    {
        this.speed = s;
        this.minRawDamage = min;
        this.maxRawDamage = max;

        if (e == null)
            this.effects = new List<string>();
        else
            this.effects = new List<string>(e);

        this.replaceWithMove = rwm;
    }

    /* Put two modifiers together */
    public Modifier Combine(Modifier other)
    {
        List<string> temp;
        if (this.effects != null && this.effects.Count > 0)
        {
            temp = new List<string>(this.effects);
            temp.AddRange(other.effects);
        }
        else
        {
            temp = new List<string>();
        }

        return new Modifier(
            this.speed + other.speed,
            this.minRawDamage + other.minRawDamage,
            this.maxRawDamage + other.maxRawDamage,
            other.replaceWithMove,
            temp
        );
    }
}