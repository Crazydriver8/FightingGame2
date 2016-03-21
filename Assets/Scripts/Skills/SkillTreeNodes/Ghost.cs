using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/* Adds invincibility frames in a pinch
 */
public class Ghost : SkillTreeNode
{
    /* Fields */
    // STRIPs that this ability uses
    Rally rallyScript;

    // Activation critieria
    public float triggerDist = 5.0f;

    // Penalties on use (decrement rally)
    // Also describes the base values needed to activate
    public float basicPenalty = 5.0f,
                 strongPenalty = 5.0f,
                 evadePenalty = 10.0f,
                 grabPenalty = 5.0f;


    // Use this for initialization
    void Start () {
        rallyScript = GameObject.Find("STRIPs").GetComponent<Rally>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    /* What does this skill do?
     * 
     * Passive effect: Stack rally
     * Actice effect:
     *      Evade: Move through and behind opponent
     *      Everything else: iframes during move
     */
    public override Modifier Resolve(SkillTree move, MoveInfo ufeMove, bool p1UsedMove, bool passive)
    {
        Modifier mod = new Modifier();

        if (passive)
        {
            // As damage increases, stack more rally
            // Rally calculation: deltaR = (|p1.currentLifePoints - p2.currentLifePoints| / p1.lifePoints) * .5f * move.hits[0].damageOnHit
            float deltaR = 0.0f;
            if (p1UsedMove && move.players[0].currentLifePoints > move.players[1].currentLifePoints)
            {
                // P1 has a health advantage and just landed an attack
                // P2 rallies in response
                deltaR = (0.5f * ufeMove.hits[0].damageOnHit * Mathf.Abs(move.players[0].currentLifePoints - move.players[1].currentLifePoints)) / move.players[0].lifePoints;
                rallyScript.PassiveEffect(deltaR, Constants.p2Key);
            }
            else if (!p1UsedMove && move.players[1].currentLifePoints > move.players[0].currentLifePoints)
            {
                // P2 has a health advantage and just landed an attack
                // P1 rallies in response
                deltaR = (0.5f * ufeMove.hits[0].damageOnHit * Mathf.Abs(move.players[1].currentLifePoints - move.players[0].currentLifePoints)) / move.players[0].lifePoints;
                rallyScript.PassiveEffect(deltaR, Constants.p1Key);
            }
        }
        else
        {
            // Do something depending on the move that called it
            switch(move.move)
            {
                case Constants.BASIC:
                case Constants.STRONG:
                case Constants.GRAB:
                    break;

                case Constants.EVADE:
                    Dictionary<string, string> atLeast = new Dictionary<string, string>() { { Constants.indexRally, evadePenalty.ToString() } };

                    if (Vector3.Distance(UFE.GetPlayer1ControlsScript().transform.position, UFE.GetPlayer2ControlsScript().transform.position) <= triggerDist)
                    {
                        if (p1UsedMove && UFE.GetPlayer1().currentLifePoints < UFE.GetPlayer2().currentLifePoints)
                        {
                            if (rallyScript.VerifyActiveEffect(atLeast, null, null, Constants.p1Key))
                            {
                                mod = new Modifier(0, 0, 0, "Maneuver");

                                // Update BlackBoard with penalty
                                rallyScript.PassiveEffect(-1.0f * evadePenalty, (p1UsedMove ? Constants.p1Key : Constants.p2Key));
                            }
                        }
                        else if (!p1UsedMove && UFE.GetPlayer2().currentLifePoints < UFE.GetPlayer1().currentLifePoints)
                        {
                            if (rallyScript.VerifyActiveEffect(atLeast, null, null, Constants.p2Key))
                            {
                                mod = new Modifier(0, 0, 0, "Maneuver");

                                // Update BlackBoard with penalty
                                rallyScript.PassiveEffect(-1.0f * evadePenalty, Constants.p2Key);
                            }
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
