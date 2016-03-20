using UnityEngine;
using System.Collections;

public class Applause : SkillTreeNode
{
    /* Fields */
    // STRIPs that this ability uses
    Favor favorScript;

    // Time until next use
    float lastUsed = 0.0f;
    public float cooldown = 2.0f;


    // Use this for initialization
    void Start()
    {
        favorScript = GameObject.Find("STRIPs").GetComponent<Favor>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    /* What does this skill do?
     */
    // Only a passive: 
    public override Modifier Resolve(SkillTree move, MoveInfo ufeMove, bool p1UsedMove, bool passive)
    {
        Modifier mod = new Modifier();

        if (passive)
        {
            // As damage increases, stack more favor
            // Favor calculation: deltaF = .25f * move.hits[0].damageOnHit
            float deltaF = 0.25f * ufeMove.hits[0].damageOnHit;
            if (Time.time > lastUsed)
            {
                if (p1UsedMove)
                {
                    favorScript.PassiveEffect(deltaF, Constants.p1Key);
                }
                else
                {
                    favorScript.PassiveEffect(deltaF, Constants.p2Key);
                }

                lastUsed = Time.time + cooldown;
            }
        }

        return mod;
    }
}
