using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SimpleJSON;


public class DecisionTreeAI : MonoBehaviour {

    struct AIMoveInfo
    {
        string buttonName;
        List<Dictionary<string, Dictionary<string, string>>> rules; // A list of faux BlackBoards that stores conditions
        List<float> likelihoods; // The probability of this button being used corresponds to the index of the rule set

        int goodness;
        int ruleMet; // Index of the list of the rules that produced the highest goodness, in case probability analysis is needed


        public AIMoveInfo(string buttonName)
        {
            this.buttonName = buttonName;
            this.rules = new List<Dictionary<string, Dictionary<string, string>>>();

            // Store likelihoods[i] = p(this move | rules[i])
            this.likelihoods = new List<float>();

            this.goodness = 0;
            this.ruleMet = -1;
        }

        /* Insert a rule for using this move and its probability (assume 100% if not given)
         */
        public void AddRule(Dictionary<string, Dictionary<string, string>> rule, string probability = null)
        {
            // Parse the condition
            this.rules.Add(rule);
            if (probability == null)
            {
                this.likelihoods.Add(1.0f);
            }
            else
            {
                this.likelihoods.Add(float.Parse(probability.Substring(probability.IndexOf("=") + 1, probability.Length - (probability.IndexOf("=") + 1))));
            }
        }

        /* Chooses a course of action based on how many rules are satisfied
         */
        public void Deliberate(BlackBoard bb)
        {
            // Get the maximal match and save its index
            foreach (Dictionary<string, Dictionary<string, string>> rule in rules)
            {
                int numRulesMet = 0;

                foreach(KeyValuePair<string, string> r in rule[Constants.p1Key])
                {
                    if (bb.GetValue(Constants.p1Key, r.Key) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }

                foreach (KeyValuePair<string, string> r in rule[Constants.p2Key])
                {
                    if (bb.GetValue(Constants.p2Key, r.Key) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }

                if (numRulesMet > this.goodness)
                {
                    this.goodness = numRulesMet;
                    this.ruleMet = this.rules.IndexOf(rule);
                }
            }
        }

        /* Reset counts to get ready for next round of deliberation
         */
        public void Reset()
        {
            this.goodness = 0;
            this.ruleMet = -1;
        }
    }

    Dictionary<string, AIMoveInfo> possibleMoves = new Dictionary<string, AIMoveInfo>();


    // Creates an AIMoveInfo for each possible move, then populates each move with its conditions
    public void PopulateMoves(string aJSON)
    {
        // Make a move for each moveName
        foreach(string moveName in Constants.moveNames)
        {
            possibleMoves[moveName] = new AIMoveInfo(moveName);
        }

        // Populate
        _PopulateMoves(JSON.Parse(aJSON));
    }
    void _PopulateMoves(JSONNode tree, List<string> path = null)
    {
        List<string> rules = (path == null ? new List<string>() : path);
        string buttonName = null,
               probability = null;

        foreach (string key in tree.Keys)
        {
            if (key == "result")
            {
                // Button press is result
                buttonName = tree[key];

                // Process the list of rules
                Dictionary<string, Dictionary<string, string>> ruleDict = new Dictionary<string, Dictionary<string, string>>();
                foreach (string rule in rules)
                {
                    if (rule.Contains("probability"))
                    {
                        probability = rule;
                    }
                    else
                    {
                        // Parse the condition
                        string player = rule.Substring(0, Constants.keyLength),
                               index = rule.Substring(Constants.keyLength + 1, rule.IndexOf("=") - (Constants.keyLength + 1)),
                               value = rule.Substring(rule.IndexOf("=") + 1, rule.Length - (rule.IndexOf("=") + 1));

                        // Add as dictionary entry
                        ruleDict[player][index] = value;
                    }
                }

                this.possibleMoves[buttonName].AddRule(ruleDict, probability);
            }
            else
            {
                rules.Add(key);
                _PopulateMoves(tree[key], rules);
            }
        }
    }


    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
