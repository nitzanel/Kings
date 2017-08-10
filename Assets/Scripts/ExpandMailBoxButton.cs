using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ExpandMailBoxButton : MonoBehaviour {
	Transform MessagesParent;
	bool expanded = true;
	Vector2 minOriginal = new Vector2(0.8f,0.0f);
	Vector2 maxOriginal = new Vector2(1.0f,0.25f);
	Vector2 minShrink = new Vector2(0.9f,0.0f);
	Vector2 maxShrink = new Vector2(1.0f,0.1f);
	// Use this for initialization
	void Start () 
	{
		MessagesParent = transform.parent.Find ("MessagesParent").transform;
		DisableMessagesParent();
		expanded = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	// expand the mail box, should be changed to not be instant (animation?)
	public void ExpandMailBox()
	{
		GetComponent<Image> ().color = Color.white;
		RectTransform rt = transform.parent.GetComponent<RectTransform>();
		if (!expanded)
		{
			// expand
			expanded = true;
			GetComponentInChildren<Text> ().text = "^";
			ShrinkButton ();
			rt.anchorMin = new Vector2(rt.anchorMin.x,0.5f);
			EnableMessagesParent ();
		} 
		else
		{
			expanded = false;
			rt.anchorMin= new Vector2(rt.anchorMin.x,0.85f);
			// compress
			GetComponentInChildren<Text> ().text = "V";
			DisableMessagesParent ();
			ExpandButton ();
		}
		Helper.ResetRect (rt);
	}

	void DisableMessagesParent()
	{
		MessagesParent.localScale = Vector3.zero;
	}

	void EnableMessagesParent()
	{
		MessagesParent.localScale = Vector3.one;
	}

	void ShrinkButton()
	{
		RectTransform rt = GetComponent<RectTransform> ();
		rt.anchorMax = maxShrink;
		rt.anchorMin = minShrink;
		Helper.ResetRect (rt);
	}

	void ExpandButton()
	{

		RectTransform rt = GetComponent<RectTransform> ();
		rt.anchorMax = maxOriginal;
		rt.anchorMin = minOriginal;
		Helper.ResetRect (rt);	
	}
}
