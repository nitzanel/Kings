using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/*
 * Warnings that can be displayed on a city in the map view
 */
public enum WarningType
{
	NoMayor, 
	Burning,
}

public struct CharactersStruct
{
    public card[] mayors;
    public card[] prisoners;
    public card[] court;
    public card[] inHand;
}

/*
 * Utility class with a lot of repeated code of getting objects.
 */
public static class Helper
{
    /*
     * returns the game manager script of game manager object
     */
    public static GameManager GetGameManager()
    {

        return GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    /*
    * Help function that gets the realm.
    */
    public static Realm GetRealm()
    {
        return GetGameManager().realm;
    }

    /*
     * Get the prison dropzone transform
     */
    public static Transform GetPrisonDropZone()
    {
        return GameObject.Find("Prison").transform.Find("DropZone");
    }

    /*
     * returns the castle panel
     */
    public static RectTransform GetCastlePanel()
    {
        return GetGameManager().castlePanel.GetComponent<RectTransform>();
    }

    public static List<Event> GetNextTurnEvents()
    {
        return GetRealm().nextTurnEvents;
    }

    public static City[] GetCities()
    {
        return GetRealm().cities;
    }

    public static void AddEvent(string storyLine, int index)
    {
        GetNextTurnEvents().Add(GetGameManager().GetComponent<EventCreator>().GetEvent(storyLine, index));
    }

    public static Transform GetCharactersBank()
    {
        return GameObject.Find("CharacterBank").transform;
    }

    public static Transform GetHandDropZone()
    {
        return GetCharactersBank().Find("DropZone");
    }

    public static card[] GetCourt()
    {
        return GetGameManager().updateCourt();
    }
    /*
     * D
     */
    public static void ModifyAllLoyalty(int amount)
    {
        foreach (card charCard in GetCharactersList())
        {
            if(charCard)
                charCard.character.data.ModifyLoyalty(amount);
        }
    }

    public static card[] GetMayors()
    {
        List<card> mayors = new List<card>();
        foreach(City city in GetCities())
        {
			if (city.cards.Count !=0)
            	mayors.Add(city.cards[0]);
        }
        return mayors.ToArray();
    }

    public static card[] GetPrisoners()
    {   
        // get the prisoners, could be null.
        return GetPrisonDropZone().GetComponentsInChildren<card>();    
    }

    public static card[] GetCharactersInHand()
    {
        // could be null
        return GetHandDropZone().GetComponentsInChildren<card>();
    }
    /*
     * Return a struct of cards arrays, of the characters in the game.
     */
    public static CharactersStruct GetCharacters()
    {
        CharactersStruct characters = new CharactersStruct();
        characters.court = GetCourt();
        characters.prisoners = GetPrisoners();
        characters.mayors = GetMayors();
        characters.inHand = GetCharactersInHand();

        return characters;
    }

    public static List<card> GetCharactersList()
    {
        List<card> characters = new List<card>();
        CharactersStruct chars = GetCharacters();
        foreach(card character in chars.court)
        {
            if (character)
                characters.Add(character);
        }
        foreach (card character in chars.prisoners)
        {
            if (character)
                characters.Add(character);
        }
        foreach (card character in chars.mayors)
        {
            if (character)
                characters.Add(character);
        }
        foreach (card character in chars.inHand)
        {
            if (character)
                characters.Add(character);
        }
        return characters;
    }

    public static void AddYearSinceLastEvent()
    {
        foreach (card characterCard in GetCharactersList())
        {
            characterCard.character.data.timeFromLastEvent++;
        }
    }

    public static card GetGeneral()
    {
        return GetCourt()[0];
    }

    public static card GetCoinMaster()
    {
        return GetCourt()[1];
    }

    public static card GetSpyMaster()
    {
        return GetCourt()[2];
    }


    /*
	 * returns peopleData of game object
	 */
    public static peopleData GetTargetPeople(GameObject obj)
    {
        return obj.GetComponent<characterPanel>().card.character.data;
    }
    /*
		Returns city from a cityPanel object
	*/
    public static City GetTargetCity(GameObject obj)
    {
        // looks on the name of the panel, returns the city in the index according to the name of the panel.
        // the 48 is because of ascii(ithink)
        // some hacky code -- ask Omri
        return GetRealm().cities[obj.name[obj.name.Length - 1] - '0' - 1];
    }
  

    public static void removeFromCourt(card c)
    {
        card[] court = GetCourt();
        for (int i = 0; i < court.Length; i++)
        {
            if (court[i] == c)
            {
                court[i] = null;
            }
        }
    }

	public static ActionCard[] GetActionCardsInHand()
	{
		return GameObject.Find ("PickUpZone").transform.GetComponentsInChildren<ActionCard> ();
	}

	/*
	 * Update every action card's AP text colr in the player hand. 
	 */
	public static void UpdateCanPlayCards(int availableActionPoints)
	{
		foreach (ActionCard actionCard in GetActionCardsInHand()) 
		{
			actionCard.CheckCanPlay (availableActionPoints);
		}
	}

	/*
	 * Checks if the newly opend panel's target matches action cards target, and recolor their icon.
	 */
	public static void UpdateCardsTargetMatch(actionData.Targets target)
	{
		foreach (ActionCard actionCard in GetActionCardsInHand())
		{
			actionCard.CheckTargetMatch (target);
		}
	}

	/*
	 * Updates the card condition icon
	 * if there is no panel, pass null.
	 */
	public static void UpdateCardsCondition(GameObject panel)
	{
		foreach (ActionCard actionCard in GetActionCardsInHand())
		{
			actionCard.UpdateConditionIcon (panel);
		}
	}

	/*
	 * Returns cities without a mayor
	 */
	public static City[] GetCitiesWithNoMayors()
	{
		List<City> cities = new List<City> (); 
		foreach (City city in GetCities()) 
		{
			if (city.cards.Count == 0)
				cities.Add (city);
		}
		return cities.ToArray ();
	}

	/*
	 * Returns a transform on the map that represents the city
	 */
	public static Transform CityToTransform(City city)
	{
		City[] cities = GetCities ();
		for (int i = 0; i < cities.Length; i++) 
		{
			if (city == cities [i]) 
			{
				return GameObject.Find ("City" + (i + 1)).transform;
			}
		}
		Debug.LogError ("Couldnt find city: " + city.name);
		return null;
	}

	/*
	 * Adds a warning to a city.
	 */
	public static void AddCityWarning(GameObject city, WarningType warning)
	{
		GameObject warningGO = new GameObject ();
		warningGO.transform.SetParent (city.transform);
		warningGO.name = "Warning " + warning.ToString ();

		// dont forget to set position correctly
		Image image = warningGO.AddComponent<Image>();
		image.sprite = Resources.Load<Sprite> ("ImprovedGraphics/Icons/Warnings/" + warning.ToString ());
		// set the position - currently only support one transform
		RectTransform rt = warningGO.GetComponent<RectTransform> ();
		rt.anchorMin = new Vector2 (0.0f, 0.7f);
		rt.anchorMax = new Vector2 (0.3f, 1.0f);
		rt.sizeDelta = Vector2.zero;
		rt.anchoredPosition = Vector2.zero;
		rt.localScale = Vector3.one;
	}

	/*
	 * Give warning sign to cities with no mayors
	 */
	public static void WarnCitiesWithNoMayors()
	{
		foreach (City city in GetCitiesWithNoMayors()) 
		{
			Transform cityTransform = CityToTransform (city);
			if (!cityTransform.Find("Warning "+WarningType.NoMayor))
				AddCityWarning (cityTransform.gameObject, WarningType.NoMayor);
		}
	}

	/*
	 * Remove a warning sign from a city
	 */
	public static void RemoveWarningsFromCity(City city,WarningType warning)
	{
		Transform cityTransform = CityToTransform (city);
		Transform warningTransform = cityTransform.Find ("Warning " + warning.ToString ());
		if (warningTransform) 
		{
			GameObject.DestroyImmediate (warningTransform.gameObject);
		}

	}

    public static void AddMessage(string title, string content)
    {
        GameObject mailbox = GameObject.Find("MailBox");
		mailbox.transform.Find ("Button").GetComponent<Image> ().color = Color.blue;
        Transform messageParent = mailbox.transform.Find("MessagesParent").transform;
        GameObject message = GameObject.Instantiate(mailbox.GetComponent<MailBox>().Message, messageParent);
        //	message.transform.SetParent (mailbox.transform.Find ("MessagesParent"));
        message.transform.Find("Parent").transform.Find("Button").Find("Title").GetComponent<Text>().text = title;
        message.transform.Find("Parent").transform.Find("Content").GetComponent<Text>().text = content;
    }

    public static void ResetRect(RectTransform rt)
    {
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
        rt.localScale = Vector3.one;
    }

}

