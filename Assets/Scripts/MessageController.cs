using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageController : MonoBehaviour {

    // Use this for initialization
    bool Expanded;
    public bool inUse;
	void Start ()
    {
        Shrink();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Click()
    {
        if (Expanded)
            Shrink();
        else
            Expand();
    }

    public void Shrink()
    {
        // set message anchor to small (fit as many as possible)
        float children = (float)transform.parent.childCount + 5;
        float index = (float)transform.GetSiblingIndex() ;
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(rt.anchorMin.x, index / children);
        rt.anchorMax= new Vector2(rt.anchorMax.x, (index+1.0f) / children);
        Helper.ResetRect(rt);

        // set button anchor to full
        SetButtonFull();
        // disable content text
        DisableContent();
        Expanded = false;
    }

    void Expand()
    {
        
        // shrink all others
        // set message anchor to small (fit as many as possible)
        // set as last
        transform.SetAsLastSibling();
        float children = (float)transform.parent.childCount +5 ;
        float index = (float)transform.GetSiblingIndex();
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(rt.anchorMin.x, index / children);
        rt.anchorMax = new Vector2(rt.anchorMax.x, (index + 5.0f) / children);
        Helper.ResetRect(rt);
        SetButtonPartial();
        EnableContent();

        Expanded = true;
        inUse = true;
        GameObject.Find("MailBox").GetComponent<MailBox>().CleanMessages();
        inUse = false;
    }


    void DisableContent()
    {
        transform.Find("Parent").Find("Content").gameObject.SetActive(false);
    }

    void EnableContent()
    {
        transform.Find("Parent").Find("Content").gameObject.SetActive(true);
    }

    void SetButtonFull()
    {
        RectTransform rt = transform.Find("Parent").Find("Button").GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = new Vector2(1.0f, 1.0f);
        Helper.ResetRect(rt);
        RectTransform rtX = transform.Find("X").GetComponent<RectTransform>();
        rtX.anchorMin = new Vector2(0.8f,0.0f);
        rtX.anchorMax = Vector2.one;
        Helper.ResetRect(rtX);
    }

    void SetButtonPartial()
    {
        RectTransform rt = transform.Find("Parent").Find("Button").GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f,0.8f);
        rt.anchorMax = new Vector2(1.0f, 1.0f);
        Helper.ResetRect(rt);
        RectTransform rtX = transform.Find("X").GetComponent<RectTransform>();
        rtX.anchorMin = new Vector2(0.8f, 0.8f);
        rtX.anchorMax = Vector2.one;
        Helper.ResetRect(rtX);
    }
}

