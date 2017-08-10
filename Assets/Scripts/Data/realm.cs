using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public enum Result { MajorFailure, Failure, Nothing, Success, MajorSuccess, FailedSabotage, Sabotage, MajorSabotage }
/*
 * The Realm class controls the flow of the game, 
 * and the cities it holds. 
 */
public class Realm  
{
	const int NumberOfCities = 3;
    const int startingGold = 500;
    const int startingArmies = 100;
    const int startingActionPoints = 10;
	const int LoyaltyDecay = 1;

	public RectTransform castlePanel;
	// array of cities in the realm.
	public City[] cities = new City[NumberOfCities];
	// how much gold does the king have.
	public long  gold;
	public long nextTurnGold;
	// how much armies does the king have in the realm.
	public long loyalArmy;
	// how loyal the realm is to the king.
	public int realmLoyalty;
    public int actionPoints; //used to play action cards
    private int beginningOfTurnActionPoints; //used to calculate the difference

	// list of people in the realm, not utilized at the moment
	public List<peopleData> people;

    public Random rand;

	// nameGenarator for city names
	private nameGenerator nameGen;

    public int turnCounter = 0;

    public List<Event> nextTurnEvents = new List<Event>();
    public List<string> thisTurnEventFunctions = new List<string>();

	public Realm ()
	{
        rand = new Random();
        
		// create cities, starting gold, armies, loyalty
		gold = startingGold;
		realmLoyalty = startingArmies;
        actionPoints = startingActionPoints;
        beginningOfTurnActionPoints = startingActionPoints;
        nameGen = new nameGenerator();
        // set cities names, gold, armies
        for (int i = 0; i < NumberOfCities; i++) 
		{
			City city = new City (nameGen.GetName(), Random.Range(100, 1000),Random.Range(5,11),Random.Range(10,51));
			cities [i] = city;
		}
		DeduceArmy();
	}

    public void NormalizeValues()
    {
        realmLoyalty = Mathf.Min(100, realmLoyalty);
        realmLoyalty = (int)Mathf.Max(0, realmLoyalty);
        gold = (long)Mathf.Max(0, gold);
        loyalArmy = (int)Mathf.Max(0, loyalArmy);
        actionPoints = (int)Mathf.Max(0, actionPoints);
    }

	/*
	 * Set the name of the cities on the map
	 */
	public void SetCitiesNames(GameObject[] citiesGO)
	{
		for (int i = 0; i < citiesGO.Length; i++) 
		{
			citiesGO [i].GetComponentInChildren<Text>().text = cities [i].name;
		}
	}


	/*
	 * Process the happening of a turn
	 */
	public void ProcessTurn()
	{
        turnCounter++;
        NormalizeValues();

        //deal with event functions
        ExecuteEventFunctions();

        // Add an year
        Helper.AddYearSinceLastEvent();

        // create new events
        CreateNewEvents();

		//do actions
		ResetReportText();

		PerformActions();
        // Add gold
        DeduceGold();

		// Add armies
		DeduceArmy();
		// Do the rest
        UpdateCities();

		// Update Loyalty
		UpdateLoyalty();

		// Update mayors
		UpdateMayorsLoyalty();
		//update action points
        UpdateActionPoints();

		//update UI
        UpdateUI();
	}
	

	/*
		Adds army from cities to player according to loyalty and talent of mayors.
		Decided on no army from cities regularly
	*/
	public void DeduceArmy()
	{/*
		foreach (City city in cities)
		{
			if (city.people.Count > 0)
			{
				if (city.people[0].getStats(true).loyalty  >= 50 && city.loyalty >=50)
				{
					loyalArmy += (int)(city.army * (0.005f) *city.loyalty);
				} 
				else
				{
					loyalArmy -= (int)(city.army * (0.5f - (0.005f * city.loyalty)));
				}
			}
		}

		loyalArmy = Mathf.Max (0, loyalArmy);
		*/

	}

	// Update Mayors loyalty
	public void UpdateMayorsLoyalty()
	{
		foreach (City city in cities)
		{
			if (city.cards.Count > 0)
			{
				city.cards [0].character.data.ModifyLoyalty (-LoyaltyDecay);
			}
		}
	}


    public void CreateNewEvents()
    {
        // May be changed!
        const int chanceToDoEvent = 10;
        CharactersStruct chars = Helper.GetCharacters();
        // get a list from the array
        List<card> court = new List<card>();
        foreach (card c in chars.court)
        {
            if (c)
                court.Add(c);
        }
        // sort the list with a simple lambda function
        court.Sort((c1, c2) => c1.character.data.timeFromLastEvent.CompareTo(c2.character.data.timeFromLastEvent));
        for(int i=0; i<chars.court.GetLength(0); i++)
        {
            
        }
        
    }

