using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SimpleJSON;
using System.IO;

public class DecisionTreeAI : MonoBehaviour {
    Dictionary<string, AIMoveInfo> possibleMoves = new Dictionary<string, AIMoveInfo>();
    public AITimingInfo timing;

    // Loads JSON file and populates dictionary based on contents
    public void LoadJSON(string path)
    {
        if (path.Length != 0)
        {
            //WWW w = new WWW("file:///" + path);
            TextAsset targetFile = Resources.Load<TextAsset>(path);
            string jsonInput;
            using (StreamReader r = new StreamReader(path))
            {
                jsonInput = r.ReadToEnd();
            }
            if (jsonInput == "" || jsonInput == null)
            {
                Debug.Log("No json found");
            }
            else
            {
                Debug.Log("JSON found : " + jsonInput);
                this.PopulateMoves(jsonInput);
                this.BestMove();
            }
            //Debug.Log(targetFile.text);

        }
    }
    // Creates an AIMoveInfo for each possible move, then populates each move with its conditions
    public void PopulateMoves(string aJSON)
    {
        // Make a move for each moveName
        foreach (string moveName in Constants.moveNames)
        {
            Debug.Log(moveName + " entered");
            possibleMoves[moveName] = new AIMoveInfo(moveName);
        }

        // Populate
        JSONNode data = JSON.Parse(aJSON);
        _PopulateMoves(data[0]);
        this.timing = new AITimingInfo(data[1]);
    }
    void _PopulateMoves(JSONNode tree, List<string> path = null)
    {
        List<string> rules = (path == null ? new List<string>() : path.ConvertAll(rule => rule));
        string buttonName = null,
               probability = null;

        foreach (string key in tree.Keys)
        {
            //Debug.Log(tree[key].GetType() == typeof(JSONArray));
            if (tree[key].GetType() == typeof(JSONArray))
            {
                // There are a bunch of results in here
                for(int i = 0; i < tree[key].Count; i++)
                {
                    // Button press is result
                    buttonName = tree[key][i];

                    if (buttonName == "Button5" || buttonName == "Button6")
                        continue;

                    // Make a copy of the rules
                    List<string> conditions = rules.ConvertAll(rule => rule);
                    if (key.Contains("probablilty"))
                    {
                        probability = key;
                    }
                    else
                    {
                        conditions.Add(key);
                    }

                    string debuglog = "if ";
                    // Process the list of rules
                    Dictionary<string, Dictionary<string, string>> ruleDict = new Dictionary<string, Dictionary<string, string>>();
                    ruleDict[Constants.p1Key] = new Dictionary<string, string>();
                    ruleDict[Constants.p2Key] = new Dictionary<string, string>();
                    foreach (string rule in conditions)
                    {
                        if (rule.Contains("probability"))
                        {
                            probability = rule;
                        }
                        else
                        {
                            // Parse the condition
                            if (rule.Contains("phase"))
                            {
                                string index = "phase",
                                       value = rule.Substring(6, rule.Length - 6);

                                ruleDict[Constants.p2Key][index] = value;
                                ruleDict[Constants.p2Key][index] = value;
                            }
                            else
                            {
                                string player = rule.Substring(0, Constants.keyLength),
                                       index = rule.Substring(Constants.keyLength + 1, rule.IndexOf("=") - (Constants.keyLength + 1)),
                                       value = rule.Substring(rule.IndexOf("=") + 1, rule.Length - (rule.IndexOf("=") + 1));

                                // Add as dictionary entry
                                ruleDict[player][index] = value;
                            }
                        }

                        debuglog += rule + ", ";
                    }

                    Debug.Log(debuglog + "then " + buttonName);
                    
                    // Associate rule with the move
                    this.possibleMoves[buttonName].AddRule(ruleDict, probability);

                    // Reset in preparation for next rule
                    buttonName = null;
                    probability = null;
                }
            }
            else
            {
                rules.Add(key);
                _PopulateMoves(tree[key], rules);

                // Reset for the next key
                rules = (path == null ? new List<string>() : path.ConvertAll(rule => rule));
            }
        }
    }

