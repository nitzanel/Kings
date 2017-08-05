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
	public int gold;
	// how much armies does the king have in the realm.
	int loyalArmy;
	// how loyal the realm is to the king.
	int realmLoyalty;
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
		nameGen = new nameGenerator();
		// create cities, starting gold, armies, loyalty
		gold = startingGold;
		realmLoyalty = startingArmies;
        actionPoints = startingActionPoints;
        beginningOfTurnActionPoints = startingActionPoints;
		// set cities names, gold, armies
		for (int i = 0; i < NumberOfCities; i++) 
		{
			City city = new City (nameGen.GetName(), Random.Range(100, 1000),Random.Range(5,11),Random.Range(10,51));
			cities [i] = city;
		}
		DeduceArmy();
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

        //deal with event functions
        ExecuteEventFunctions();

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
		UpdateMayors();
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
	public void UpdateMayors()
	{
		foreach (City city in cities)
		{
			if (city.people.Count > 0)
			{
				city.people [0].ModifyLoyalty (-LoyaltyDecay);
			}
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
			Application.LoadLevel ("GameOver");
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
            if (city.people.Count != 0)
            {
                peopleData mayor = city.people[0];
                barStats stats = mayor.getStats(true);

                switch (city.rollResult(stats.loyalty, stats.talent, MinLoayaltyTaxCollect, MaxLoayaltyTaxCollect))
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

        beginningOfTurnActionPoints = actionPoints;
    }
	/*
		Update ui elements.
	*/
    public void UpdateUI()
    {
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
        GameObject.Find("Turn").transform.Find("Text").GetComponent<Text>().text = turnCounter.ToString();
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
            if (!child.gameObject.active)
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
        System.Type thisType = this.GetType();
        MethodInfo theMethod = thisType.GetMethod(c.data.function);
        theMethod.Invoke(this, new object[] { c.transform.parent.parent.gameObject });
    }

    void ExecuteEventFunctions()
    {
        foreach (string function in thisTurnEventFunctions)
        {
            try
            {
                System.Type thisType = this.GetType();
                MethodInfo theMethod = thisType.GetMethod(function);
                theMethod.Invoke(this, new object[] { });
            }
            catch
            {
                Debug.Log(function + " does not exist!");
            }
        }
        thisTurnEventFunctions = new List<string>();
	
		GameObject.Find ("GameManager").GetComponent<GameManager> ().GetNewEvents ();

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
                        if (currentCity.people.Count > 0)
                        {
                            peopleText = "in the city:\n";
                            bool first = true;
                            foreach (peopleData data in currentCity.people)
                            {
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

    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions
    // Action functions

    /*
	 * returns peopleData of game object
	 */
    public peopleData GetTargetPeople(GameObject obj)
	{
		return obj.GetComponent<characterPanel> ().card.character.data;
	}
	/*
		Returns city from a cityPanel object
	*/
	public City GetTargetCity(GameObject obj)
	{
		// looks on the name of the panel, returns the city in the index according to the name of the panel.
		// the 48 is because of ascii(ithink)
		// some hacky code -- ask Omri
		return this.cities [obj.name [obj.name.Length - 1] - '0' - 1];
	}
	/*
		Action that imprison a character in jail.
	*/

	private void removeFromCourt(card c)
	{
		card[] court = GameObject.Find ("GameManager").GetComponent<GameManager> ().updateCourt ();
		for (int i = 0; i < court.Length; i++)
		{
			if (court [i] == c)
			{
				Debug.Log ("REMOVED");
				court [i] = null;
			}
		}
	}

    public void Imprison(GameObject obj)
    {
        // Move character to prison dropzone, and set it to be inactive
		card prisoner = obj.GetComponent<characterPanel>().card;
		removeFromCourt (prisoner);
		prisoner.Transfer(GameObject.Find("Prison").transform.Find("DropZone"));


		// need to remove from court if in court

		if (obj.activeSelf)
            obj.SetActive(false);
    }
	/*
		Action that kills a character.
		destroy both the card object, and the card panel.
	*/
	public void Kill(GameObject obj)
	{
		// card object to destroy
		GameObject toDestroy;
		toDestroy = obj.GetComponent<characterPanel> ().card.gameObject;
		/// NULL FUCKING REFERENCE WHEN YOU TRY TO KILL SOMEONE --FIXED--
		//obj.GetComponent<characterPanel> ().card.Transfer (GameObject.Find ("Map").transform.Find("Prison").transform.Find("DropZone").transform);
		obj.GetComponent<characterPanel>().card.ExitCurrent();
		GameObject.Destroy (obj);
		GameObject.Destroy (toDestroy);

	}
	
	/*
		Gives a Bribe to a character, inceases loyalty to you.
		The price is a placeholder, and should probably be changed.
	*/
	public void Bribe(GameObject obj)
	{
		GetTargetPeople(obj).ModifyLoyalty(20);
        gold -= 200;
	}

	/*
		Spies on a target.
		Reveals all the information on the target (Debug Reveal)
	*/
	public void Spy(GameObject obj)
	{
		peopleData target = GetTargetPeople(obj);
		target.DebugReveal ();
	}
	/*
		Raises taxes in the city target for the rest of the game.
		(Maybe it shouldnt be for the rest of the game)
		the value is a place holder, consider changing it sometime.
	*/
	public void RaiseTaxes(GameObject obj)
	{
		GetTargetCity (obj).tax += 0.2f;
	}
	/*
		Increase Charisma of a target character.
		the value is a place holder, consider changing it sometime.
	*/
	public void IncreaseCharisma(GameObject obj)
	{
		GetTargetPeople (obj).ModifyCharisma (20);
	}

	/*
		Increase Charisma of a target character.
		the value is a place holder, consider changing it sometime.
	*/
	public void IncreaseTalent(GameObject obj)
	{
		GetTargetPeople (obj).ModifyTalent (20);
	}
	/*
		Attacks a city target.
		Creates an event of a city attack.
		--TODO--
	*/
	public void AttackCity(GameObject obj)
	{
		Debug.Log("Attacked: " + GetTargetCity (obj).name);
		AttackACity (GetTargetCity (obj));
	}
	/*
		Call to arms people in the city, increasing the levy tax, 
		and brings the player more army from the city.
		the value is a place holder, consider changing it sometime.
		Should probably be removed --TODO--
	*/
	public void CallToArms(GameObject obj)
	{
		City city = GetTargetCity (obj);
		this.loyalArmy  += city.army /2;
		city.UpdateReportText ("Action used: Call To Arms - Took " + city.army/2 + " army units.\n");
		city.army /= 2;
	}
	/*
		Recruit army in the city, increasing the amout of soldiers it will have.
		--TODO--
	*/
	public void RecruitArmy(GameObject obj)
	{
		GetTargetCity (obj).millateristic += 20;
		GetTargetCity (obj).UpdateReportText ("Action used: Recruit Army - millateristic increased by 20%\n");

	}
	/*
		Increase the loyalty of a city.
		the value is a place holder, consider changing it sometime.
	*/
	public void IncreaseCityLoyalty(GameObject obj)
	{
		GetTargetCity (obj).loyalty += 20;
		GetTargetCity (obj).UpdateReportText ("Action used: Increase Loyalty - loyalty increased by 20%\n");

	}
	/*
		Increase the growth of the city.
		the value is a place holder, consider changing it sometime.
	*/
	public void IncreaseCityPopulation(GameObject obj)
	{
		GetTargetCity (obj).growth += 20;
		GetTargetCity (obj).UpdateReportText ("Action used: Increase Population - growth increased by 20%\n");

	}
	/*
		Kills everyone in the court, then finds replacements for them.
		--TODO--
	*/
	public void ReplaceCourt(GameObject obj)
	{
		Transform castle = GameObject.Find ("CastlePanel").transform;
		Debug.Log ("DO REPLACE COURT");
	}



// events
// events
// events
// events
// events
// events
// events
// events
// events
// events


	/*
	 * KIlls random court member
	 */
	public void Accident()
	{
		/*
		 * Accident event
		 */
		Debug.Log ("Entered accident");
		bool startState = false;
		castlePanel = GameObject.Find ("GameManager").GetComponent<GameManager> ().castlePanel.GetComponent<RectTransform>();
		if (castlePanel.gameObject)
			startState = castlePanel.gameObject.activeSelf;
		castlePanel.gameObject.SetActive (true);
		//Transform castle = GameObject.Find("CastlePanel").transform;
		Debug.Log("looking for gamemanager");
		card[] court = GameObject.Find ("GameManager").GetComponent<GameManager> ().updateCourt();
		Debug.Log ("got court");
		List<int> targets = new List<int> ();
		for (int i = 0; i < 3; i++)
		{
			if (court [i])
			{	
				targets.Add (i);
			} 
			else
			{
				// court member is rip
			}
				
		}
		// if anyone in the court alive, slay!
		if (targets.Count > 0)
		{
			int targetIndex = targets[Random.Range (0, targets.Count)];
			Kill (court [targetIndex].charPanel.gameObject);
			Debug.Log ("Sent to kill target");
		}
		else 
		{
			//no one to 'accidently' kill
		}

		castlePanel.gameObject.SetActive (startState);
	}
	public void AccidentProbablyjustanaccident()
	{
		Accident ();
	}

	public void AccidentInvestigatefurther()
	{
		Accident ();
		realmLoyalty += 5;
		realmLoyalty = Mathf.Max (realmLoyalty, 100);
	}
	/*
	 * Burning city event
	 */
	/*
	A random city burns, and loses half of its population.
	the value is a place holder, consider changing it sometime.
	*/
	public void BurningCity()
	{
		cities [Random.Range (0, 3)].population /= 2;
	}
	public void BurnningCitySendthearmy()
	{
		//rip army
		loyalArmy /= 2;
	}

	public void BurnningCityRebuildwithmoney()
	{
		// rip money
		gold =(int)(gold * 0.75f);
	}

	public void BurnningCityCryinthecorner()
	{
		BurningCity ();
	}
	// bad luck investigate
	public void BadLuckInvestigate()
	{
		// damn money bag
		if (Random.Range (0, 100) > 95)
			gold += 99999;
		else //or nothing
			gold -= 200;
		gold = Mathf.Max (gold, 0);

	}

	public void BadLuckMournhim()
	{
		gold -= 200;
		gold = Mathf.Max (gold, 0);
		realmLoyalty += 5;
	}

	public void BadLuckIneverlikedbabiesanyway()
	{
		realmLoyalty -= 5;
	}
	// feast event
	public void FeastFirethecoinmaster ()
	{
		gold /= 2;
		castlePanel = GameObject.Find ("GameManager").GetComponent<GameManager> ().castlePanel.GetComponent<RectTransform> ();
		bool state = castlePanel.gameObject.activeSelf;
		castlePanel.gameObject.SetActive (true);
		// update the court variable of the game manager. ( get newest court)
		card[] court = GameObject.Find ("GameManager").GetComponent<GameManager> ().updateCourt ();
		// check if there is a coin master
		if (court[1])
		{	
			Debug.Log ("Found coin master");
			Kill (court[1].charPanel.gameObject);
		}
		else
		{
			// no coin master
		}
		castlePanel.gameObject.SetActive (state);
	}

	public void FeastLetsenjoythemealatleast()
	{
		gold /= 2;
		realmLoyalty -= 10;
	}

	/*
	A ceremony is taking place.
	lose gold and army.
	*/
	public void CeremonyOhwell()
	{
		this.loyalArmy = (int)(0.5f * this.loyalArmy);
		this.gold = (int)(0.8f * this.gold);
	}

	// rebeliion
	public void RebellionRumorsDamnTraitors()
	{
		Debug.Log ("Added rebellion");
		nextTurnEvents.Add(GameObject.Find("GameManager").GetComponent<EventCreator>().events[1]["RebellionRumorsDamnTraitors"]);
	}
	public void RebellionRumorsButImGood()
	{
		nextTurnEvents.Add(GameObject.Find("GameManager").GetComponent<EventCreator>().events[1]["RebellionRumorsButImGood"]);
	}

	public void RebellionRumorsYes()
	{
		nextTurnEvents.Add(GameObject.Find("GameManager").GetComponent<EventCreator>().events[2]["RebellionRumorsYes"]);
	}

	public void RebellionRumorsNo()
	{
		nextTurnEvents.Add(GameObject.Find("GameManager").GetComponent<EventCreator>().events[2]["RebellionRumorsNo"]);
	}
	public void RebellionRumorsExecuteHim()
	{
		card[] court = GameObject.Find ("GameManager").GetComponent<GameManager> ().updateCourt ();
		if (court [0])
		{
			Kill (court [0].charPanel.gameObject);
		} else
		{
			// no general to kill
		}
        nextTurnEvents.Add(GameObject.Find("GameManager").GetComponent<EventCreator>().events[3]["RebellionRumorsExecuteHim"]);
	}
	public void RebellionRumorsSpareHim()
	{
		nextTurnEvents.Add(GameObject.Find("GameManager").GetComponent<EventCreator>().events[3]["RebellionRumorsSpareHim"]);
	}
	public void RebellionRumorsLetsParty()
	{
		gold += 10000;
		realmLoyalty = 100;
		loyalArmy += 1000;
		Debug.Log ("LetsParty!");
	}

	public void RevDesireGreatJob()
	{
		
	}

	public  void RevAllAmazing()
	{

	}

	public void RevTraitsNice()
	{
		
	}
	public void RevTraitOk()
	{

	}

	public void RevNoneBadluck()
	{
	}
	public void RevNoneSpytheSpyMaster()
	{
		
	}
	/*
		A golden age arrived, increasing growth by 20%
		the value is a place holder, consider changing it sometime.
	*/
	public void GoldenAgePraisetherealm()
	{
		foreach (City city in cities)
		{
			city.EnterGoldenAge (10);
		}
	}
	/*
		Some bad stuff happend, you should probably be sad
		--TODO--
	*/
	public void BadLuck()
	{
		gold -= 200;
		gold = Mathf.Max (0, gold);
		Debug.Log ("So Sad");
	}
	/*
		Spend alot of money on a lavish feast.
		The people are angry.
	*/
	public void Feast()
	{
		this.gold = (int)(0.8f * this.gold);
		this.realmLoyalty -= 5;
		this.realmLoyalty = Mathf.Max (0, this.realmLoyalty);
	}
	/*
		make sure that there is a general
		Attacks a city, the leader of the attack is the general.
		If the attacker is strong enough, the city is taken, and its loyalty restored.
		else, the player loses and all loyalty is lost.
	*/
	public void AttackACity(City target)
	{
		// Obvioulsy this try catch block should not be here, but it is a jam.
	
		card generalCard = GameObject.Find ("GameManager").GetComponent<GameManager> ().updateCourt () [0];

		if (generalCard)
		{
			peopleData general = generalCard.character.data;
			if ((general.getStats(true).talent) * (0.01f) * loyalArmy >= target.army)
			{
				target.loyalty = 100;
				loyalArmy -= target.army;
				target.army = 0;
			}
			// failed attack
			else
			{
				target.loyalty=0;
				loyalArmy -= target.army;
				target.army = target.army - (int) (loyalArmy * 0.01f * general.getStats(true).talent ) ;
			}
			loyalArmy = Mathf.Max (0, loyalArmy);
		}
		else
		{
			// should actually not allow to place the card
			Debug.Log("there is no general you dumb");
		}
	}
}