    /*
	 * Update realm loyalty according to cities loyaty's 
	 */
    public void UpdateLoyalty()
	{
		// reduce by fixed amount every turn.
		realmLoyalty -= LoyaltyDecay;

		foreach (City city in cities)
		{
			if (city.loyalty >= 100)
				realmLoyalty += 3;
			if (city.loyalty >= 75)
				realmLoyalty += 2;
			else if (city.loyalty >= 50)
				realmLoyalty += 1;
			else if (city.loyalty >= 25)
				realmLoyalty -= 1;
			else if (city.loyalty ==0)
				realmLoyalty -= 2;
		}

		realmLoyalty = Mathf.Min (realmLoyalty, 100);
		realmLoyalty = Mathf.Max (realmLoyalty, 0);
		// if the realm is reaches 0 realmLoyalty, the game is over.
		// maybe this check shouldn't be here
		if (realmLoyalty <= 0)
		{
            // switched to scenemanager
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
		//	Application.LoadLevel ("GameOver");
		}

	}
	/*
		Reset the report text of all cities
	*/
	public void ResetReportText()
	{
		foreach (City city in cities)
		{
			city.lastTurnReport = "";
		}
	}
	const int MinLoayaltyTaxCollect = 15;
    const int MaxLoayaltyTaxCollect = 95;

    
	
	/*
	 * Add gold to player according to cities loyalties, mayor loyalties, talents, and noah's random algorithm
	 */
	void DeduceGold()
    {
		
        // from cities
        foreach (City city in cities)
        {
            // loyalty check >50%?
            // talent check 
            int cityGold = (int)(city.population * (0.01f) * city.loyalty * city.tax);
            int result = 0;
            // Setup the text
            city.lastTurnReport = "Gold: ";
            if (city.cards.Count != 0)
            {
                card mayor = city.cards[0];
                barStats stats = mayor.character.data.getStats(true);

                switch (City.rollResult(stats.loyalty, stats.talent, MinLoayaltyTaxCollect, MaxLoayaltyTaxCollect))
                {
                    case Result.MajorSabotage:
                    // Currently the same
                    case Result.MajorFailure:
                        result -= cityGold;
                        // call event
                        city.UpdateReportText("Major failure :( ");
                        break;
                    case Result.Sabotage:
                    // Currently the same
                    case Result.Failure:
                        result -= cityGold / 2;
                        city.UpdateReportText("Failure :( ");
                        // Call event
                        break;
                    case Result.FailedSabotage:
                    // Currently the same
                    case Result.Nothing:
                        // call event
                        city.UpdateReportText("Nothing :| ");
                        break;
                    case Result.Success:
                        result += cityGold;
                        // call event
                        city.UpdateReportText("Success :) ");
                        break;
                    case Result.MajorSuccess:
                        result += 2 * cityGold;
                        // call event
                        city.UpdateReportText("Major success :) ");
                        break;
                    default:
                        break;
                }
                // MODIFY WITH COIN MASTER?
                gold += result;
                city.UpdateReportText(result + "\n");
            }
            else
            {
                // No mayor, chaos?
                city.UpdateReportText("No Mayor.\n");
            }
        }
        // get gold from loyalty of cities
        if (gold < 0)
        {
            // Broke?
            gold = 0;
        }
		nextTurnGold = gold;
    }

    // call the update function of each city.
    void UpdateCities()
    {
        foreach (City city in cities)
        {
            city.UpdateCity(this.realmLoyalty);
        }

    }
	/*
		Update the amount of action points the player have, depending on realmLoyalty.
		Should also probably depend on court actions.
	*/
    void UpdateActionPoints()
    {
        actionPoints += startingActionPoints * realmLoyalty / 100;
		Helper.UpdateCanPlayCards (actionPoints);
        beginningOfTurnActionPoints = actionPoints;
    }
	/*
		Update ui elements.
	*/
    public void UpdateUI()
    {
		// update what cards can be played
		Helper.UpdateCanPlayCards (Helper.GetRealm().actionPoints);
		Helper.UpdateCardsCondition (null);

        //gold
        GameObject.Find("Gold").transform.Find("Current").GetComponent<Text>().text = gold.ToString();
        //loyalty
        GameObject.Find("Loyalty").transform.Find("Current").GetComponent<Text>().text = realmLoyalty.ToString();
        //army
        GameObject.Find("Army").transform.Find("Current").GetComponent<Text>().text = loyalArmy.ToString();
        //AP
        GameObject.Find("ActionPoints").transform.Find("CurrentAP").GetComponent<Text>().text = beginningOfTurnActionPoints.ToString();
        GameObject.Find("ActionPoints").transform.Find("UsingAP").GetComponent<Text>().text = "+" + (startingActionPoints * realmLoyalty / 100) + " -" + (beginningOfTurnActionPoints - actionPoints);
        GameObject.Find("ActionPoints").transform.Find("NextAP").GetComponent<Text>().text = actionPoints.ToString();
        try
        {
            GameObject.Find("CharacterPanel(Clone)").GetComponent<characterPanel>().card.character.UpdatePanel(GameObject.Find("CharacterPanel(Clone)").GetComponent<RectTransform>());
        }
        catch 
		{
			// cant add
			//Debug.Log("Please check why we do try catch here"); //because we are not sure if the character panel is open, and we don't want to do an if statement :)
		}
        //turns
		Helper.WarnCitiesWithNoMayors();
        GameObject.Find("Turn").transform.Find("Text").GetComponent<Text>().text = turnCounter.ToString();
        GameObject.Find("MailBox").GetComponent<MailBox>().CleanMessages();
    }

