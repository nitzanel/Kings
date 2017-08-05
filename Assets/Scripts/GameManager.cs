using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour 
{
	public Realm realm;
	public GameObject cardPrefab;
	// was there a change in the description
	public bool changed = false;

	// the cities gameobjects (in the map)
	public GameObject[] cities;
	// the cities cards
	public GameObject[] cityPanels;
    public int cityCount = 3;

    GameObject castlePanel;

	/// <summary>
	/// The drop zones in cities
	/// </summary>
	RectTransform[] dropZones;

    Transform pickUpZone;
    public int handSize = 5;

    public Transform actionCardPrefab;
    public Transform eventPanelPrefab;
    public Transform eventButtonPrefab;

	/// <summary>
	/// The court HOLDS THE WRONG FUCKING THING
	/// IT NEEDS THE CHARACTER PANEL
	/// NOT THE FUCKING CARD
	/// </summary>
	public card[] court = new card[3];

	void Awake () 
	{
        //get cities
        cities = new GameObject[cityCount];
        for (int i = 1; i <= cityCount; i++)
        {
            cities[i - 1] = GameObject.Find("City" + i.ToString());
        }

        //get city cards
        cityPanels = new GameObject[cityCount];
        for (int i = 1; i <= cityCount; i++)
        {
            cityPanels[i - 1] = GameObject.Find("CityPanel" + i.ToString());
        }

		//get drop zones
		dropZones = new RectTransform[cityCount];
		for (int i = 0; i < cityCount; i++)
		{
			dropZones [i] = cityPanels [i].transform.Find ("CityDropZone").transform.GetComponent<RectTransform>();
		}

        //get castle panel
        castlePanel = GameObject.Find("CastlePanel");
	}

    void Start()
    {
        // create the realm
        realm = new Realm();
        
        // create names
        realm.SetCitiesNames(cities);
        // create the cards status text
		realm.SetCitiesCards(cityPanels);

		StartCoroutine (startGame());

        pickUpZone = GameObject.Find("PickUpZone").transform;
		RectTransform castlePanel = GameObject.Find ("Castle").transform.GetChild (0).GetComponent<castleButton> ().panel;
		realm.castlePanel = castlePanel;
	}

    void Update ()
	{
		// change cards status text if needed
		if (changed)
		{
			changed = false;
			realm.SetCitiesCards (cityPanels);
		}
	}

	public GameObject general;
	public GameObject masterOfCoin;
	public GameObject spyMaster;

	IEnumerator startGame()
	{
		for (int i=0;i<3;i++)
		{
			GameObject card = Instantiate (cardPrefab, GameObject.Find("Canvas").transform);
			yield return new WaitForSeconds (0.2f);
			dropZones[i].GetComponent<drop> ().CardEnter (card.GetComponent<card> ());
			card.transform.SetParent (dropZones [i].transform);
		}

        general = Instantiate(cardPrefab, GameObject.Find("Canvas").transform);
        masterOfCoin = Instantiate(cardPrefab, GameObject.Find("Canvas").transform);
		spyMaster = Instantiate(cardPrefab, GameObject.Find("Canvas").transform);

        yield return new WaitForSeconds(0.2f);
        castlePanel.transform.Find("GeneralDropZone").GetComponent<drop>().CardEnter(general.GetComponent<card>());
        general.transform.SetParent(castlePanel.transform.Find("GeneralDropZone"));
        castlePanel.transform.Find("MasterOfCoinDropZone").GetComponent<drop>().CardEnter(masterOfCoin.GetComponent<card>());
        masterOfCoin.transform.SetParent(castlePanel.transform.Find("MasterOfCoinDropZone"));
        castlePanel.transform.Find("SpyMasterDropZone").GetComponent<drop>().CardEnter(spyMaster.GetComponent<card>());
        spyMaster.transform.SetParent(castlePanel.transform.Find("SpyMasterDropZone"));

		updateCourt ();

        StartCoroutine(DrawCards());
        realm.UpdateUI();
	}

	// updates the court and returns it
	public card[] updateCourt()
	{
		try{court [0] = castlePanel.transform.Find("GeneralDropZone").GetComponentInChildren<card> ();} 
		catch{court [0] = null;}
		try{court [1] = castlePanel.transform.Find("MasterOfCoinDropZone").GetComponentInChildren<card>();}
		catch{court [1] = null;}
		try{court [2] = castlePanel.transform.Find("SpyMasterDropZone").GetComponentInChildren<card> ();}
		catch{
			court [2] = null;
		}

		return court;
	}

    public void EndTurn()
    {
        GameObject.Find("EndTurn").GetComponent<Animator>().SetTrigger("Rotate");
        realm.ProcessTurn(); 
		//GetNewEvents();
		realm.SetCitiesCards (cityPanels);
        StartCoroutine(DrawCards());
		OpenEvents();
    }

    public void OpenEvents()
    {
        foreach (Event e in realm.nextTurnEvents)
        {
            Transform newEvent = Instantiate(eventPanelPrefab, GameObject.Find("Map").transform);
            newEvent.Find("Title").GetComponent<Text>().text = e.title;
            newEvent.Find("Description").GetComponent<Text>().text = e.description;
            foreach (string function in e.functions)
            {
                Transform button = Instantiate(eventButtonPrefab, newEvent.Find("Buttons"));
                button.GetComponent<eventButton>().function = e.shortName + function.Replace(" ", "");
                button.Find("Text").GetComponent<Text>().text = function;
            }
        }
        realm.nextTurnEvents = new List<Event>();
    }

    public void GetNewEvents()
    {
		// dont add event if there is already one
		if (realm.nextTurnEvents.Count == 0)
		{
			realm.nextTurnEvents.Add (GetComponent<EventCreator> ().GetEvent ());
		} else
		{
			// there is already an  event going on
		}
	}

    IEnumerator DrawCards()
    {
        while (pickUpZone.childCount < handSize)
        {
            actionData data = ActionCardCreator.DrawCard();
            ActionCard actionCard = Instantiate(actionCardPrefab, GameObject.Find("Canvas").transform).GetComponent<ActionCard>();
            actionCard.GetComponent<Animator>().SetTrigger("card" + pickUpZone.childCount.ToString());
            yield return new WaitForSeconds(0.2f + pickUpZone.childCount * 0.1f);
            actionCard.transform.SetParent(pickUpZone);
            actionCard.GetComponent<Animator>().enabled = false;
            actionCard.GetComponent<Animation>().enabled = false;
            actionCard.data = data;
            actionCard.UpdateCard();
        }
    }
}
