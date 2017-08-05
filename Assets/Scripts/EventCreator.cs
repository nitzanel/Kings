using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCreator : MonoBehaviour
{
    public TextAsset text;

    const int titleIndex = 0;
    const int descriptionIndex = 1;
    const int storylineIndex = 2;
    const int indexIndex = 3;
    const int shortNameIndex = 4;
    const int functionIndex = 5;

    public Dictionary<int, Dictionary<string, Event>> events = new Dictionary<int, Dictionary<string, Event>>();

	void Start ()
    {
        string[] lines = text.text.Split('\n');

        foreach (string line in lines)
        {
            string[] entries = line.Split('|');
            if (entries[0][0] == '#') continue;
            List<string> functions = new List<string>();
            for (int i = functionIndex; i < entries.Length; i++)
            {
                functions.Add(entries[i]);
            }
            Event newEvent = new Event(entries[titleIndex], entries[descriptionIndex], entries[storylineIndex], int.Parse(entries[indexIndex]), entries[shortNameIndex], functions);
			// put each event in his indexed position dictionary 
			if (events.Count <= newEvent.index)
			{
				events.Add (newEvent.index, new Dictionary<string, Event> ());
				//Debug.Log(newEvent.title + "    --->    " + "[" + newEvent.index + "][" + )
			} 
            events[newEvent.index].Add(newEvent.storyline, newEvent);
        }
		//printAllEvents ();

    }

	private void printAllEvents()
	{
		/* 
		 * see where every event goes (helps for debug)
		*/
		foreach (var a in events)
		{
			Debug.Log ("in key: " + a.Key.ToString ());
			foreach (var b in a.Value)
			{
				Debug.Log (b.ToString ());
			}
		}
	}

    public Event GetEvent()
    {
        List<string> keys = new List<string>(events[0].Keys);
        return events[0][keys[Random.Range(0, keys.Count)]];
    }

    public Event GetEvent(string storyline, int index)
    {
        return events[index][storyline];
    }

	//is string in string[]?
	public bool IsIn(List<string> arr, string str)
	{
		bool retVal = false;
		for (int i = 0; i < arr.Count; i++)
		{
			if (str == arr[i]) retVal = true;
		}
		return retVal;
	}
		
}
