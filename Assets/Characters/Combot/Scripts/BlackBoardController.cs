using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


/* Pushes updates about this object to the BlackBoard
 *
 * In addition, exports collected data at the end of each round
 */
public class BlackBoardController : MonoBehaviour {

    /* Fields */

    // SkillTree handlers
    Dictionary<string, Func<MoveInfo, bool, bool, Modifier>> handlers = new Dictionary<string, Func<MoveInfo, bool, bool, Modifier>>();
    Basic basic;
    Strong strong;
    Evade evade;
    Grab grab;

    // Characters
    CharacterInfo p1, p2;

    // BlackBoard
    BlackBoard bb;


    // Happens when the object is enabled
    void OnEnable()
    {
        // Get the BlackBoard
        bb = GameObject.Find("BlackBoard").GetComponent<BlackBoard>();

        // Subscribe to UFE events
        UFE.OnGameBegin += OnGameBegin;
        UFE.OnRoundBegins += OnRoundBegins;
        UFE.OnInput += OnInput;
        UFE.OnMove += OnMove;
        UFE.OnHit += OnHit;
        UFE.OnRoundEnds += OnRoundEnds;
    }

    // Happens when the object is disabled
    void OnDisable()
    {
        // Unsubscribe from UFE events
        UFE.OnGameBegin -= OnGameBegin;
        UFE.OnRoundBegins -= OnRoundBegins;
        UFE.OnInput -= OnInput;
        UFE.OnMove -= OnMove;
        UFE.OnHit -= OnHit;
        UFE.OnRoundEnds -= OnRoundEnds;
    }


    // Use this for initialization
    void Start () {
        StartCoroutine(PostDataToServer.PostData());
        StartCoroutine(PostDataToServer.PostData(false));
    }

    // Initialize player information
    void OnGameBegin(CharacterInfo player1, CharacterInfo player2, StageOptions stage)
    {
        p1 = player1;
        p2 = player2;

        // Get all of the moves' SkillTree handlers
        basic = GetComponent<Basic>();
        basic.GetTree(p1, true);
        basic.GetTree(p2, false);
        handlers[Constants.BASIC] = basic.Resolve;

        strong = GetComponent<Strong>();
        strong.GetTree(p1, true);
        strong.GetTree(p2, false);
        handlers[Constants.STRONG] = strong.Resolve;

        evade = GetComponent<Evade>();
        evade.GetTree(p1, true);
        evade.GetTree(p2, false);
        handlers[Constants.EVADE] = evade.Resolve;

        grab = GetComponent<Grab>();
        grab.GetTree(p1, true);
        grab.GetTree(p2, false);
        handlers[Constants.GRAB] = evade.Resolve;
    }

