using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event 
{
    public string title;
    public string description;
    public string storyline;
    public int index;
    public string shortName;
    public List<string> functions;
    public card originator;

    public Event() { }

    public Event(string Title, string Description, string Storyline, int Index, string ShortName, List<string> Functions)
    {
        title = Title;
        description = Description;
        storyline = Storyline;
        index = Index;
        shortName = ShortName;
        functions = Functions;
        originator = null;
    }

    public Event(Event copy)
    {
        title = copy.title;
        description = copy.description;
        storyline = copy.storyline;
        index = copy.index;
        shortName = copy.shortName;
        functions = copy.functions;
        originator = copy.originator;
    }
}
