using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour
{
    public string title;
    public string description;
    public string storyline;
    public int index;
    public string shortName;
    public List<string> functions;

    public Event() { }

    public Event(string Title, string Description, string Storyline, int Index, string ShortName, List<string> Functions)
    {
        title = Title;
        description = Description;
        storyline = Storyline;
        index = Index;
        shortName = ShortName;
        functions = Functions;
    }

    public Event(Event copy)
    {
        title = copy.title;
        description = copy.description;
        storyline = copy.storyline;
        index = copy.index;
        shortName = copy.shortName;
        functions = copy.functions;
    }

	void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}
}
