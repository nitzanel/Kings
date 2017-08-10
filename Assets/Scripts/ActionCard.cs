using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActionCard : MonoBehaviour
{
    public actionData data; //data of the person in the card

    public const drop.DropType type = drop.DropType.Action;

    void Start()
    {
        GetComponent<card>().type = drop.DropType.Action;
    }

    public void UpdateCard()
    {
		// set the title
        transform.Find("Title").GetComponent<Text>().text = data.name;
        // set the description
		transform.Find("Description").GetComponent<Text>().text = data.description;

        // set the card art
		transform.Find("Art").GetComponent<Image>().sprite = Resources.Load<Sprite>("ImprovedGraphics/Cards/" + data.function);
        // set the action points cost
		transform.Find("AP").GetComponent<Text>().text = data.cost.ToString();
		// set the icon sprite
		transform.Find ("TargetIcon").GetComponent<Image> ().sprite = Resources.Load<Sprite> ("ImprovedGraphics/Icons/" + data.target.ToString() + "Icon");
		// set the icon text tooltip
		transform.Find ("TargetIcon").GetComponent<tooltip> ().panel.transform.Find("Text").GetComponent<Text>().text = "Target: " + data.target.ToString();
		tooltip conditionTooltip = transform.Find ("ConditionIcon").GetComponent<tooltip> ();
		if (conditionTooltip)
		{
			if (data.tooltip != "")
				transform.Find ("ConditionIcon").GetComponent<tooltip> ().panel.transform.Find ("Text").GetComponent<Text> ().text = data.tooltip;
			else
				Destroy (conditionTooltip);
		}

    }

	/*
	 * Nitzan: What should be here?
	 */
    public void UpdatePanel(RectTransform panel)
    {
        //to be continued
    }

	/*
	 * Check the condition to play the card.
	 * Should be called from a dropzone - to see if can be used.
	 */
	public bool CheckCondition(GameObject target)
	{
		return data.TestCondition (target);
	}

	// bad function name
	// turns the AP text green if have enough points to play, otherwise, turns red
	public void CheckCanPlay(int availableAP)
	{
		if (data.cost <= availableAP) 
		{
			transform.Find ("AP").GetComponent<Text> ().color = Color.green;
		} 
		else 
		{
			transform.Find ("AP").GetComponent<Text> ().color = Color.red;
		}
	}

	public void CheckTargetMatch(actionData.Targets target)
	{
		if (target == data.target)
		{
			transform.Find ("TargetIcon").GetComponent<Image> ().color = Color.green; 
		}
		else
		{
			transform.Find ("TargetIcon").GetComponent<Image> ().color = Color.red; 
		}
	}

	public void UpdateConditionIcon(GameObject panel)
	{
		if (CheckCondition (panel))
		{
			// condition met
			transform.Find ("ConditionIcon").GetComponent<Image> ().color = Color.green;
		} 
		else
		{
			transform.Find ("ConditionIcon").GetComponent<Image> ().color = Color.red;
		}
	}
}
