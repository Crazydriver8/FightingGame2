using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


/* Make a bunch of these to keep all parts of the graph organized
 */
public class GraphPortion
{
	public float start = 0.0f,
				 end = 0.0f;
	
	public GraphPortion(float start, float end)
	{
		this.start = start;
		this.end = end;
	}
	
	public void Destroy()
	{
		
	}
}


/* Does the heavy-lifting in calculating the distribution and making the graph, portion by portion
 */
public class Distribution : MonoBehaviour {
	Dictionary<string, int> moveFreq;
	int totalInputs = 0;
	bool resetEveryRound;
	public float graphLength = -1;

    private GameObject upBar;
    private GameObject downBar;
    private GameObject fowardBar;
    private GameObject backwardBar;
    private GameObject but1Bar;
    private GameObject but2Bar;
    private GameObject but3Bar;
    private GameObject but4Bar;

    private RectTransform upRect;
    private RectTransform downRect;
    private RectTransform fowardRect;
    private RectTransform backwardRect;
    private RectTransform but1Rect;
    private RectTransform but2Rect;
    private RectTransform but3Rect;
    private RectTransform but4Rect;

    private GameObject upText;
    private GameObject downText;
    private GameObject fowardText;
    private GameObject backwardText;
    private GameObject but1Text;
    private GameObject but2Text;
    private GameObject but3Text;
    private GameObject but4Text;

    List<GraphPortion> portions = new List<GraphPortion> ();
	
	NameHolder nh;
	
	
	void Start()
	{
        
		nh = GameObject.Find("Name").GetComponent<NameHolder>();
		resetEveryRound = false;
		
		// Default size of distribution graph
		if (graphLength == -1)
			graphLength = 1000;

        this.moveFreq = new Dictionary<string, int>() {
                {"Foward", 0},
                {"Backward", 0},
                {"Up", 0},
                {"Down", 0},
                {"Button1", 0},
                {"Button2", 0},
                {"Button3", 0},
                {"Button4", 0}
            };

        this.totalInputs = 0;
    }
	
	
	/* Basic logging
	 */
	// A move log can (and should) only increment
	public bool Increment(string move)
	{
		int val;
		if (this.moveFreq.TryGetValue(move, out val))
		{
			this.moveFreq[move] += 1;
			this.totalInputs += 1;
            //Debug.Log ("Total Inputs: " + totalInputs);
            //Debug.Log("Inputs for " + move + " : " + this.moveFreq[move]);
			return true;
		}
		else
			return false;
	}
	
	// Reset? Call at the end of a round
	public bool ResetCount()
	{
		if (this.resetEveryRound)
		{
			this.moveFreq = new Dictionary<string, int> () {
				{"Foward", 0},
				{"Backward", 0},
				{"Up", 0},
				{"Down", 0},
				{"Button1", 0},
				{"Button2", 0},
				{"Button3", 0},
				{"Button4", 0}
			};
			
			this.totalInputs = 0;
		}
		
		return this.resetEveryRound;
	}
	
	// Whether or not the reset should happen (changes state, then outputs new current state)
	public bool ToggleReset()
	{
		this.resetEveryRound = !this.resetEveryRound;
		return this.resetEveryRound;
	}
	
	
	/* Drawing stuff on the canvas
	 */
	// This only works if it's enabled in options
	public bool Visualize()
	{
		ClearDrawing();
		
		if (nh)
		{
			// Check if in diagnosticMode, draw only if TRUE
			if (nh.diagnosticMode)
			{
				float location = 0.0f;
				foreach(KeyValuePair<string, int> entry in this.moveFreq)
				{
					if (this.totalInputs > 0)
					{
						location += _DrawGraph(entry.Key, location, entry.Value);
					}
				}
			}
			
			return nh.diagnosticMode;
		}
		else
			return false;
	}
	
