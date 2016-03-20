using UnityEngine;
using System.Collections;

public class Coach : SkillTreeNode
{

    /* Fields */
    // STRIPs that this ability uses
    Rally rallyScript;


    // Use this for initialization
    void Start()
    {
        rallyScript = GameObject.Find("STRIPs").GetComponent<Rally>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    /* What does this skill do?
     */
    // Only passively stacks rally
    public override Modifier Resolve(SkillTree move, MoveInfo ufeMove, bool p1UsedMove, bool passive)
    {
        Modifier mod = new Modifier();

        if (passive)
        {
            // As damage increases, stack more rally
            // Rally calculation: deltaR = (|p1.currentLifePoints - p2.currentLifePoints| / p1.lifePoints) * .25f * move.hits[0].damageOnHit
            float deltaR = 0.0f;
            if (p1UsedMove && move.players[0].currentLifePoints > move.players[1].currentLifePoints)
            {
                // P1 has a health advantage and just landed an attack
                // P2 rallies in response
                deltaR = (0.25f * ufeMove.hits[0].damageOnHit * Mathf.Abs(move.players[0].currentLifePoints - move.players[1].currentLifePoints)) / move.players[0].lifePoints;
                rallyScript.PassiveEffect(deltaR, Constants.p2Key);
            }
            else if (!p1UsedMove && move.players[1].currentLifePoints > move.players[0].currentLifePoints)
            {
                // P2 has a health advantage and just landed an attack
                // P1 rallies in response
                deltaR = (0.25f * ufeMove.hits[0].damageOnHit * Mathf.Abs(move.players[1].currentLifePoints - move.players[0].currentLifePoints)) / move.players[0].lifePoints;
                rallyScript.PassiveEffect(deltaR, Constants.p1Key);
            }
        }

        return mod;
    }
}
