using UnityEngine;
using System.Collections;


/* A listing of publicly accessible constants, enums, etc.
 */
public static class Constants {

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


    /* Ambient effects */

    // BlackBoard indices
    public static string indexLifePoints = "Current Life Points",
                         indexFavor = "Favor",
                         indexRally = "Rally",
                         indexBalance = "Balance";

    // Favor
    public static int MIN_FAVOR = 0;
    public static int MAX_FAVOR = 100;

    // Rally
    public static int MIN_RALLY = 0;
    public static int MAX_RALLY = 100;

    // Balance is a value between 0 and 100
    public static int MIN_BALANCE = 0;
    public static int STARTING_BALANCE = 50;
    public static int MAX_BALANCE = 100;


    /* Keeping track of players */
    public static string p1Key = "Player1",
                         p2Key = "Player2";

    // Gets the right key for a player
    public static string WhichPlayer(CharacterInfo player, CharacterInfo p1)
    {
        return (player.GetInstanceID() == p1.GetInstanceID() ? p1Key : p2Key);
    }
}
