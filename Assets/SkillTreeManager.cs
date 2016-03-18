using UnityEngine;
using System.Collections;

public class SkillTreeManager : MonoBehaviour
{
    public static int skillpoints = 8;
    public static int remainingP = 8;

    public static int skill1 = 0;
    public static int skill1_2 = 0;
    public static int skill1_3 = 0;
    public static int skill1_4 = 0;

    public static int skill2 = 0;
    public static int skill2_2 = 0;
    public static int skill2_3 = 0;
    public static int skill2_4 = 0;
    public static int skill2_5 = 0;

    public static int skill3 = 0;
    public static int skill3_2 = 0;
    public static int skill3_3= 0;
   


    void start()
    {
        if ((gameObject.name == "T1S1") && (skillpoints > 0))
        {
            GetComponent<SpriteRenderer>().color = new Color(.8f, .8f, .8f);
        }

        if ((gameObject.name == "T2S1") && (skillpoints > 0))
        {
            GetComponent<SpriteRenderer>().color = new Color(.8f, .8f, .8f);
        }

        if ((gameObject.name == "T3S1") && (skillpoints > 0))
        {
            GetComponent<SpriteRenderer>().color = new Color(.8f, .8f, .8f);
        }

    }

    void Update()
    {
        if ((skill1 == 1) && (gameObject.name == "T1S2") && (skill1_2 < 1))
        {
            GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, .5f);
        }

        if ((skill1_2==1) && (skill1_3 < 1) && (gameObject.name == "T1S3"))
        {
            GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, .5f);
        }

        if ((skill1_3 == 1) && (skill1_4 < 1) && (gameObject.name == "T1S4"))
        {
            GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, .5f);
        }

        
        //2nd skill tree
        if ((skill2 == 1) && (gameObject.name == "T2S2") && (skill2_2 < 1))
        {
            GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, .5f);
        }

        if ((skill2_2 == 1) && (gameObject.name == "T2S3") && (skill2_3 < 1))
        {
            GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, .5f);
        }

        if ((skill2_2 == 1) && (gameObject.name == "T2S4") && (skill2_4 < 1))
        {
            GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, .5f);
        }

        if ((skill2_4 == 1) && (gameObject.name == "T2S5") && (skill2_5 < 1))
        {
            GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, .5f);
        }


        //3rd skill tree
        if ((skill3 == 1) && (gameObject.name == "T3S2") && (skill3_2 < 1))
        {
            GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, .5f);
        }

        if ((skill3_2 == 1) && (gameObject.name == "T3S3") && (skill3_3 < 1))
        {
            GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, .5f);
        }
    }

    void OnMouseDown()
    {
        // 1st Skill Tree

        if ((remainingP > 0) && (gameObject.name == "T1S1"))
        {
            skill1 += 1;
            remainingP -= 1;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }

        if ((remainingP > 0) && (skill1 == 1) && (gameObject.name == "T1S2"))
        {
            skill1_2 += 1;
            remainingP -= 1;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }

        if ((remainingP > 0) && (skill1_2 == 1) && (gameObject.name == "T1S3"))
        {
            skill1_3 += 1;
            remainingP -= 1;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }

        if ((remainingP > 0) && (skill1_3 == 1) && (gameObject.name == "T1S4"))
        {
            skill1_4 += 1;
            remainingP -= 1;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }

       
        
        //2nd skill tree

        if ((remainingP > 0) && (gameObject.name == "T2S1"))
        {
            skill2 += 1;
            remainingP -= 1;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }

        if ((remainingP > 0) && (skill2 == 1) && (gameObject.name == "T2S2"))
        {
            skill2_2 += 1;
            remainingP -= 1;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }

        if ((remainingP > 0) && (skill2_2 == 1) && (gameObject.name == "T2S3"))
        {
            skill2_3 += 1;
            remainingP -= 1;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }

        if ((remainingP > 0) && (skill2_2 == 1) && (gameObject.name == "T2S4"))
        {
            skill2_4 += 1;
            remainingP -= 1;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }

        if ((remainingP > 0) && (skill2_4 == 1) && (gameObject.name == "T2S5"))
        {
            skill2_5 += 1;
            remainingP -= 1;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }

        // 3rd skill tree

        if ((remainingP > 0) && (gameObject.name == "T3S1"))
        {
            skill3 += 1;
            remainingP -= 1;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }

        if ((remainingP > 0) && (skill3 == 1) && (gameObject.name == "T3S2"))
        {
            skill3_2 += 1;
            remainingP -= 1;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }

        if ((remainingP > 0) && (skill3_2 == 1) && (gameObject.name == "T3S3"))
        {
            skill3_3 += 1;
            remainingP -= 1;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }

    }

}