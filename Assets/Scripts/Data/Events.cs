using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class Events
{

    /*
	 * KIlls random court member
	 */
    public static void Accident()
    {
        /*
		 * Accident event
		 */
        bool startState = false;
        RectTransform castlePanel = Helper.GetCastlePanel();
        if (castlePanel.gameObject)
            startState = castlePanel.gameObject.activeSelf;
        castlePanel.gameObject.SetActive(true);
        card[] court = GameObject.Find("GameManager").GetComponent<GameManager>().updateCourt();

        List<int> targets = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            if (court[i])
            {
                targets.Add(i);
            }
            else
            {
                // court member is rip
            }

        }
        // if anyone in the court alive, slay!
        if (targets.Count > 0)
        {
            int targetIndex = targets[UnityEngine.Random.Range(0, targets.Count)];
            Actions.Kill(court[targetIndex].charPanel.gameObject);

        }
        else
        {
            //no one to 'accidently' kill
        }

        castlePanel.gameObject.SetActive(startState);
    }
    public static void AccidentProbablyjustanaccident()
    {
        Accident();
    }

    public static void AccidentInvestigatefurther()
    {
        Accident();
        Helper.GetRealm().realmLoyalty += 5;
    }
    /*
	 * Burning city event
	 */
    /*
	A random city burns, and loses half of its population.
	the value is a place holder, consider changing it sometime.
	*/
    public static void BurningCity()
    {
        Helper.GetRealm().cities[UnityEngine.Random.Range(0, 3)].population /= 2;
    }
    public static void BurnningCitySendthearmy()
    {
        //rip army
       Helper.GetRealm().loyalArmy /= 2;
    }

    public static void BurnningCityRebuildwithmoney()
    {
        // rip money
        Helper.GetRealm().gold = (long)(Helper.GetRealm().gold * 0.75f);
    }

    public static void BurnningCityCryinthecorner()
    {
        BurningCity();
    }
    // bad luck investigate
    public static void BadLuckInvestigate()
    {
        // damn money bag
        if (UnityEngine.Random.Range(0, 100) > 95)
            Helper.GetRealm().gold += 99999;
        else //or nothing
            Helper.GetRealm().gold -= 200;
    }

    public static void BadLuckMournhim()
    {
        Helper.GetRealm().gold -= 200;
        Helper.GetRealm().realmLoyalty += 5;
    }

    public static void BadLuckIneverlikedbabiesanyway()
    {
        Helper.GetRealm().realmLoyalty -= 5;
    }
    // feast event
    public static void FeastFirethecoinmaster()
    {
        Helper.GetRealm().gold /= 2;
        RectTransform castlePanel = Helper.GetCastlePanel();
        bool state = castlePanel.gameObject.activeSelf;
        castlePanel.gameObject.SetActive(true);
        // update the court variable of the game manager. ( get newest court)
        card coinMaster = Helper.GetCoinMaster();
        // check if there is a coin master
        if (coinMaster)
        {
            Actions.Kill(coinMaster.charPanel.gameObject);
        }
        else
        {
            Debug.Log("No coin master to kill, event shouldnt have been drawn.");
            // no coin master
        }
        castlePanel.gameObject.SetActive(state);
    }

    public static void FeastLetsenjoythemealatleast()
    {
        Helper.GetRealm().gold /= 2;
        Helper.GetRealm().realmLoyalty -= 10;
    }

    /*
	A ceremony is taking place.
	lose gold and army.
	*/
    public static void CeremonyOhwell()
    {
        Helper.GetRealm().loyalArmy /=2;
        Helper.GetRealm().gold = (int)(0.8f * Helper.GetRealm().gold);
    }

    // rebeliion
    public static void RebellionRumorsDamnTraitors()
    {
        Helper.AddEvent("RebellionRumorsDamnTraitors", 1);
    }
    public static void RebellionRumorsButImGood()
    {
        Helper.AddEvent("RebellionRumorsButImGood", 1);
    }

    public static void RebellionRumorsYes()
    {
        Helper.AddEvent("RebellionRumorsYes", 2);
    }

    public static void RebellionRumorsNo()
    {
        Helper.AddEvent("RebellionRumorsNo", 2);
    }
    public static void RebellionRumorsExecuteHim()
    {
        card[] court = Helper.GetCourt();
        if (court[0])
        {
            Actions.Kill(court[0].charPanel.gameObject);
        }
        else
        {
            // no general to kill
        }
        Helper.AddEvent("RebellionRumorsExecuteHim", 3);
    }
    public static void RebellionRumorsSpareHim()
    {
        Helper.AddEvent("RebellionRumorsSpareHim", 3);
    }
    public static void RebellionRumorsLetsParty()
    {
        // event chain done. well done.
        Helper.GetRealm().gold += 10000;
        Helper.GetRealm().loyalArmy += 1000;
    }

    public static void RevDesireGreatJob()
    {

    }

    public static void RevAllAmazing()
    {

    }

    public static void RevTraitsNice()
    {

    }
    public static void RevTraitOk()
    {

    }

    public static void RevNoneBadluck()
    {
    }
    public static void RevNoneSpytheSpyMaster()
    {

    }
    /*
		A golden age arrived, increasing growth by 20%
		the value is a place holder, consider changing it sometime.
	*/
    public static void GoldenAgePraisetherealm()
    {
        foreach (City city in Helper.GetCities())
        {
            city.EnterGoldenAge(10);
        }
    }
    /*
		Some bad stuff happend, you should probably be sad
		--TODO--
	*/
    public static void BadLuck()
    {
        Helper.GetRealm().gold -= 200;
    }
    /*
		Spend alot of money on a lavish feast.
		The people are angry.
	*/
    public static void Feast()
    {
        Helper.GetRealm().gold = (int)(0.8f * Helper.GetRealm().gold);
        Helper.GetRealm().realmLoyalty -= 5;
    }
    /*
		make sure that there is a general
		Attacks a city, the leader of the attack is the general.
		If the attacker is strong enough, the city is taken, and its loyalty restored.
		else, the player loses and all loyalty is lost.
	*/
    public static void AttackACity(City target)
    {
        // Obvioulsy this try catch block should not be here, but it is a jam.

        card generalCard = Helper.GetGeneral();

        if (generalCard != null)
        {
            peopleData general = generalCard.character.data;
			// successfull attack
            if ((general.getStats(true).talent) * (0.01f) * Helper.GetRealm().loyalArmy >= target.army)
            {
                target.loyalty = 100;
                Helper.GetRealm().loyalArmy -= target.army;
                target.army = 0;
				// kill the traitor mayor
				if (target.cards [0])
					Actions.Kill (target.cards [0].charPanel.gameObject);
            }
            // failed attack
            else
            {
                target.loyalty = 0;
                Helper.GetRealm().loyalArmy -= target.army;
                target.army = target.army - (int)(Helper.GetRealm().loyalArmy * 0.01f * general.getStats(true).talent);
            }
        }
        else
        {
            // should actually not allow to place the card
            Debug.Log("there is no general you dumb");
        }
    }
}