    // Happens every round
    public void OnRoundBegins(int roundNumber) {
        // Reset the BlackBoard to clear out information from the previous round
        bb.ClearBlackBoard();

        // Add information about each player to Blackboard
        bb.Register(Constants.p1Key, new Dictionary<string, string>() {
            // Who am I?
            { Constants.playerName, "" },

            // Passives
            { Constants.indexLifePoints, p1.currentLifePoints.ToString() },
            { Constants.indexFavor, Constants.MIN_FAVOR.ToString() },
            { Constants.indexRally, Constants.MIN_RALLY.ToString() },
            { Constants.indexBalance, Constants.STARTING_BALANCE.ToString() },
            
            // Extra data for conditioning on moves
            { Constants.lastHitDamage, "0"},
            { Constants.lastAttackByPlayer, ""},
            { Constants.landedLastAttack, "" },
            { Constants.lastEvade, "" },
            { Constants.lastEvadeSuccessful, "" },
            { Constants.lastAttackByOpponent, "" },
            { Constants.opponentLandedLastAttack, "" },

            // Extra data for skill tree nodes

            // Surprise
            { Surprise.attackCount, "0" },
            { Surprise.evadeCount, "0" },

            // Match results
            { Constants.winner, "false" }
        });
        bb.Register(Constants.p2Key, new Dictionary<string, string>() {
            // Who am I?
            { Constants.playerName, "" },

            // Passives
            { Constants.indexLifePoints, p2.currentLifePoints.ToString() },
            { Constants.indexFavor, Constants.MIN_FAVOR.ToString() },
            { Constants.indexRally, Constants.MIN_RALLY.ToString() },
            { Constants.indexBalance, Constants.STARTING_BALANCE.ToString() },

            // Extra data for conditioning on moves
            { Constants.lastHitDamage, "0"},
            { Constants.lastAttackByPlayer, ""},
            { Constants.landedLastAttack, "" },
            { Constants.lastEvade, "" },
            { Constants.lastEvadeSuccessful, "" },
            { Constants.lastAttackByOpponent, "" },
            { Constants.opponentLandedLastAttack, "" },

            // Extra data for skill tree nodes

            // Surprise
            { Surprise.attackCount, "0" },
            { Surprise.evadeCount, "0" },

            // Match results
            { Constants.winner, "false" }
        });

        // Save BlackBoard state
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            bb.UpdateProperty(Constants.p1Key, Constants.playerName, GameObject.Find("Name").GetComponent<NameHolder>().username);
            bb.DumpBlackBoard(Constants.p2Key);
        }
        else
        {
            if (UFE.GetLocalPlayer() == 1)
            {
                bb.UpdateProperty(Constants.p1Key, Constants.playerName, GameObject.Find("Name").GetComponent<NameHolder>().username);
            }
            else
            {
                bb.UpdateProperty(Constants.p2Key, Constants.playerName, GameObject.Find("Name").GetComponent<NameHolder>().username);
            }
        }
    }

    // Update is called once per frame
    void Update () {
	    
	}


    /* UFE events
     */
    void OnInput(InputReferences[] inputReferences, int player)
    {
        foreach (InputReferences inRef in inputReferences)
        {
            StartCoroutine(InputLog(inRef.engineRelatedButton.ToString(), (player == 1 ? Constants.p1Key : Constants.p2Key)));
            Debug.Log(inRef.engineRelatedButton.ToString() + " by Player " + player);
        }
    }
    
    void OnMove(MoveInfo move, CharacterInfo player)
    {
        // Record the button that was pressed and the time it was pressed
        //Debug.Log("[" + Time.time + "] Player " + player.GetInstanceID() + " inputted " + (int)move.buttonExecution[0]);

        // Record move information
        if (player.GetInstanceID() == p1.GetInstanceID())
        {
            if (move.moveName != Constants.EVADE)
            {
                bb.UpdateProperty(Constants.p1Key, Constants.lastAttackByPlayer, move.moveName);
                bb.UpdateProperty(Constants.p2Key, Constants.lastAttackByOpponent, move.moveName);

                // This is an attack
                bb.UpdateProperty(Constants.p1Key, Surprise.attackCount, (int.Parse(bb.GetProperties(Constants.p1Key)[Surprise.attackCount]) + 1).ToString());

                // Wait to see if it missed
                AttackMissed(move, player);
            }
            else
            {
                bb.UpdateProperty(Constants.p1Key, Constants.lastEvade, Constants.TRUE);

                // This is an evade
                bb.UpdateProperty(Constants.p1Key, Surprise.evadeCount, (int.Parse(bb.GetProperties(Constants.p1Key)[Surprise.evadeCount]) + 1).ToString());
            }
        }
        else
        {
            if (move.moveName != Constants.EVADE)
            {
                bb.UpdateProperty(Constants.p2Key, Constants.lastAttackByPlayer, move.moveName);
                bb.UpdateProperty(Constants.p1Key, Constants.lastAttackByOpponent, move.moveName);

                // This is an attack
                bb.UpdateProperty(Constants.p2Key, Surprise.attackCount, (int.Parse(bb.GetProperties(Constants.p2Key)[Surprise.attackCount]) + 1).ToString());

                // Wait to see if it missed
                AttackMissed(move, player);
            }
            else
            {
                bb.UpdateProperty(Constants.p2Key, Constants.lastEvade, Constants.TRUE);

                // This is an evade
                bb.UpdateProperty(Constants.p2Key, Surprise.evadeCount, (int.Parse(bb.GetProperties(Constants.p2Key)[Surprise.evadeCount]) + 1).ToString());
            }
        }

        // Save the state of the BlackBoard
        //Debug.Log("Saved BlackBoard state");
    }

    void OnHit(HitBox strokeHitBox, MoveInfo move, CharacterInfo hitter)
    {
        // Stuff that happens regardless of ambient effects
        // Record the amount of damage done
        //Debug.Log("Hit damage: " + move.hits[0].damageOnHit);

        // Update BlackBoard with new life totals
        if (p1.GetInstanceID() == hitter.GetInstanceID())
        {
            bb.UpdateProperty(Constants.p2Key, Constants.indexLifePoints, p2.currentLifePoints.ToString()); // Hitter is p1 -> p2 got hit
            bb.UpdateProperty(Constants.p2Key, Constants.opponentLandedLastAttack, Constants.TRUE);
            bb.UpdateProperty(Constants.p1Key, Constants.landedLastAttack, Constants.TRUE);

            // Did the other player try to evade?
            if (bb.GetProperties(Constants.p2Key)[Constants.lastEvade] == Constants.TRUE)
            {
                bb.UpdateProperty(Constants.p2Key, Constants.lastEvadeSuccessful, Constants.FALSE);
                bb.UpdateProperty(Constants.p2Key, Constants.lastEvade, Constants.FALSE);
            }
        }
        else
        {
            bb.UpdateProperty(Constants.p1Key, Constants.indexLifePoints, p1.currentLifePoints.ToString()); // And vice versa
            bb.UpdateProperty(Constants.p1Key, Constants.opponentLandedLastAttack, Constants.TRUE);
            bb.UpdateProperty(Constants.p2Key, Constants.landedLastAttack, Constants.TRUE);

            // Did the other player try to evade?
            if (bb.GetProperties(Constants.p1Key)[Constants.lastEvade] == Constants.TRUE)
            {
                bb.UpdateProperty(Constants.p1Key, Constants.lastEvadeSuccessful, Constants.FALSE);
                bb.UpdateProperty(Constants.p1Key, Constants.lastEvade, Constants.FALSE);
            }
        }

        // Calculate passive effects
        Func<MoveInfo, bool, bool, Modifier> resolver;
        if (handlers.TryGetValue(WhatMoveIsThis(move.moveName), out resolver))
            resolver(move, hitter.GetInstanceID() == p1.GetInstanceID(), true);
    }


    /* Utilities
     */
    // At the end of each round
    void OnRoundEnds(CharacterInfo winner, CharacterInfo loser)
    {
        // Save the winner
        //Debug.Log(Constants.WhichPlayer(winner, p1) + " wins");
        bb.UpdateProperty(Constants.WhichPlayer(winner, p1), Constants.winner, "true");
    }

    // Which of the 4 types of move (Basic, Strong, Evade, Grab) is this move?
    public string WhatMoveIsThis(string moveName)
    {
        if (basic.RefersToThis(moveName))
            return Constants.BASIC;
        if (strong.RefersToThis(moveName))
            return Constants.STRONG;
        if (evade.RefersToThis(moveName))
            return Constants.EVADE;
        if (grab.RefersToThis(moveName))
            return Constants.GRAB;

        return "Invalid";
    }

    // Hit detector; sets hit to FALSE after .6 seconds
    IEnumerator AttackMissed(MoveInfo move, CharacterInfo player)
    {
        yield return new WaitForSeconds(0.6f);

        Dictionary<string, string> p1Properties = bb.GetProperties(Constants.p1Key);
        Dictionary<string, string> p2Properties = bb.GetProperties(Constants.p2Key);

        if (player.GetInstanceID() == p1.GetInstanceID())
        {
            // Did the attack land?
            if (p1Properties[Constants.landedLastAttack] != Constants.TRUE)
            {
                // Updates that happen regardless of ambient effects
                bb.UpdateProperty(Constants.p1Key, Constants.landedLastAttack, Constants.FALSE);

                // Did the other player evade?
                if (p2Properties[Constants.lastEvade] == Constants.TRUE)
                {
                    bb.UpdateProperty(Constants.p2Key, Constants.lastEvadeSuccessful, Constants.TRUE);
                    bb.UpdateProperty(Constants.p2Key, Constants.lastEvade, Constants.FALSE);
                }
                else
                {
                    bb.UpdateProperty(Constants.p2Key, Constants.opponentLandedLastAttack, Constants.TRUE);
                }
            }
        }
        else
        {
            // Did the attack land?
            if (p2Properties[Constants.landedLastAttack] != Constants.TRUE)
            {
                // Updates that happen regardless of ambient effects
                bb.UpdateProperty(Constants.p2Key, Constants.landedLastAttack, Constants.FALSE);

                // Did the other player evade?
                if (p1Properties[Constants.lastEvade] == Constants.TRUE)
                {
                    bb.UpdateProperty(Constants.p1Key, Constants.lastEvadeSuccessful, Constants.TRUE);
                    bb.UpdateProperty(Constants.p1Key, Constants.lastEvade, Constants.FALSE);
                }
                else
                {
                    bb.UpdateProperty(Constants.p2Key, Constants.opponentLandedLastAttack, Constants.FALSE);
                }
            }
        }

        yield return null;
    }

    // Input logger
    IEnumerator InputLog(string input, string player)
    {
        Dictionary<string, string> playerProperties = bb.GetProperties(player);

        // Record for the player who pressed the key
        KeyData data = new KeyData(UFE.GetTimer(), input, (playerProperties[Constants.playerName] == "" ? player : playerProperties[Constants.playerName]), null);
        string write_to = Constants.addLogUrl + data.AsUrlParams() + "&hash=" + data.Md5Sum(Constants.notSoSecretKey);

        // Enqueue for POSTing to server
        if (UFE.GetLocalPlayer() == 1)
            PostDataToServer.postQueueP1.Add(new WWW(write_to));
        else
            PostDataToServer.postQueueP2.Add(new WWW(write_to));
        yield return null;
    }
}