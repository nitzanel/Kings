using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    Transform veryOriginalParent = null; //used to know if object was dropped on dropzone or not
    public Transform originalParent = null; //used for drag drop
    Vector3 startScale; //used for rescaling
    
    static bool isDragged = false; //notes if any of the cards is being dragged
    bool isPointerOn = false; //notes if the pointer is on the current card

    public RectTransform panelPrefab; //panel template. has to be inserted from editor!
    RectTransform panel; //actual reference to panel

    public ActionCard action;
    public CharacterCard character;

    public drop.DropType type;

    void Start()
    {
        startScale = transform.localScale = new Vector3(1, 1, 1); //find the initial scale

        if (!panelPrefab) Debug.LogError("No panel prefab! Please add it."); //log error if we forgot to put in a character panel

        character = GetComponent<CharacterCard>();
        action = GetComponent<ActionCard>();

        CreatePanel();

		drop d = transform.parent.GetComponent<drop> ();
		if (d)
			d.CardEnter (this);
    }
	
	void Update ()
    {
        if (Input.GetMouseButtonUp(0) && isPointerOn && !isDragged) //on click open panel
        {
            if (character)
            {
                character.UpdatePanel(panel);
                OpenPanel();
            }
        }
    }
    
    public void OnBeginDrag(PointerEventData data)
    {
        //remember original parent and go to his parent
        originalParent = transform.parent;
        veryOriginalParent = originalParent;
        transform.SetParent(GameObject.Find("Canvas").transform);

        //help drop zone know that a card is leaving
        drop d = originalParent.GetComponent<drop>();
		if (d) d.CardExit(this);

        //disable image raycast (to enable dropping) and ignore layout
        GetComponent<Image>().raycastTarget = false;
        foreach (Image i in GetComponentsInChildren<Image>())
            i.raycastTarget = false;
        GetComponent<LayoutElement>().ignoreLayout = false;

        //be on top
        transform.SetAsLastSibling();

		StartCoroutine (SetDragBool (true));
    }

    public void OnDrag(PointerEventData data)
    {
        //follow mouse
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData data)
    {
        //return to original parent (changed on drop)
        transform.SetParent(originalParent);

        if (originalParent == veryOriginalParent) //if came back to same parent, enter it (it wont enter itself)
            originalParent.GetComponent<drop>().CardEnter(this);

        //raycast and layout normal again
        GetComponent<Image>().raycastTarget = true;
        foreach (Image i in GetComponentsInChildren<Image>())
            i.raycastTarget = true;
        GetComponent<LayoutElement>().ignoreLayout = false;

		StartCoroutine (SetDragBool (false));
    }

    public void OnPointerEnter(PointerEventData data)
    {
        //big if mouse is on
        StartCoroutine(LerpScale(0.1f, true));

        isPointerOn = true;
    }

    public void OnPointerExit(PointerEventData data)
    {
        //small if mouse is off
        StartCoroutine(LerpScale(0.1f, false));

        isPointerOn = false;
    }

    void OpenPanel()
    {
        if (!panel.gameObject.active)
        {
            //close all open panels
            foreach (GameObject o in GameObject.FindGameObjectsWithTag("Panel"))
            {
                o.SetActive(false);
            }
            panel.gameObject.SetActive(true);
        }
        else
            panel.gameObject.SetActive(false);
    }

    void CreatePanel()
    {
        //create panel and parent it to canvas
        panel = Instantiate(panelPrefab);
        panel.gameObject.SetActive(false);
        panel.parent = GameObject.Find("Canvas").transform;
        panel.offsetMax = new Vector2(0, 0);
        panel.offsetMin = new Vector2(0, 0);
        
        if (character)
		    panel.GetComponent<characterPanel> ().card = this;
    }

    public void Transfer(Transform parent)
    {
        transform.parent.GetComponent<drop>().CardExit(this);
        transform.SetParent(parent);
        if (transform.parent.GetComponent<drop>())
            transform.parent.GetComponent<drop>().CardEnter(this);
    }

    IEnumerator LerpScale(float time, bool big)
    {
        //create new scale vector and time
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = big ? startScale + new Vector3(0.1f, 0.1f, 0f) : startScale;
        float originalTime = time;

        //change size
        while (time > 0.0f)
        {
            time -= Time.deltaTime;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, time / originalTime);
            yield return new WaitForFixedUpdate();
        }
    }

	IEnumerator SetDragBool(bool drag)
	{
		yield return new WaitForSeconds (0.05f);
		isDragged = drag;
	}
}
