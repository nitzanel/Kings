using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cityButton : MonoBehaviour
{
    Transform panel;
	public GameObject gameManager;

    void Start()
    {
        //get the panel (based on number and naming conventions) and turn it off
        panel = GameObject.Find("CityPanel" + transform.parent.name[transform.parent.name.Length - 1]).transform;
        panel.gameObject.SetActive(false);
        gameManager = GameObject.Find("GameManager");
    }

    public void OpenPanel()
    {
        if (!panel.gameObject.active) //if the panel is not active, turn it on
        {
			GameManager gm = gameManager.GetComponent<GameManager> ();
			gm.changed = true;
            //before turning the panel on, turn all other panels off
			foreach (GameObject o in GameObject.FindGameObjectsWithTag("Panel"))
            {
                o.SetActive(false);
            }
            panel.gameObject.SetActive(true);
        }
        else //turn the panel off
            panel.gameObject.SetActive(false);
    }
}
