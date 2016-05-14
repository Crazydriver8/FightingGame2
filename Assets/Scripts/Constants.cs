using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/* A listing of publicly accessible constants, enums, etc.
 */
public static class Constants {
    /* Data logging */
    public const string notSoSecretKey = "TheCakeIsALie";
    public const string addLogUrl = "http://legacy.moonlit-spring.org/datalog/keyloggertext.php?";
    public const string getTreeUrl = "http://legacy.moonlit-spring.org/skilltrees/readtree.php?";
    public const string postTreeUrl = "http://legacy.moonlit-spring.org/skilltrees/writetree.php?";

    /* The basic move types */
    public const string BASIC = "Basic";
    public const string STRONG = "Strong";
    public const string EVADE = "Evade";
    public const string GRAB = "Grab";
    

    /* Skill trees */
    // The possible branches where child nodes can go
    public enum Branch { UP, DOWN, LEFT, RIGHT };

    // A special parent value that marks a node as "root only"
    public static int ROOT_NODE = -1;

    // Directory where the skill tree data will be stored
    public static readonly string SKILL_TREE_DIR = Application.dataPath + "/PlayerData/";

    // Default tree for demonstration purposes
    public static SkillTreeStructure defaultTree = new SkillTreeStructure();
    public static SkillTreeStructure BuildDefaultTree()
    {
        defaultTree = new SkillTreeStructure("Ghost", new SkillTreeStructure("Root"), new SkillTreeStructure(), new SkillTreeStructure(), new SkillTreeStructure());
        defaultTree.Attach(new SkillTreeStructure("Surprise"), (int)Constants.Branch.DOWN);
        defaultTree.connections[(int)Constants.Branch.DOWN].Attach(new SkillTreeStructure("Applause"), (int)Constants.Branch.LEFT);

        return defaultTree;
    }


    /* Ambient effects */

    // Name
    public static string playerName = "Player Name";
    
    // BlackBoard indices
    public static string indexLifePoints = "Current Life Points",
                         indexFavor = "Favor",
                         indexRally = "Rally",
                         indexBalance = "Balance";
    public static string lastHitDamage = "Last Hit", // float
                         lastAttackByPlayer = "Last Attack by Player", // string
                         landedLastAttack = "Landed Last Attack", // bool
                         lastEvade = "Last Evade", // bool
                         lastEvadeSuccessful = "Successful Evade", // bool
                         lastAttackByOpponent = "Last Attack by Opponent", // string
                         opponentLandedLastAttack = "Opponent Landed Last Attack"; // bool
    public static string distToOpponent = "Distance to Opponent"; // float
    public static string winner = "Winner"; // bool

    // Booleans
    public const string TRUE = "true",
                        FALSE = "false";

    // Favor
    public static int MIN_FAVOR = 0;
    public static int MAX_FAVOR = 100;

    // Rally
    public static int MIN_RALLY = 0;
    public static int MAX_RALLY = 100;

    // Balance is a value between 0 and 100
    public static int MIN_BALANCE = 0;
    public static int STARTING_BALANCE = 33;
    public static int MAX_BALANCE = 100;


    /* Move modifications */

    // Special effects on moves
    public const string STUN = "stun",
                        KNOCKBACK = "knockback",
                        KNOCKDOWN = "knockdown",
                        IFRAME = "invincible";


    /* Keeping track of players */
    public static string p1Key = "Player1",
                         p2Key = "Player2";
    public static int keyLength = 7;


    // Gets the right key for a player
    public static string WhichPlayer(CharacterInfo player, CharacterInfo p1)
    {
        return (player.GetInstanceID() == p1.GetInstanceID() ? p1Key : p2Key);
    }


    /* Player-emulating AI */
    public static string[] moveNames = { "Foward", "Back", "Up", "Down", "Button1", "Button2", "Button3", "Button4" };

    public static bool IsHorizontal(string move)
    {
        return move == "Foward" || move == "Back";
    }
    public static bool IsVertical(string move)
    {
        return move == "Up" || move == "Down";
    }

    public static ButtonPress ToButtonPress(string move)
    {
        switch(move)
        {
            case "Foward":
                return ButtonPress.Foward;

            case "Back":
                return ButtonPress.Back;

            case "Up":
                return ButtonPress.Up;

            case "Down":
                return ButtonPress.Down;

            case "Button1":
                return ButtonPress.Button1;

            case "Button2":
                return ButtonPress.Button2;
            
            case "Button3":
                return ButtonPress.Button3;
            
            case "Button4":
                return ButtonPress.Button4;
            
            default:
                return ButtonPress.Button5;
        }
    }

    public static string ToMove(ButtonPress b)
    {
        switch (b)
        {
            case ButtonPress.Foward:
                return "Foward";

            case ButtonPress.Back:
                return "Back";

            case ButtonPress.Up:
                return "Up";

            case ButtonPress.Down:
                return "Down";

            case ButtonPress.Button1:
                return "Button1";

            case ButtonPress.Button2:
                return "Button2";

            case ButtonPress.Button3:
                return "Button3";

            case ButtonPress.Button4:
                return "Button4";

            default:
                return "Button5";
        }
    }
}
