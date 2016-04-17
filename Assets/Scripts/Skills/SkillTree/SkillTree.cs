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
    //public string p1Filename = "skilltree1.json";
    // Player2
    CharacterInfo p2;
    public SkillTreeStructure p2Root;
    //public string p2Filename = "skilltree2.json";


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
            //p1Root = new SkillTreeStructure("Ghost", RootSkill(), new SkillTreeStructure(), new SkillTreeStructure(), new SkillTreeStructure());
            //p1Root.Attach(new SkillTreeStructure("Surprise"), (int)Constants.Branch.DOWN);
            //p1Root.connections[(int)Constants.Branch.DOWN].Attach(new SkillTreeStructure("Applause"), (int)Constants.Branch.LEFT);
            NameHolder store = GameObject.Find("Name").GetComponent<NameHolder>();
            if (store.ExistsTree())
                p1Root = store.skillTree;
            else
                p1Root = Constants.BuildDefaultTree();
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
            //p2Root = new SkillTreeStructure("Ghost", RootSkill(), new SkillTreeStructure(), new SkillTreeStructure(), new SkillTreeStructure());
            //p2Root.Attach(new SkillTreeStructure("Surprise"), (int)Constants.Branch.DOWN);
            //p2Root.connections[(int)Constants.Branch.DOWN].Attach(new SkillTreeStructure("Applause"), (int)Constants.Branch.LEFT);
            NameHolder store = GameObject.Find("Name").GetComponent<NameHolder>();
            if (store.ExistsTree())
                p2Root = store.skillTree;
            else
                p2Root = Constants.BuildDefaultTree();
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
            Debug.Log(temp.name);
            Modifier modToAdd = nodeLoader.handlers[temp.name](this, ufeMove, p1UsedMove, passive);
            mod = mod.Combine(modToAdd);

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

    // Faux skill tree node; if a skill tree has this as its parent, it is the "root"
    public SkillTreeStructure RootSkill()
    {
        return new SkillTreeStructure("Root");
    }
}


/* What does a skill tree look like (bare-bones)? */
public struct SkillTreeStructure
{
    public string name; // Set this to an empty string to denote "null"
    public SkillTreeStructure[] connections;

    public int parent;

    /* Constructor */
    // General
    public SkillTreeStructure(string name, SkillTreeStructure up, SkillTreeStructure down, SkillTreeStructure left, SkillTreeStructure right, int parent = (int)Constants.Branch.UP)
    {
        this.name = name;

        // Branches
        this.connections = new SkillTreeStructure[4];
        connections[(int)Constants.Branch.UP] = up;
        connections[(int)Constants.Branch.DOWN] = down;
        connections[(int)Constants.Branch.LEFT] = left;
        connections[(int)Constants.Branch.RIGHT] = right;

        // Where a reference to the parent is stored
        this.parent = parent;
    }

    // Skill
    public SkillTreeStructure(string name, int parent = (int)Constants.Branch.UP)
    {
        this.name = name;

        // Branches
        this.connections = new SkillTreeStructure[4] { new SkillTreeStructure(), new SkillTreeStructure(), new SkillTreeStructure(), new SkillTreeStructure() };

        // Where a reference to the parent is stored
        this.parent = parent;
    }

    // From JSON
    public SkillTreeStructure FromJSON(string aJSON)
    {
        // Parse JSON using SimpleJSON library
        JSONNode N = JSON.Parse(aJSON);

        // Fill in default information
        this.name = "";
        this.connections = new SkillTreeStructure[4] { new SkillTreeStructure(), new SkillTreeStructure(), new SkillTreeStructure(), new SkillTreeStructure() };
        this.parent = -1;
        if (N == null || N.Keys == null)
        {
            Debug.Log("No skill tree found");
        }
        else
        {
            // Build the children
            foreach (string child in N.Keys)
            {
                switch (child)
                {
                    case "name":
                        this.name = N[child];
                        break;

                    case "left":
                        this.connections[(int)Constants.Branch.LEFT] = new SkillTreeStructure(N[child], (int)Constants.Branch.RIGHT);
                        this.connections[(int)Constants.Branch.LEFT].connections[(int)Constants.Branch.RIGHT] = this;
                        break;

                    case "right":
                        this.connections[(int)Constants.Branch.RIGHT] = new SkillTreeStructure(N[child], (int)Constants.Branch.LEFT);
                        this.connections[(int)Constants.Branch.RIGHT].connections[(int)Constants.Branch.LEFT] = this;
                        break;

                    case "down":
                        this.connections[(int)Constants.Branch.DOWN] = new SkillTreeStructure(N[child], (int)Constants.Branch.UP);
                        this.connections[(int)Constants.Branch.DOWN].connections[(int)Constants.Branch.UP] = this;
                        break;

                    default:
                        break;
                }

                //Debug.Log(child);
            }
        }

        Debug.Log(this.ToString());
        Debug.Log(this.ToString() == aJSON);

        return this;
    }
    // Helper for building children from JSONNodes
    public SkillTreeStructure(JSONNode node, int parent)
    {
        // Fill in default information
        this.name = "";
        this.connections = new SkillTreeStructure[4] { new SkillTreeStructure(), new SkillTreeStructure(), new SkillTreeStructure(), new SkillTreeStructure() };
        this.parent = parent;

        // Build the children
        foreach (string child in node.Keys)
        {
            switch (child)
            {
                case "name":
                    this.name = node[child];
                    break;

                case "left":
                    this.connections[(int)Constants.Branch.LEFT] = new SkillTreeStructure(node[child], (int)Constants.Branch.RIGHT);
                    this.connections[(int)Constants.Branch.LEFT].connections[(int)Constants.Branch.RIGHT] = this;
                    break;

                case "right":
                    this.connections[(int)Constants.Branch.RIGHT] = new SkillTreeStructure(node[child], (int)Constants.Branch.LEFT);
                    this.connections[(int)Constants.Branch.RIGHT].connections[(int)Constants.Branch.LEFT] = this;
                    break;

                case "down":
                    this.connections[(int)Constants.Branch.DOWN] = new SkillTreeStructure(node[child], (int)Constants.Branch.UP);
                    this.connections[(int)Constants.Branch.DOWN].connections[(int)Constants.Branch.UP] = this;
                    break;

                default:
                    break;
            }

            //Debug.Log(child);
        }
    }
    
