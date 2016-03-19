using UnityEngine;
using System.Collections;


/* A listing of publicly accessible constants, enums, etc.
 */
public static class Constants {

    /* Skill trees */
    // The possible branches where child nodes can go
    public enum Branch { UP, DOWN, LEFT, RIGHT };

    // A special parent value that marks a node as "root only"
    public static int ROOT_NODE = -1;

    // Directory where the skill tree data will be stored
    public static readonly string SKILL_TREE_DIR = Application.dataPath + "/PlayerData/";


    /* Ambient effects */

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
}
