using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class drop : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum DropType { Character, Action };

    public DropType type;
    public actionData.Targets targetType;

	public GameObject gameManager;
    GameManager gm;
    public int max; //maximum number of cards
    public int cur = 0;
    
	void Awake ()
    {
        gameManager = GameObject.Find("GameManager");
        gm = gameManager.GetComponent<GameManager>();
    }

    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData data)
    {

    }

    public void OnPointerExit(PointerEventData data)
    {

    }

    public void OnDrop(PointerEventData data)
    {
        card card; //temporary variable, not really important
        //get the component. if it doesn't exist or if the drop zone is full, return
        card = data.pointerDrag.GetComponent<card>();
        if (!card || transform.childCount + 1 > max || card.type != type ||
            (card.type == DropType.Action && (gm.realm.actionPoints < card.action.data.cost ||
            (targetType != actionData.Targets.All && card.action.data.target != actionData.Targets.All && card.action.data.target != targetType)))) return;
        //perform the drop
        card.originalParent = transform;
        CardEnter(card);
    }

    /*
     * this function has to be called from the outside, and it is super important because it is the only way to get cur down.
     */
    public void CardExit(card card)
    {
        if (transform.name == "CityDropZone")
        {
            cur--;
            // remove character from city
            Realm realm = gm.realm;
            gm.changed = true;
            realm.cities[card.character.data.location].RemoveCharacter(card.character.data);
            card.character.data.location = 0;
        }
        if (type == DropType.Action && transform.name != "PickUpZone")
        {
            cur--;
            // follow action points
            gm.realm.actionPoints += card.action.data.cost;
            gm.realm.UpdateUI();
        }
    }

    public void CardEnter(card card)
    {
        if (transform.name == "CityDropZone" && cur < max)
        {
            cur++;
            // add character to city
            gm.changed = true;
            string parentName = transform.parent.name;
            int cityIndex = (int)(parentName[parentName.Length - 1]) - 48;
            Realm realm = gm.realm;
            card.character.data.location = cityIndex - 1;
            realm.cities[cityIndex - 1].AddCharacter(card.character.data);
        }
        if (type == DropType.Action && transform.name != "PickUpZone" && cur < max)
        {
            cur++;
            // follow action points
            gm.realm.actionPoints -= card.action.data.cost;
            gm.realm.UpdateUI();
        }
    }
}
