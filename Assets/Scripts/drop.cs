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
		if (transform.name == "CardPack")
		{
			// discard card
			GameObject.DestroyImmediate(card.gameObject);
		}
		// WTF
        if (!card || transform.childCount + 1 > max || card.type != type ||
            (card.type == DropType.Action && (gm.realm.actionPoints < card.action.data.cost ||
            (targetType != actionData.Targets.All && card.action.data.target != actionData.Targets.All && card.action.data.target != targetType)))) return;


		// test condition if it is an action card
		if (gameObject.name != "PickUpZone" && card.type == DropType.Action) 
		{
			if (!card.action.CheckCondition (gameObject)) 
			{
				
				// condition not met
				return;
			}

			// condition met, drop the action card
		}
		//perform the drop
        card.originalParent = transform;
        CardEnter(card);
		Helper.GetRealm ().UpdateUI ();
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
            realm.cities[card.character.data.location].RemoveCharacter(card);
            card.character.data.location = 0;
			gm.realm.UpdateUI ();
        }
        if (type == DropType.Action && transform.name != "PickUpZone")
        {
			if (card.action.data.name == "Bribe") 
			{
				Helper.GetRealm ().gold += 200;
			}

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
			Realm realm = Helper.GetRealm ();
            card.character.data.location = cityIndex - 1;
            realm.cities[cityIndex - 1].AddCharacter(card);
			Helper.RemoveWarningsFromCity (realm.cities [cityIndex - 1], WarningType.NoMayor);
        }
        if (type == DropType.Action && transform.name != "PickUpZone" && cur < max)
        {
			if (card.action.data.name == "Bribe")
			{
				Helper.GetRealm ().gold -= 200;
			}

            cur++;
            // follow action points
			Helper.GetRealm().actionPoints -= card.action.data.cost;
            gm.realm.UpdateUI();
        }
    }
}
