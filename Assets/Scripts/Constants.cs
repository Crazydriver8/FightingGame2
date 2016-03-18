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


    /* Ambient effects */

    // Maximum values
    public static int MAX_FAVOR = 100;
}
