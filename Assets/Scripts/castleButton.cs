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
        if (!panel.gameObject.active)
        {
            //before turning the panel on, turn all other panels off
            foreach (GameObject o in GameObject.FindGameObjectsWithTag("Panel"))
            {
                o.SetActive(false);
            }

            panel.gameObject.SetActive(true);
        }
        else
            panel.gameObject.SetActive(false);
    }
}
