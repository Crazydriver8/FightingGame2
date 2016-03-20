using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class NodeLoader : MonoBehaviour {
    public Dictionary<string, Func<SkillTree, MoveInfo, bool, bool, Modifier>> handlers = new Dictionary<string, Func<SkillTree, MoveInfo, bool, bool, Modifier>>();


    // Use this for initialization
    void Start () {
        // For every node, make sure that it gets loaded in here!
        // All of the nodes should be attached to the same object that this script is attached to

        // Passive only
        GolfClap golfClap = GetComponent<GolfClap>();
        handlers[golfClap.name] = golfClap.Resolve;
        Applause applause = GetComponent<Applause>();
        handlers[applause.name] = applause.Resolve;
        Coach coach = GetComponent<Coach>();
        handlers[coach.name] = coach.Resolve;


        // Has an active
        Ghost ghost = GetComponent<Ghost>();
        handlers[ghost.name] = ghost.Resolve;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