    public string Deliberate(BlackBoard bb)
    {
        //Debug.Log("Deliberate entered");
        foreach(KeyValuePair<string, AIMoveInfo> move in this.possibleMoves)
        {
            //Debug.Log("move found");
            move.Value.Deliberate(bb);
        }
        return this.BestMove();
    }

    /* Order by goodness
     */
    public string BestMove()
    {
        //Debug.Log("BestMove calculating...");
        float max = 0.0f;
        List<AIMoveInfo> results = new List<AIMoveInfo>();
        //Debug.Log("Possible moves " + this.possibleMoves.Count);
        foreach(KeyValuePair<string, AIMoveInfo> move in this.possibleMoves)
        {
            if (move.Value.goodness > max)
            {
                max = move.Value.goodness;
                results = new List<AIMoveInfo>() { move.Value };
            }
            else if (move.Value.goodness == max)
            {
                results.Add(move.Value);
            }
        }
        //Debug.Log("Found " + results.Count + " valid moves");

        // Tiebreaker using RNGsus
        return TieBreaker(results);
    }
    string TieBreaker(List<AIMoveInfo> moves)
    {
        //Debug.Log(moves.Count + " potential moves");
        if (moves.Count == 0)
            return null;

        if (moves.Count == 1)
            return moves[0].buttonName;

        // Get a random number and see where it falls on the distribution
        float distribution = 0.0f;
        float seed = Random.value;
        foreach (AIMoveInfo move in moves)
        {
            //Debug.Log(move.ToString());
            //Debug.Log(move.ruleMet);
            if (move.ruleMet >= 0) {
                distribution += move.likelihoods[move.ruleMet];
            }
            if(seed <= distribution)
            {
                //Debug.Log("Best move: " + move.buttonName);
                return move.buttonName;
            }
        }

        return null;
    }


    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}


public class AIMoveInfo
{
    public string buttonName;
    public List<Dictionary<string, Dictionary<string, string>>> rules; // A list of faux BlackBoards that stores conditions
    public List<float> likelihoods; // The probability of this button being used corresponds to the index of the rule set

    public int goodness;
    public int ruleMet; // Index of the list of the rules that produced the highest goodness, in case probability analysis is needed
    
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

