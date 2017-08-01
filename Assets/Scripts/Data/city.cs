using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City
{
	// city's name
	public string name;
	// population in the city
	public int population;
	// % of loyalty to the player in the city.
	public int loyalty;
	// army units the city have
	public int army;
	// 5 % growth every year (default)
	public int growth = 5;
	// 30% millateristic ( % of people in the army)
	public int millateristic = 30;

	public float goldenAgeMod = 1.0f;

	//years of golden age left
	public int goldenAgeYearsLeft = 0;

	// modifier of tax, can be raised with action cards/events
	public float tax = 1.0f;

	public string lastTurnReport="";

	// poeple in the city
	public List<peopleData> people;

	public City (string name, int population,int growth=5, int millateristic=30)
	{
		people = new List<peopleData> ();
		this.name = name;
		this.population = population;
		this.loyalty = 100;
		this.growth = growth;
		this.millateristic = millateristic;
        this.army = 10;
	}

    public Result rollResult(int loyalty, int stat, int minChance, int maxChance)
    {
        if (Random.Range(minChance, maxChance) < loyalty)
        {
            // loyalty success
            if (Random.Range(minChance, maxChance) < stat)
            {
                // Success, try to have major success
                if (Random.Range(0, maxChance) < stat / 5)
                {
                    return Result.MajorSuccess;
                }
                return Result.Success;
            }
            else
            {
                // try to at least to not fail
                if (Random.Range(minChance, maxChance) < stat)
                {
                    // saved, at least
                    return Result.Nothing;
                }
                // Failed, at least no major fail?
                if (Random.Range(minChance, 100) > stat)
                {
                    // oh boy
                    return Result.MajorFailure;
                }
                // Regular fail
                return Result.Failure;
            }
        }
        else
        {
            // loyalty check failed
            // try to sabotage
            if (Random.Range(minChance, maxChance) < stat)
            {
                // Subotage, try to have major subotage
                if (Random.Range(0, maxChance) < stat / 5)
                {
                    return Result.MajorSabotage;
                }
                return Result.Sabotage;
            }
            else
            {
                // sabotage failed
                return Result.FailedSabotage;
            }
        }
    }

    public void AddCharacter(peopleData character) 
	{
		people.Add(character);
	}

	public void RemoveCharacter(peopleData character)
	{
		people.Remove (character);
	}

	// update city stats every turn
	public void UpdateCity(int realmLoyalty)
	{
		// do something --->0.O
		if (goldenAgeYearsLeft > 0)
		{
			goldenAgeYearsLeft--;
			goldenAgeMod = 1.5f;
		} else
		{
			goldenAgeMod = 1.0f;
		}
		UpdatePopulation(this.growth);
		UpdateArmy (this.millateristic);
        UpdateLoyalty(realmLoyalty);
	}

    const int MinLoayalty = 15;
    const int MaxLoayalty = 95;
    const int MinChange = 5;
    // loyalty update
    private void UpdateLoyalty(int realmloyalty)
    {
        UpdateReportText("Loyalty: ");
		loyalty -= MinChange;
		int mayorInfluence = 0;
		if (people.Count > 0)
		{
			peopleData mayor = people [0];
			barStats mayorStats = mayor.getStats (true);
			mayorInfluence = (int)((mayorStats.charisma / 100f) * (mayorStats.talent / 100.0f) * (mayorStats.loyalty - 50f) / 10.0f);
			loyalty += mayorInfluence;
		}
		int result = mayorInfluence - MinChange;
        UpdateReportText(result.ToString() + "\n");
       
        if(loyalty<0)
        {
            // not loyal :)
			loyalty = 0;
        }
        loyalty = Mathf.Min(100, loyalty);
    }
    const int MinLoayaltyPop = 15;
    const int MaxLoayaltyPop = 95;
    // pop growth
    private void UpdatePopulation(int currentGrowth)
	{
        UpdateReportText("Population: ");
        int result = 0;
        int change = (int)(0.01f * currentGrowth * this.population);
        if (people.Count != 0)
        {
            barStats stats = people[0].getStats(true);
            switch (rollResult(stats.loyalty, stats.talent, MinLoayaltyPop, MaxLoayaltyPop))
            {
                case Result.MajorSabotage:
                // Currently the same
                case Result.MajorFailure:
                    result -= change;
                    // call event
                    UpdateReportText("Major failure :( ");
                    break;
                case Result.Sabotage:
                // Currently the same
                case Result.Failure:
                    result -= change / 2;
                    // Call event
                    UpdateReportText("Failure :( ");
                    break;
                case Result.FailedSabotage:
                // Currently the same
                case Result.Nothing:
                    // call event
                    UpdateReportText("Nothing :| ");
                    break;
                case Result.Success:
                    result += change;
                    // call event
                    UpdateReportText("Success :) ");
                    break;
                case Result.MajorSuccess:
                    result += 2 * change;
                    // call event
                    UpdateReportText("Major success :) ");
                    break;
                default:
                    break;
            }
            population += result;
            UpdateReportText(result + "\n");
        }
        else
        {
            // no mayor, chaos?
            UpdateReportText("No Mayor.\n");
        }
        if(population<0)
        {
            // rip city?
            population = 0;
        }
	}

    const int MinLoayaltyArmy = 15;
    const int MaxLoayaltyArmy = 95;
    // army in the city is proportional to city population
    private void UpdateArmy(int currentMillateristic)
	{
        UpdateReportText("Army: ");
        int change = (int)(0.01f * currentMillateristic * this.population);
        int result = 0;
        if (people.Count != 0)
        {
            barStats stats = people[0].getStats(true);
            switch (rollResult(100, stats.talent, MinLoayaltyArmy, MaxLoayaltyArmy))
            {
                case Result.MajorSabotage:
                // Currently the same
                case Result.MajorFailure:
                    result -= change;
                    // call event
                    UpdateReportText("Major failure :( ");
                    break;
                case Result.Sabotage:
                // Currently the same
                case Result.Failure:
                    result -= change / 2;
                    // Call event
                    UpdateReportText("Failure :( ");
                    break;
                case Result.FailedSabotage:
                // Currently the same
                case Result.Nothing:
                    // call event
                    UpdateReportText("Nothing :| ");
                    break;
                case Result.Success:
                    result += change;
                    // call event
                    UpdateReportText("Success :) ");
                    break;
                case Result.MajorSuccess:
                    result += 2 * change;
                    // call event
                    UpdateReportText("Major success :) ");
                    break;
                default:
                    break;
            }
            army += result;
            UpdateReportText(result + "\n");
            if (army > population) army = population;
            else if (army < 0) army = 0;
        }
        else
        {
            // no mayor, chaos? YES
            UpdateReportText("No Mayor.\n");
        }
        if(army<0)
        {
            // rip army?
            army = 0;
        }
    }
	/*
	 * Causes the city to enter a golden age
	 */
	public void EnterGoldenAge(int years)
	{
		goldenAgeYearsLeft += years;
	}
	/*
	 * Updates the report text of the city, that is shown on the city card.
	 */
	public void UpdateReportText(string text)
	{
		this.lastTurnReport += text;
	}
}


