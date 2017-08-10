using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


/*
 * Actions implantations.
 * Invoked from Realm class.
 * Actions should match the action card function name in actions.txt
 */
public class Actions
{


    public static void Imprison(GameObject obj)
    {
        // Move character to prison dropzone, and set it to be inactive
        card prisoner = obj.GetComponent<characterPanel>().card;
        // remove from court
        Helper.removeFromCourt(prisoner);
        prisoner.Transfer(Helper.GetPrisonDropZone());
        // Decrease everyone's loyalty
        Helper.ModifyAllLoyalty(-5);
        // deactivate  --TODO-- RETHINK
        // Nitzan: no need to, because you cant target them anyway, and by keeping them active you can use them in events and stuff
       // if (obj.activeSelf)
        ///    obj.SetActive(false);
    }
    /*
		Action that kills a character.
		destroy both the card object, and the card panel.
	*/
    public static void Kill(GameObject obj)
    {
        // card object to destroy
        GameObject toDestroy;
        toDestroy = obj.GetComponent<characterPanel>().card.gameObject;
        // Decrease everyone's loyalty
        Helper.ModifyAllLoyalty(-10);
        Helper.removeFromCourt(obj.GetComponent<characterPanel>().card);
        /// NULL FUCKING REFERENCE WHEN YOU TRY TO KILL SOMEONE --FIXED--
        obj.GetComponent<characterPanel>().card.ExitCurrent();
        GameObject.Destroy(obj);
        GameObject.Destroy(toDestroy);

    }

	const int BribeCost = 200;
	public static bool BribeTest (GameObject obj)
	{
		// yes -- possible exploit with many bribe cards.
		if (Helper.GetRealm ().gold >= 200) 
		{
			return true;
		}
		return false;
	}
    /*
		Gives a Bribe to a character, inceases loyalty to you.
		The price is a placeholder, and should probably be changed.
	*/
    public static void Bribe(GameObject obj)
    {
        peopleData charData=Helper.GetTargetPeople(obj);
        if(charData.secretDesire==Desires.DestroyCorruption)
        {
            // rip king
            charData.ModifyLoyalty(-20);
            Helper.AddMessage("Bribe failure", charData.name + " snaps at you after your attempt to bribe him!");
            return;
        }
        if(charData.secretDesire==Desires.GetRich)
        {
            // nice dude
            charData.ModifyLoyalty(30);
            Helper.AddMessage("Bribe major success", charData.name + " really liked the bribe, and his loyalty was greatly increased!");
            return;
        }
        int successChance = 80;
        if((!charData.secretRevealed)&&(charData.knownDesire==NiceDesires.DestroyCorruption))
        {
            // He "doesnt" like corruption
            successChance = 10;
        }
        foreach (trait t in charData.traits)
        {
            if(t.theTrait==ListOfTraits.greedy)
            {
                successChance += 10;
                break;
            }
            if(t.theTrait==ListOfTraits.modest)
            {
                successChance -= 30;
                break;
            }
        }

        if(UnityEngine.Random.Range(0,100)<successChance)
        {
            // Successfully bribed
            if(UnityEngine.Random.Range(0,80)<successChance/2)
            {
                // mega success
                charData.ModifyLoyalty(30);
                Helper.AddMessage("Bribe major success", charData.name + " really liked the bribe, and his loyalty was greatly increased!");
            }
            else
            {
                // mega success
                charData.ModifyLoyalty(20);
                Helper.AddMessage("Bribe success", charData.name + " accepted the bribe, and his loyalty was increased!");
            }
        }
        else
        {
            // FAIL
            charData.ModifyLoyalty(-20);
            Helper.AddMessage("Bribe failure", charData.name + " snaps at you after your attempt to bribe him!");
        }
    }

	/*
	 * Check if there is a spy master
	 */
	public static bool SpyTest(GameObject obj)
	{
		if (Helper.GetSpyMaster ())
			return true;
		return false;
	}
    /*
		Spies on a target.
		Reveals all the information on the target (Debug Reveal)
	*/
    public static void Spy(GameObject obj)
    {
        peopleData target = Helper.GetTargetPeople(obj);
        barStats spyMasterStats = Helper.GetSpyMaster().character.data.getStats(true);
        // check how it went
        switch (City.rollResult(spyMasterStats.loyalty, (spyMasterStats.talent + spyMasterStats.charisma) / 2, 10, 95))
        {
            case Result.Sabotage:

            case Result.MajorSabotage:

            case Result.MajorFailure:
                // Fail!!
                Helper.AddMessage("Reveal Failed", "Your spy failed! this will greatly lower "+target.name+"'s loyalty!");
                target.ModifyLoyalty(-20);
                break;
            case Result.FailedSabotage:
            case Result.Failure:
                // Found nothing
                Helper.AddMessage("Revealed Nothing", "Unfortunatly Your spy could not find anything, or did he?");
                break;
            case Result.Nothing:
                // Found 1 trait
                foreach (trait t in target.traits)
                {
                    if (!t.isRevealed)
                    {
                        // We have found unknown trait
                        t.isRevealed = true;
                        break;
                    }
                }
                Helper.AddMessage("Revealed a Trait", "The spy could only reveal one trait of " + target.name + ", unfortunatly...");
                break;
            case Result.Success:
                // desire/ all traits
                if (target.secretRevealed)
                {
                    // Stuff about revealing traits
                    target.revealTraits();
                    Helper.AddMessage("Revealed All Traits", "The spy successfully discovered all the traits of " + target.name);
                }
                else
                {
                    // Stuff about reealing desire
                    target.secretRevealed = true;
                    Helper.AddMessage("Revealed Desire", "The spy successfully discovered " + target.name + "'s secret desire!");
                }
                break;
            case Result.MajorSuccess:
                // everything!
                target.DebugReveal();
                Helper.AddMessage("Revealed Everything!", "The spy successfully discovered everything we need to know about " + target.name);
                break;
            default:
                // WutFace
                break;
        }
    }