	// Helper method that draws a single box
	float _DrawGraph(string moveName, float start, float count)
	{
		float barLength = this.graphLength * count / this.totalInputs; // Bar length proportional to number of inputs of the given name
		float textStart = barLength / 2; // Draw a line from the middle and put the name of the move

        // Create a GraphPortion to represent this part of the distribution graph
        //Debug.Log("Ratio of " + moveName + " is " + (start + barLength));
        barLength = barLength * 3;
        switch (moveName)
        {
            case "Up":
                if (upBar == null) upBar = GameObject.Find("upBar");
                if (upRect == null) upRect = upBar.GetComponent<RectTransform>();
                if (upText == null)
                {
                    upText = GameObject.Find("upText");
                    upText.GetComponent<Text>().text = "Up";
                }
                //Debug.Log("Setting width to " + (barLength + start));
                upRect.sizeDelta = new Vector2(barLength, 50);
                break;
            case "Down":
                if (downBar == null) downBar = GameObject.Find("downBar");
                if (downRect == null) downRect = downBar.GetComponent<RectTransform>();
                if (downText == null)
                {
                    downText = GameObject.Find("downText");
                    downText.GetComponent<Text>().text = "Down";
                }
                //Debug.Log("Setting width to " + (barLength + start));
                downRect.sizeDelta = new Vector2(barLength, 50);
                break;
            case "Foward":
                if (fowardBar == null) fowardBar = GameObject.Find("fowardBar");
                if (fowardRect == null) fowardRect = fowardBar.GetComponent<RectTransform>();
                if (fowardText == null)
                {
                    fowardText = GameObject.Find("fowardText");
                    fowardText.GetComponent<Text>().text = "Foward";
                }
                //Debug.Log("Setting width to " + (barLength + start));
                fowardRect.sizeDelta = new Vector2(barLength, 50);
                break;
            case "Backward":
                if (backwardBar == null) backwardBar = GameObject.Find("backwardBar");
                if (backwardRect == null) backwardRect = backwardBar.GetComponent<RectTransform>();
                if (backwardText == null) {
                    backwardText = GameObject.Find("backwardText");
                    backwardText.GetComponent<Text>().text = "Backward";
                }
                //Debug.Log("Setting width to " + (barLength + start));
                backwardRect.sizeDelta = new Vector2(barLength, 50);
                break;
            case "Button1":
                if (but1Bar == null) but1Bar = GameObject.Find("but1Bar");
                if (but1Rect == null) but1Rect = but1Bar.GetComponent<RectTransform>();
                if (but1Text == null)
                {
                    but1Text = GameObject.Find("but1Text");
                    but1Text.GetComponent<Text>().text = "Button 1";
                }
                //Debug.Log("Setting width to " + (barLength + start));
                but1Rect.sizeDelta = new Vector2(barLength, 50);
                break;
            case "Button2":
                if (but2Bar == null) but2Bar = GameObject.Find("but2Bar");
                if (but2Rect == null) but2Rect = but2Bar.GetComponent<RectTransform>();
                if (but2Text == null)
                {
                    but2Text = GameObject.Find("but2Text");
                    but2Text.GetComponent<Text>().text = "Button 2";
                }
                //Debug.Log("Setting width to " + (barLength + start));
                but2Rect.sizeDelta = new Vector2(barLength, 50);
                break;
            case "Button3":
                if (but3Bar == null) but3Bar = GameObject.Find("but3Bar");
                if (but3Rect == null) but3Rect = but3Bar.GetComponent<RectTransform>();
                if (but3Text == null)
                {
                    but3Text = GameObject.Find("but3Text");
                    but3Text.GetComponent<Text>().text = "Button 3";
                }
                //Debug.Log("Setting width to " + (barLength + start));
                but3Rect.sizeDelta = new Vector2(barLength, 50);
                break;
            case "Button4":
                if (but4Bar == null) but4Bar = GameObject.Find("but4Bar");
                if (but4Rect == null) but4Rect = but4Bar.GetComponent<RectTransform>();
                if (but4Text == null)
                {
                    but4Text = GameObject.Find("but4Text");
                    but4Text.GetComponent<Text>().text = "Button 4";
                }

                //Debug.Log("Setting width to " + (barLength + start));
                but4Rect.sizeDelta = new Vector2(barLength, 50);
                break;
            default:
                break;
        }

		return start + (barLength / 2);
	}
	
	// Cleans out the graph to be redrawn
	void ClearDrawing()
	{
		foreach(GraphPortion gp in this.portions)
		{
			gp.Destroy();
		}
		
		this.portions = new List<GraphPortion>();
	}
}