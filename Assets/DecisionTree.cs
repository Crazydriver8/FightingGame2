using UnityEngine;
using System.Collections;

public class DecisionTree {
    // Generates key without corresponding value
    public string GenerateKey(string player, string property)
    {
        return player + " " + property + "=";
    }

    // Generates values
    public string Hp_level(string hp)
    {
        float hpVal = float.Parse(hp);
        if (hpVal > 7500)
        {
            return "> 7500";
        } else if (hpVal > 5000)
        {
            return "> 5000";
        } else if (hpVal > 2500)
        {
            return "> 2500";
        } else
        {
            return "<= 2500";
        }
    }

    public string LastHitDamage(string last_hit)
    {
        float damage = float.Parse(last_hit);
        if (damage == 0)
        {
            return "0";
        } else if (damage <= 50)
        {
            return "<= 50";
        } else
        {
            return "> 50";
        }
    }

    public string Favor(string favor)
    {
        float favorLevel = float.Parse(favor);
        if (favorLevel == 0)
        {
            return "0";
        } else if (favorLevel <= 25)
        {
            return "<= 25";
        } else if (favorLevel <= 50)
        {
            return "<= 50";
        } else if (favorLevel <= 75)
        {
            return "<= 75";
        } else
        {
            return "<= 100";
        }
    }

    public string Rally(string rally)
    {
        float rallyLevel = float.Parse(rally);
        if (rallyLevel == 0)
        {
            return "0";
        } else if (rallyLevel <= 25)
        {
            return "<= 25";
        } else if (rallyLevel <= 50)
        {
            return "<= 50";
        } else if (rallyLevel <= 75)
        {
            return "<= 75";
        } else
        {
            return "<= 100";
        }
    }

    public string Balance(string balance)
    {
        float balanceLevel = float.Parse(balance);
        if (balanceLevel == 33)
        {
            return "33";
        } else if (balanceLevel < 33)
        {
            return "< 33";
        } else
        {
            return "> 33";
        }
    }

    public string[] attackEvade(string attackCount, string evadeCount)
    {
        float attackNum = float.Parse(attackCount);
        float evadeNum = float.Parse(evadeCount);
        float totalMoves = attackNum + evadeNum;
        if (totalMoves == 0)
        {
            return new string[2] { "about the same", "about the same" };
        } else if (Mathf.Abs((attackNum - evadeNum) / totalMoves) < 0.05) {
            return new string[2] { "about the same", "about the same" };
        } else if (attackNum > evadeNum)
        {
            return new string[2] { "more", "less" };
        } else
        {
            return new string[2] { "less", "more" };
        }
    }

    public string IsClose(string distance)
    {
        return (float.Parse(distance) < 3.5 ? "Close" : "Far");
    }
}