    /* Builds a skill tree
     */
    public bool SetParent(SkillTreeStructure parent, int position)
    {
        if (connections != null)
        {
            // Change the parent if there isn't one set
            if (connections[position].IsNull())
            {
                connections[position] = parent;
                this.parent = position;
                return true;
            }
            else
                return false;
        }

        return false;
    }

    public bool Attach(SkillTreeStructure child, int position)
    {
        if (connections != null)
        {
            // Insert only if there's nothing in that position
            if (connections[position].IsNull() && this.parent != position)
            {
                switch (position)
                {
                    case (int)Constants.Branch.DOWN:
                        connections[position] = child;
                        connections[position].SetParent(this, (int)Constants.Branch.UP);
                        return true;

                    case (int)Constants.Branch.LEFT:
                        connections[position] = child;
                        connections[position].SetParent(this, (int)Constants.Branch.RIGHT);
                        return true;

                    case (int)Constants.Branch.RIGHT:
                        connections[position] = child;
                        connections[position].SetParent(this, (int)Constants.Branch.LEFT);
                        return true;

                    default:
                        return false;
                }
            }
            else
                return false;
        }

        return false;
    }


    /* Gets information about skill tree
     */
    public List<SkillTreeStructure> GetChildren()
    {
        List<SkillTreeStructure> children = new List<SkillTreeStructure>();

        // Resolution order for BFS is left, right, down
        if (connections != null)
        {
            // For LEFT, RIGHT, DOWN... add child to list if it exists and is not marked as the parent
            if (!connections[(int)Constants.Branch.LEFT].IsNull() && parent != (int)Constants.Branch.LEFT)
                children.Add(connections[(int)Constants.Branch.LEFT]);
            if (!connections[(int)Constants.Branch.RIGHT].IsNull() && parent != (int)Constants.Branch.RIGHT)
                children.Add(connections[(int)Constants.Branch.RIGHT]);
            if (!connections[(int)Constants.Branch.DOWN].IsNull() && parent != (int)Constants.Branch.DOWN)
                children.Add(connections[(int)Constants.Branch.DOWN]);
        }

        return children;
    }

    public bool IsNull()
    {
        return name == "" || name == null;
    }

    public bool IsRoot()
    {
        return parent == (int)Constants.Branch.UP && connections != null && connections[parent].name == "Root";
    }

    public override string ToString()
    {
        string str = "{";

        // Name
        str += "\"name\" : \"" + this.name + "\",";

        // Children (in resolution order)
        // Keep an empty string for no children
        if (this.connections != null)
        {

            str += "\"left\" : ";
            if (!this.connections[(int)Constants.Branch.LEFT].IsNull() && this.parent != (int)Constants.Branch.LEFT)
            {
                str += connections[(int)Constants.Branch.LEFT].ToString() + ",";
            }
            else
            {
                str += "\"\",";
            }

            str += "\"right\" : ";
            if (!this.connections[(int)Constants.Branch.RIGHT].IsNull() && this.parent != (int)Constants.Branch.RIGHT)
            {
                str += connections[(int)Constants.Branch.RIGHT].ToString() + ",";
            }
            else
            {
                str += "\"\",";
            }

            str += "\"down\" : ";
            if (!this.connections[(int)Constants.Branch.DOWN].IsNull() && this.parent != (int)Constants.Branch.DOWN)
            {
                str += connections[(int)Constants.Branch.DOWN].ToString();
            }
            else
            {
                str += "\"\"";
            }
        }

        return str + "}";
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
            (other.replaceWithMove != null && other.replaceWithMove != "" ? other.replaceWithMove : this.replaceWithMove),
            temp
        );
    }
}