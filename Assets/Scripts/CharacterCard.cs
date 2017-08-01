using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterCard : MonoBehaviour
{
    public peopleData data; //data of the person in the card

    //stat bars on the cards (the inner panel)
    RectTransform loyaltyBar;
    RectTransform charismaBar;
    RectTransform talentBar;

    void Awake()
    {
        data = new peopleData(); //initialize people dat

        //find the bars of the card
        loyaltyBar = transform.Find("Loyalty").GetChild(0).GetComponent<RectTransform>();
        charismaBar = transform.Find("Charisma").GetChild(0).GetComponent<RectTransform>();
        talentBar = transform.Find("Talent").GetChild(0).GetComponent<RectTransform>();
    }

    void Start()
    {
		GetComponent<card>().type = drop.DropType.Character;
		data.name = GetComponentInChildren<Text>().text;
    }

    void Update()
    {
        //SHOULD BE EVERY TURN OR SOMETHING MORE GOOD
        UpdateBars();
    }

    public void UpdatePanel(RectTransform panel)
    {
        panel.Find("Name").GetComponent<Text>().text = data.name; //set name

        //create face
        panel.Find("FaceImage").Find("Head").Find("Face").GetComponent<Image>().sprite = transform.Find("Head").Find("Face").GetComponent<Image>().sprite;
        for (int i = 0; i < panel.Find("FaceImage").Find("Head").Find("Face").childCount; i++)
        {
            panel.Find("FaceImage").Find("Head").Find("Face").GetChild(i).GetComponent<Image>().sprite = transform.Find("Head").Find("Face").GetChild(i).GetComponent<Image>().sprite;
        }
        panel.Find("FaceImage").Find("Head").Find("Ear").GetComponent<Image>().sprite = transform.Find("Head").Find("Ear").GetComponent<Image>().sprite;

        //create status
        barStats stats = data.getStats(false);
		Text t = panel.Find ("Status").GetComponent<Text> ();

		t.text = panel.Find ("Status").GetComponent<Text> ().text = "Stats:\n" +
		"Loyalty: " + stats.loyalty + "\n" +
		"Charisma: " + stats.charisma + "\n" +
		"Talent: " + stats.talent + "\n\n" +
		"Traits:\n";
		for (int i = 0; i < 3; i++)
		{
			if (data.traits [i].isRevealed)
			{
				if (data.traits [i].theTrait.ToString () == "None")
				{
					t.text += "\n";
				} else
				{
					t.text += data.traits [i].theTrait.ToString () + "\n";
				}
			} 
			else
			{
				t.text += "Not Revealed\n";
			}
		}
		t.text+= "\nKnown Desire:\n" + data.knownDesire.ToString() + 
			"\n\nSecret Desire:\n" +
         	(data.secretRevealed ? data.secretDesire.ToString() : "Shhhhh");
    }

    void UpdateBars()
    {
        barStats stats = data.getStats(false);

        //update the anchors of the bars according to the value in the stats struct
        loyaltyBar.anchorMax = new Vector2(Remap(stats.loyalty, 0, 100, 0, 1), 1);
        charismaBar.anchorMax = new Vector2(Remap(stats.charisma, 0, 100, 0, 1), 1);
        talentBar.anchorMax = new Vector2(Remap(stats.talent, 0, 100, 0, 1), 1);
    }

    float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}