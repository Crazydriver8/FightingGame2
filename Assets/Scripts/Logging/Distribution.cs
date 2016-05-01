using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


/* Make a bunch of these to keep all parts of the graph organized
 */
public class GraphPortion
{
	float start = 0;
		  end = 0;
	
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
	
	List<GraphPortion> portions = new List<GraphPortion> ();
	
	NameHolder nh;
	
	
	void Start()
	{
		nh = GameObject.Find("Name").GetComponent<NameHolder>();
		resetEveryRound = false;
		
		// Default size of distribution graph
		if (graphLength == -1)
			graphLength = 1000;
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
		
		return start + barLength;
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