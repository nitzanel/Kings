using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class ActionCardCreator : MonoBehaviour
{
    public TextAsset text;

    //name|description|target|cost|function
    //(target = people/city/castle/event/all)
    const int namIndex = 0;
    const int descriptionIndex = 1;
    const int targetIndex = 2;
    const int costIndex = 3;
    const int functionIndex = 4;

    static List<actionData> data = new List<actionData>();

	void Start ()
    {
        string[] lines = text.text.Split('\n');

        foreach (string line in lines)
        {
            string[] entries = line.Split('|');
            actionData.Targets target = actionData.Targets.All;
            switch (entries[targetIndex])
            {
                case "people":
                    target = actionData.Targets.People;
                    break;
                case "city":
                    target = actionData.Targets.City;
                    break;
                case "castle":
                    target = actionData.Targets.Castle;
                    break;
                case "event":
                    target = actionData.Targets.Event;
                    break;
            }
            actionData action = new actionData(entries[namIndex], int.Parse(entries[costIndex]), entries[descriptionIndex], target, entries[functionIndex]);
            for(int i = 0; i < int.Parse(entries[entries.Length - 1]); i++)
                data.Add(action);
        }
	}

    public static actionData DrawCard()
    {
        return new actionData(data[Random.Range(0, data.Count - 1)]);
    }
}