	/*
	 * Check if there is a coin master
	 */
	public static bool RaiseTaxesTest(GameObject obj)
	{
		if (Helper.GetCoinMaster ())
			return true;
		return false;
	}
    /*
		Raises taxes in the city target for the rest of the game.
		(Maybe it shouldnt be for the rest of the game)
		the value is a place holder, consider changing it sometime.
	*/
    public static void RaiseTaxes(GameObject obj)
    {
        Helper.GetTargetCity(obj).tax += 0.2f;
    }
    /*
		Increase Charisma of a target character.
		the value is a place holder, consider changing it sometime.
	*/
    public static void IncreaseCharisma(GameObject obj)
    {
        Helper.GetTargetPeople(obj).ModifyCharisma(20);
    }

    /*
		Increase Charisma of a target character.
		the value is a place holder, consider changing it sometime.
	*/
    public static void IncreaseTalent(GameObject obj)
    {
        Helper.GetTargetPeople(obj).ModifyTalent(20);
    }

	/*
	 * Check that there is a general, and an army (atleast 1)
	 */
	public static bool AttackCityTest(GameObject obj)
	{
		if (Helper.GetGeneral () && Helper.GetRealm().loyalArmy > 0)
			return true;
		return false;
	}
	/*
		Attacks a city target.
		Creates an event of a city attack.
	*/
    public static void AttackCity(GameObject obj)
    {
        //Debug.Log("Attacked: " + Helper.GetTargetCity(obj).name);
        // stupid call to events, --TODO-- change
        Events.AttackACity(Helper.GetTargetCity(obj));
    }

	/*
	 * Check that there is a general
	 */ 
	public static bool CallToArmsTest(GameObject obj)
	{
		if (Helper.GetGeneral ())
			return true;
		return false;
	}
    /*
		Call to arms people in the city, increasing the levy tax, 
		and brings the player more army from the city.
		the value is a place holder, consider changing it sometime.
	*/
    public static void CallToArms(GameObject obj)
    {
        City city = Helper.GetTargetCity(obj);
        Helper.GetRealm().loyalArmy += city.army / 2;
        city.UpdateReportText("Action used: Call To Arms - Took " + city.army / 2 + " army units.\n");
        city.army /= 2;
    }

	/*
	 * Check that there is a general
	 */
	public static bool RecruitArmyTest(GameObject obj)
	{
		if (Helper.GetGeneral ())
			return true;
		return false;
	}
    /*
		Recruit army in the city, increasing the amout of soldiers it will have.

	*/
    public static void RecruitArmy(GameObject obj)
    {
        Helper.GetTargetCity(obj).millateristic += 20;
        Helper.GetTargetCity(obj).UpdateReportText("Action used: Recruit Army - millateristic increased by 20%\n");

    }
    /*
		Increase the loyalty of a city.
		the value is a place holder, consider changing it sometime.
	*/
    public static void IncreaseCityLoyalty(GameObject obj)
    {
        Helper.GetTargetCity(obj).loyalty += 20;
        Helper.GetTargetCity(obj).UpdateReportText("Action used: Increase Loyalty - loyalty increased by 20%\n");

    }
    /*
		Increase the growth of the city.
		the value is a place holder, consider changing it sometime.
	*/
    public static void IncreaseCityPopulation(GameObject obj)
    {
        Helper.GetTargetCity(obj).growth += 20;
        Helper.GetTargetCity(obj).UpdateReportText("Action used: Increase Population - growth increased by 20%\n");

    }

	/*
	 * Check that atleast one of the court member is present.
	 */
	public static bool ReplaceCourtTest(GameObject obj)
	{
		if (Helper.GetGeneral () || Helper.GetCoinMaster () || Helper.GetSpyMaster ())
			return true;
		return false;
	}
    /*
		Kills everyone in the court, then finds replacements for them.
		--TODO--
	*/
    public static void ReplaceCourt(GameObject obj)
    {
		foreach (card character in Helper.GetCourt())  
		{
			if (character)
				Kill (character.charPanel.gameObject);
		}
    }


}

