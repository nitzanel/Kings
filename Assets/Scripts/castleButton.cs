using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class castleButton : MonoBehaviour
{
    public RectTransform panel;

    void Start()
    {
        panel = GameObject.Find("CastlePanel").GetComponent<RectTransform>();
        panel.gameObject.SetActive(false);
    }

    public void OpenPanel()
	{
		if (!panel.gameObject.activeSelf)
		{
			//before turning the panel on, turn all other panels off
			foreach (GameObject o in GameObject.FindGameObjectsWithTag("Panel"))
			{
				o.SetActive (false);
			}
			// tell to get targets castle
			panel.gameObject.SetActive (true);
			Helper.UpdateCardsTargetMatch (actionData.Targets.Castle);
			Helper.UpdateCardsCondition (panel.gameObject);
		} 
		else
		{
			Helper.UpdateCardsTargetMatch (actionData.Targets.City);
			Helper.UpdateCardsCondition (null);
			panel.gameObject.SetActive (false);
		}
	}
}
