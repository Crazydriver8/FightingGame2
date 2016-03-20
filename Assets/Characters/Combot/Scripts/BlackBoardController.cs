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
        UFE.OnMove += OnMove;
        UFE.OnHit += OnHit;
        UFE.OnRoundEnds += OnRoundEnds;
    }

    // Happens when the object is disabled
    void OnDisable()
    {
        // Unsubscribe from UFE events
        UFE.OnGameBegin -= OnGameBegin;
        UFE.OnMove -= OnMove;
        UFE.OnHit -= OnHit;
        UFE.OnRoundEnds -= OnRoundEnds;
    }


    // Use this for initialization
    void Start () {

    }

    // Initialize player information
    void OnGameBegin(CharacterInfo player1, CharacterInfo player2, StageOptions stage)
    {
        p1 = player1;
        p2 = player2;

        // Get all of the moves' SkillTree handlers
        Evade evade = GetComponent<Evade>();
        evade.GetTree(p1, true);
        evade.GetTree(p2, false);
        handlers["Evade"] = evade.Resolve;

        // Add information about each player to Blackboard
        bb.Register(Constants.p1Key, new Dictionary<string, string>() {
            { Constants.indexLifePoints, p1.currentLifePoints.ToString() },
            { Constants.indexFavor, Constants.MIN_FAVOR.ToString() },
            { Constants.indexRally, Constants.MIN_RALLY.ToString() },
            { Constants.indexBalance, Constants.STARTING_BALANCE.ToString() }
        });
        bb.Register(Constants.p2Key, new Dictionary<string, string>() {
            { Constants.indexLifePoints, p2.currentLifePoints.ToString() },
            { Constants.indexFavor, Constants.MIN_FAVOR.ToString() },
            { Constants.indexRally, Constants.MIN_RALLY.ToString() },
            { Constants.indexBalance, Constants.STARTING_BALANCE.ToString() }
        });
    }

    // Update is called once per frame
    void Update () {
	    
	}


    /* UFE events
     */
    void OnMove(MoveInfo move, CharacterInfo player)
    {
        // Record the button that was pressed and the time it was pressed
        Debug.Log("[" + Time.time + "] Player " + player.GetInstanceID() + " inputted " + (int)move.buttonExecution[0]);

        // Save the state of the BlackBoard
        Debug.Log("Saved BlackBoard state");
    }

    void OnHit(HitBox strokeHitBox, MoveInfo move, CharacterInfo hitter)
    {
        // Calculate passive effects
        Func<MoveInfo, bool, bool, Modifier> resolver;
        if (handlers.TryGetValue(move.moveName, out resolver))
            resolver(move, hitter.GetInstanceID() == p1.GetInstanceID(), true);

        // Record the amount of damage done
        Debug.Log("Hit damage: " + move.hits[0].damageOnHit);

        // Update BlackBoard with new life totals
        if (p1.GetInstanceID() == hitter.GetInstanceID())
            bb.UpdateProperty(Constants.p2Key, "Current Life Points", p2.currentLifePoints.ToString()); // Hitter is p1 -> p2 got hit
        else
            bb.UpdateProperty(Constants.p1Key, "Current Life Points", p1.currentLifePoints.ToString()); // And vice versa
    }


    /* Utilities
     */
    // At the end of each round
    void OnRoundEnds(CharacterInfo winner, CharacterInfo loser)
    {
        // Save the winner
        Debug.Log(Constants.WhichPlayer(winner, p1) + " wins");
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
}