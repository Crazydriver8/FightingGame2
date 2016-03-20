using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/* Adds an effect based on repetition
 * Increases the damage of the next attack after many dodges
 */
public class Surprise : SkillTreeNode
{
    /* Fields */
    // STRIPs that this ability uses
    Balance balanceScript;

    // Augmented attack triggers
    public int maxAttacksInARow = 5;
    public float baseDeltaBalance = 5.0f;
    public float maxBalance = 5.0f;
    public float resetBalanceAt = 75.0f; // When balance exceeds this value, reset on evade

    // Augmented attack properties
    public float extraDamage = 15.0f;

    // Extra BlackBoard stuff
    public static string attackCount = "[Surprise] Number of Attacks",
                         evadeCount = "[Surprise] Number of Evades";


    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    /* What does this skill do?
     * 
     * Passive effect: Increase balance on attack, decrease balance on evade
     * Actice effect:
     *      Everything: Reset balance
     *      Everything else: +15 damage 
     */
    public override Modifier Resolve(SkillTree move, MoveInfo ufeMove, bool p1UsedMove, bool passive)
    {
        Modifier mod = new Modifier();

        if (passive)
        {
            // Scale balance using a sigmoid
            if (p1UsedMove)
            {
                Dictionary<string, string> atLeast = new Dictionary<string, string>() { { Constants.indexBalance, Constants.STARTING_BALANCE.ToString() } };

                // Scale upwards when at least STARTING_BALANCE
                if (balanceScript.VerifyActiveEffect(atLeast, null, null, Constants.p1Key))
                    balanceScript.PassiveEffect(Constants.STARTING_BALANCE + float.Parse(balanceScript.CurrentValue(Constants.p1Key, Constants.indexBalance)) * (Constants.MAX_BALANCE - Constants.STARTING_BALANCE) / Constants.MAX_BALANCE, Constants.p1Key, set: true);
                // Scale downwards when at most STARTING_BALANCE
                else
                    balanceScript.PassiveEffect(Constants.MIN_BALANCE + float.Parse(balanceScript.CurrentValue(Constants.p1Key, Constants.indexBalance)) * (Constants.STARTING_BALANCE - Constants.MIN_BALANCE) / Constants.MAX_BALANCE, Constants.p1Key, set: true);
            }
            else
            {
                Dictionary<string, string> atLeast = new Dictionary<string, string>() { { Constants.indexBalance, Constants.STARTING_BALANCE.ToString() } };

                // Scale upwards when at least STARTING_BALANCE
                if (balanceScript.VerifyActiveEffect(atLeast, null, null, Constants.p2Key))
                    balanceScript.PassiveEffect(Constants.STARTING_BALANCE + float.Parse(balanceScript.CurrentValue(Constants.p2Key, Constants.indexBalance)) * (Constants.MAX_BALANCE - Constants.STARTING_BALANCE) / Constants.MAX_BALANCE, Constants.p2Key, set: true);
                // Scale downwards when at most STARTING_BALANCE
                else
                    balanceScript.PassiveEffect(Constants.MIN_BALANCE + float.Parse(balanceScript.CurrentValue(Constants.p2Key, Constants.indexBalance)) * (Constants.STARTING_BALANCE - Constants.MIN_BALANCE) / Constants.MAX_BALANCE, Constants.p2Key, set: true);
            }
        }
        else
        {
            Dictionary<string, string> atMost = new Dictionary<string, string>() { { Constants.indexBalance, maxBalance.ToString() } },
                                       atLeast = new Dictionary<string, string>() { { Constants.indexBalance, resetBalanceAt.ToString() } };

            // Do something depending on the move that called it
            switch (move.move)
            {
                case Constants.BASIC:
                case Constants.STRONG:
                case Constants.GRAB:
                    if (p1UsedMove)
                    {
                        if (balanceScript.VerifyActiveEffect(null, atMost, null, Constants.p1Key))
                        {
                            mod = new Modifier(0, extraDamage, 0, "");

                            // Reset stacks
                            balanceScript.PassiveEffect(0.0f, Constants.p1Key, index: attackCount, set: true);
                            balanceScript.PassiveEffect(0.0f, Constants.p1Key, index: evadeCount, set: true);
                        }
                    }
                    else
                    {
                        if (balanceScript.VerifyActiveEffect(null, atMost, null, Constants.p2Key))
                        {
                            mod = new Modifier(0, extraDamage, 0, "");

                            // Reset stacks
                            balanceScript.PassiveEffect(0.0f, Constants.p2Key, index: attackCount, set: true);
                            balanceScript.PassiveEffect(0.0f, Constants.p2Key, index: evadeCount, set: true);
                        }
                    }
                    
                    break;

                case Constants.EVADE:
                    // On evade, calculate whether or not values should be reset
                    if (p1UsedMove)
                    {
                        if (balanceScript.VerifyActiveEffect(atLeast, null, null, Constants.p1Key))
                        {
                            balanceScript.PassiveEffect(Constants.STARTING_BALANCE, Constants.p1Key, set: true);
                            balanceScript.PassiveEffect(0.0f, Constants.p1Key, index: attackCount, set: true);
                            balanceScript.PassiveEffect(0.0f, Constants.p1Key, index: evadeCount, set: true);
                        }
                    }
                    else
                    {
                        if (balanceScript.VerifyActiveEffect(atLeast, null, null, Constants.p2Key))
                        {
                            balanceScript.PassiveEffect(Constants.STARTING_BALANCE, Constants.p2Key, set: true);
                            balanceScript.PassiveEffect(0.0f, Constants.p2Key, index: attackCount, set: true);
                            balanceScript.PassiveEffect(0.0f, Constants.p2Key, index: evadeCount, set: true);
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        return mod;
    }
}