            foreach (KeyValuePair<string, string> r in rule[Constants.p1Key])
            {
                if (r.Key == Constants.indexLifePoints)
                {
                    if (Hp_level(bb.GetValue(Constants.p1Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }
                else if (r.Key == Constants.indexFavor)
                {
                    if (Favor(bb.GetValue(Constants.p1Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }
                else if (r.Key == Constants.indexRally)
                {
                    if (Favor(bb.GetValue(Constants.p1Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }
                else if (r.Key == Constants.indexBalance)
                {
                    if (Favor(bb.GetValue(Constants.p1Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }
                else if (r.Key == Constants.lastHitDamage)
                {
                    if (Favor(bb.GetValue(Constants.p1Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }
                else if (r.Key == Constants.lastAttackByPlayer)
                {
                    if (Favor(bb.GetValue(Constants.p1Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }
                else if (r.Key == Constants.landedLastAttack)
                {
                    if (Favor(bb.GetValue(Constants.p1Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }
                else if (r.Key == Constants.lastEvade)
                {
                    if (Favor(bb.GetValue(Constants.p1Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }
                else if (r.Key == Constants.lastEvadeSuccessful)
                {
                    if (Favor(bb.GetValue(Constants.p1Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }
                else if (r.Key == Constants.lastAttackByOpponent)
                {
                    if (Favor(bb.GetValue(Constants.p1Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }
                else if (r.Key == Constants.opponentLandedLastAttack)
                {
                    if (Favor(bb.GetValue(Constants.p1Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }
                else if (r.Key == Constants.distToOpponent)
                {
                    if (Favor(bb.GetValue(Constants.p1Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }
                else if (r.Key == Constants.winner)
                {
                    if (Favor(bb.GetValue(Constants.p1Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }

                else
                {
                    string value = bb.GetValue(Constants.p1Key, r.Key);
                    if (value == r.Value || (value == "" && r.Value == "None"))
                    {
                        numRulesMet += 1;
                    }

                    break;
                }
            }
            
            foreach (KeyValuePair<string, string> r in rule[Constants.p2Key])
            {
                if (r.Key == Constants.indexLifePoints)
                {
                    if (Hp_level(bb.GetValue(Constants.p2Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }
                else if (r.Key == Constants.indexFavor)
                {
                    if (Favor(bb.GetValue(Constants.p2Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }
                else if (r.Key == Constants.indexRally)
                {
                    if (Favor(bb.GetValue(Constants.p2Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }
                else if (r.Key == Constants.indexBalance)
                {
                    if (Favor(bb.GetValue(Constants.p2Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }
                else if (r.Key == Constants.lastHitDamage)
                {
                    if (Favor(bb.GetValue(Constants.p2Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }
                else if (r.Key == Constants.lastAttackByPlayer)
                {
                    if (Favor(bb.GetValue(Constants.p2Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }
                else if (r.Key == Constants.landedLastAttack)
                {
                    if (Favor(bb.GetValue(Constants.p2Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }
                else if (r.Key == Constants.lastEvade)
                {
                    if (Favor(bb.GetValue(Constants.p2Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }
                else if (r.Key == Constants.lastEvadeSuccessful)
                {
                    if (Favor(bb.GetValue(Constants.p2Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }
                else if (r.Key == Constants.lastAttackByOpponent)
                {
                    if (Favor(bb.GetValue(Constants.p2Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }
                else if (r.Key == Constants.opponentLandedLastAttack)
                {
                    if (Favor(bb.GetValue(Constants.p2Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }
                else if (r.Key == Constants.distToOpponent)
                {
                    if (Favor(bb.GetValue(Constants.p2Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }
                else if (r.Key == Constants.winner)
                {
                    if (Favor(bb.GetValue(Constants.p2Key, r.Key)) == r.Value)
                    {
                        numRulesMet += 1;
                    }
                }

                else
                {
                    string value = bb.GetValue(Constants.p2Key, r.Key);
                    if (value == r.Value || (value == "" && r.Value == "None"))
                    {
                        numRulesMet += 1;
                    }

                    break;
                }
            }

            //Debug.Log("Rules met: " + numRulesMet);
            //Debug.Log("at " + this.rules.IndexOf(rule));
            //Debug.Log("Current Goodness: " + this.goodness);
            //Debug.Log(numRulesMet >= this.goodness);
            if (numRulesMet >= this.goodness)
            {
                this.goodness = numRulesMet;
                this.ruleMet = this.rules.IndexOf(rule);
                //Debug.Log(this.ToString());
            }
        }
    }

    /* Reset counts to get ready for next round of deliberation
     */
    public void ResetValues()
    {
        this.goodness = 0;
        this.ruleMet = -1;
    }

    /* Checking blackboard conditions
    */

    // Generates values
    public static string Hp_level(string hp)
    {
        if (hp == null)
        {
            return "0";
        }
        float hpVal = float.Parse(hp);
        if (hpVal > 7500)
        {
            return "> 7500";
        }
        else if (hpVal > 5000)
        {
            return "> 5000";
        }
        else if (hpVal > 2500)
        {
            return "> 2500";
        }
        else
        {
            return "<= 2500";
        }
    }

    public static string LastHitDamage(string last_hit)
    {
        if (last_hit == null)
        {
            return "0";
        }
        float damage = float.Parse(last_hit);
        if (damage == 0)
        {
            return "0";
        }
        else if (damage <= 50)
        {
            return "<= 50";
        }
        else
        {
            return "> 50";
        }
    }

    public static string Favor(string favor)
    {
        if (favor == null)
        {
            return "0";
        }
        float favorLevel = float.Parse(favor);
        if (favorLevel == 0)
        {
            return "0";
        }
        else if (favorLevel <= 25)
        {
            return "<= 25";
        }
        else if (favorLevel <= 50)
        {
            return "<= 50";
        }
        else if (favorLevel <= 75)
        {
            return "<= 75";
        }
        else
        {
            return "<= 100";
        }
    }

    public static string Rally(string rally)
    {
        if (rally == null)
        {
            return "0";
        }
        float rallyLevel = float.Parse(rally);
        if (rallyLevel == 0)
        {
            return "0";
        }
        else if (rallyLevel <= 25)
        {
            return "<= 25";
        }
        else if (rallyLevel <= 50)
        {
            return "<= 50";
        }
        else if (rallyLevel <= 75)
        {
            return "<= 75";
        }
        else
        {
            return "<= 100";
        }
    }

    public static string Balance(string balance)
    {
        if (balance == null)
        {
            return "33";
        }
        float balanceLevel = float.Parse(balance);
        if (balanceLevel == 33)
        {
            return "33";
        }
        else if (balanceLevel < 33)
        {
            return "< 33";
        }
        else
        {
            return "> 33";
        }
    }

    public static string[] attackEvade(string attackCount, string evadeCount)
    {
        if (attackCount == null)
        {
            //return "0";
            return new string[2] { "about the same", "about the same" };
        }
        if (evadeCount == null)
        {
            //return "0";
            return new string[2] { "about the same", "about the same" };
        }
        float attackNum = float.Parse(attackCount);
        float evadeNum = float.Parse(evadeCount);
        float totalMoves = attackNum + evadeNum;
        if (totalMoves == 0)
        {
            return new string[2] { "about the same", "about the same" };
        }
        else if (Mathf.Abs((attackNum - evadeNum) / totalMoves) < 0.05)
        {
            return new string[2] { "about the same", "about the same" };
        }
        else if (attackNum > evadeNum)
        {
            return new string[2] { "more", "less" };
        }
        else
        {
            return new string[2] { "less", "more" };
        }
    }

    public static string IsClose(string distance)
    {
        if (distance == null)
        {
            return "Close";
        }
        return (float.Parse(distance) < 3.5 ? "Close" : "Far");
    }


    public override string ToString()
    {
        return "{\"name\" : " + this.buttonName + ", \"goodness\" : " + this.goodness + ", \"index\" : " + this.ruleMet + "}";
    }
}

public class AITimingInfo
{
    Dictionary<string, List<float>> waitTime;

    public AITimingInfo(JSONNode timings)
    {
        this.waitTime = new Dictionary<string, List<float>>();

        foreach (string move in Constants.moveNames)
        {
            this.waitTime[move] = new List<float>() { timings[move]["interval"].AsFloat, timings[move]["std_dev"].AsFloat };
        }
    }


    public float ButtonHoldTime(string button)
    {
        if (button != "Button1" || button != "Button2" || button != "Button3" || button != "Button4")
        {
            List<float> waitData;
            if (waitTime.TryGetValue(button, out waitData))
            {
                // Calculate a random button hold time based on a Gaussian distribution
                float u1 = Random.Range(0.0f, 1.0f),
                      u2 = Random.Range(0.0f, 1.0f);
                float randStdNormal = Mathf.Sqrt(-2 * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2),
                      randNormal = waitData[0] + waitData[1] * randStdNormal;

                return randNormal;
                //return 10.0f;
            }
        }

        return 0.0f;
    }

    public float ButtonWaitTime(string button)
    {
        if (button != "Foward" || button != "Backward" || button != "Up" || button != "Down")
        {
            List<float> waitData;
            if (waitTime.TryGetValue(button, out waitData))
            {
                // Calculate a random button wait time based on a Gaussian distribution
                float u1 = Random.Range(0.0f, 1.0f),
                      u2 = Random.Range(0.0f, 1.0f);
                float randStdNormal = Mathf.Sqrt(-2 * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2),
                      randNormal = waitData[0] + waitData[1] * randStdNormal;

                return randNormal;
            }
        }

        return 0.0f;
    }
}