    /*
     * find all action cards that were played and call the execute function.
     */
    void PerformActions()
    {
        //find all inactive objects, remember them and make them active
        List<Transform> inactive = new List<Transform>();
        foreach (Transform child in GetChildrenBarabak(GameObject.Find("Canvas").transform))
        {
            if (!child.gameObject.activeSelf)
            {
                inactive.Add(child);
                child.gameObject.SetActive(true);
            }
        }

        //now you can get all cards and be happy
        ActionCard[] cards = GameObject.Find("Canvas").GetComponentsInChildren<ActionCard>();

        //make them inactive again
        foreach (Transform active in inactive)
            active.gameObject.SetActive(false);

        //get all the ones that are not in the hand and execute
        foreach (ActionCard c in cards)
        {
            if (c.transform.parent.name != "PickUpZone")
            {
                c.transform.parent.GetComponent<drop>().cur--;
                ExecuteAction(c);
                GameObject.Destroy(c.gameObject);
            }
        }
    }

    /*
     * get all children and their children
     * 
     * nice function name
     */
    List<Transform> GetChildrenBarabak(Transform daddy)
    {
        List<Transform> children = new List<Transform>();
        foreach (Transform child in daddy)
        {
            children.Add(child);
            if (child.childCount > 0)
            {
                children.AddRange(GetChildrenBarabak(child));
            }
        }
        return children;
    }

    /*
     * execute the actual action of the action card.
     */
    void ExecuteAction(ActionCard c)
    {
        // new way of doing actions - invoke static method in the Actions class.
        System.Type actionClassType = System.Type.GetType("Actions");
        MethodInfo someMethod = actionClassType.GetMethod(c.data.function);
        someMethod.Invoke(null, new object[] { c.transform.parent.parent.gameObject });
        /*
         * old way of doing actions
        System.Type thisType = this.GetType();
        MethodInfo theMethod = thisType.GetMethod(c.data.function);
        theMethod.Invoke(this, new object[] { c.transform.parent.parent.gameObject });
        */
    }

    void ExecuteEventFunctions()
    {
        foreach (string function in thisTurnEventFunctions)
        {
            try
            {
                // new way of doing events.
                // invoke the static method in the Events class.
                System.Type eventsClasstype = System.Type.GetType("Events");
                MethodInfo someEventMethod = eventsClasstype.GetMethod(function);
                someEventMethod.Invoke(null, new object[] { });
                /*
                 * old way of doing events
                System.Type thisType = this.GetType();
                MethodInfo theMethod = thisType.GetMethod(function);
                theMethod.Invoke(this, new object[] { });
                */    
           }
            catch
            {
                Debug.Log(function + " does not exist!");
            }
        }
        thisTurnEventFunctions = new List<string>();
	
		Helper.GetGameManager().GetNewEvents ();
		UpdateUI ();
    }

    private bool first = true;

    /*
	 *  set the cities card status text
	 */
    public void SetCitiesCards(GameObject[] citiesCards)
    {
        for (int i = 0; i < citiesCards.Length; i++)
        {
            Text[] texts = citiesCards[i].GetComponentsInChildren<Text>();
            City currentCity = this.cities[i];
            foreach (Text text in texts)
            {
                switch (text.name)
                {
                    case "Name":
                        if (first)
                        {
                            text.text = currentCity.name;
                        }
                        break;

                    case "Status":
                        string peopleText = "";
                        if (currentCity.cards.Count > 0)
                        {
                            peopleText = "in the city:\n";
                            bool first = true;
                            foreach (card card in currentCity.cards)
                            {
                                peopleData data = card.character.data;
                                if (first)
                                {
                                    first = false;
                                }
                                else
                                {
                                    peopleText += ", ";
                                }
                                peopleText += data.name;
                            }
                        }
                        text.text = "population: " + currentCity.population +
                        "\narmy: " + currentCity.army +
                        "\nloyalty: " + currentCity.loyalty +
                        "\n" + peopleText +
                        "\n\nReport:\n" +
                        currentCity.lastTurnReport;
                        break;
                    default:
                        break;
                }
            }
        }
        first = false;
    }


}
