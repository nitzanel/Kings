using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool mouseOn = false;
    public string message = "";
    public GameObject panelPrefab;
    GameObject panel;

    void Start()
    {
        panel = Instantiate(panelPrefab, GameObject.Find("Canvas").transform);
        panel.transform.GetChild(0).GetComponent<Text>().text = message;
        panel.gameObject.SetActive(false);
    }

	void Update ()
    {
        if (mouseOn)
        {
            if (!panel.gameObject.active)
                panel.gameObject.SetActive(true);
            if (Input.mousePosition.x < Screen.width / 2 && Input.mousePosition.y < Screen.height / 2) //bottom left
                panel.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
            else if (Input.mousePosition.x >= Screen.width / 2 && Input.mousePosition.y < Screen.height / 2) //bottom right
                panel.GetComponent<RectTransform>().pivot = new Vector2(1, 0);
            else if (Input.mousePosition.x < Screen.width / 2 && Input.mousePosition.y >= Screen.height / 2) //top left
                panel.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            else //top right
                panel.GetComponent<RectTransform>().pivot = new Vector2(1, 1);
            panel.transform.position = Input.mousePosition;
        }
        else
        {
            panel.gameObject.SetActive(false);
        }
	}

    public void OnPointerEnter(PointerEventData data)
    {
        mouseOn = true;
    }

    public void OnPointerExit(PointerEventData data)
    {
        mouseOn = false;
    }
}
