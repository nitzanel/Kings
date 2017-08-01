using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// DONT TOUCH
public enum Desires {HelpThePoeple, KeepThePeace, DestroyCorruption, LoyaltyToTheCrown, NothingAtAll, Power, SeeTheWorldBurn,
     GetRich}
public enum NiceDesires { HelpThePoeple, KeepThePeace, DestroyCorruption, LoyaltyToTheCrown, NothingAtAll }

public struct barStats
{
    public int loyalty;
    public int charisma;
    public int talent;
    public barStats(int loy, int chari, int tal)
    {
        loyalty = loy;
        charisma = chari;
        talent = tal;
    }
}
public class peopleData
{
    // consts
    const int MaxNumOfTraits = 3;

	public int location;
    public string name;

    private int loyalty;

    private int charisma; // To keep city loyal
    private int talent; // To create more resorces out of a city/job

	public void ModifyCharisma(int value)
	{
		if (value > 0)
		{
			if (100 - this.charisma > value)
			{
				this.charisma += value;
			} else
			{
				this.charisma= 100;
			}
		} 
		else
		{
			if (this.charisma+ value > 0)
			{
				this.charisma+= value;
			} 
			else
			{
				this.charisma = 0;
			}
		}
	}
	public void ModifyTalent(int value)
	{
		if (value > 0)
		{
			if (100 - this.talent > value)
			{
				this.talent += value;
			} else
			{
				this.talent= 100;
			}
		} 
		else
		{
			if (this.talent+ value > 0)
			{
				this.talent+= value;
			} 
			else
			{
				this.talent = 0;
			}
		}
	}


	public void ModifyLoyalty(int value)
	{
		if (value > 0)
		{
			if (100 - this.loyalty > value)
			{
				this.loyalty += value;
			} else
			{
				this.loyalty = 100;
			}
		} 
		else
		{
			if (this.loyalty + value > 0)
			{
				this.loyalty += value;
			} 
			else
			{
				this.loyalty = 0;
			}
		}

	}

    public trait[] traits = new trait[MaxNumOfTraits]; // First thing the character will roll, will determine his goals and stats
    static System.Random rand = new System.Random();
    // Desires
    public Desires secretDesire;
    public NiceDesires knownDesire;
    public bool secretRevealed;

	public peopleData()
    {
        // init the other vars
        location = 0;
        name = "";
        // The rng
        // pick number of traits and the traits themselves
        setupTraitList(rand);
        // pick known desire and secret desire
        setupDesires(rand);
        // set the stats
        setupStats(rand);
        // You still dont know the real desire
        secretRevealed = false;
    }
    public void setupTraitList(System.Random rand)
    {
        int trueAmount = rand.Next(MaxNumOfTraits) + 1;
        List<ListOfTraits> traitList=new List<ListOfTraits> { ListOfTraits.Coward, ListOfTraits.Brave, ListOfTraits.Lazy, ListOfTraits.Diligent, ListOfTraits.Cruel, ListOfTraits.Kind,
            ListOfTraits.Cunning, ListOfTraits.Honest, ListOfTraits.greedy, ListOfTraits.modest, ListOfTraits.Psychopath};
        for (int i = 0; i < MaxNumOfTraits; i++)
        {
            if (i < trueAmount)
            {
                // Choose the next trait
                ListOfTraits newTrait = traitList[rand.Next(traitList.Count)];
                traitList.Remove(newTrait);
                if (newTrait != ListOfTraits.Psychopath)
                {
                    // Remove both of the conflicting traits
                    if ((int)newTrait % 2 == 0)
                    {
                        traitList.Remove(newTrait + 1);
                    }
                    else
                    {
                        traitList.Remove(newTrait-1);
                    }
                }
                // Add the new trait
                traits[i] = new trait(newTrait);
            }
            else
            {
                // No more traits
                traits[i] = new trait(ListOfTraits.None);
            }
        }
    }
    public void setupDesires(System.Random rand)
    {
        // may change
        int baseBadPercent = 70;
        for (int i = 0; i < MaxNumOfTraits; i++)
        {
            switch (traits[i].theTrait)
            {
                case ListOfTraits.Psychopath:
                    // BURN BABY, BURN
                    secretDesire = Desires.SeeTheWorldBurn;
                    // A masking desire
                    System.Array arr = System.Enum.GetValues(typeof(NiceDesires));
                    knownDesire = (NiceDesires)arr.GetValue(rand.Next(arr.GetLength(0)));
                    // We can stop here
                    return;
                case ListOfTraits.Cruel:
                    baseBadPercent += 10;
                    break;
                case ListOfTraits.Kind:
                    baseBadPercent -= 10;
                    break;
                case ListOfTraits.Cunning:
                    baseBadPercent -= 15;
                    break;
                case ListOfTraits.Honest:
                    baseBadPercent -= 15;
                    break;
                case ListOfTraits.greedy:
                    baseBadPercent += 5;
                    break;
                case ListOfTraits.modest:
                    baseBadPercent -= 5;
                    break;
                default:
                    break;
            }
        }
        // Now we know the chance to break bad, lets roll
        if (rand.Next(100) < baseBadPercent)
        {
            // BREAK BAD
            int toPower = 40;
            int toGreed = 40;
            int toPurge = 20;
            // Adjust the chances based on the traits
            for (int i = 0; i < MaxNumOfTraits; i++)
            {
                switch (traits[i].theTrait)
                {
                    case ListOfTraits.Cunning:
                        toPower += 20;
                        toGreed -= 10;
                        toPurge -= 10;
                        break;
                    case ListOfTraits.Cruel:
                        toPurge += 20;
                        toPower -= 10;
                        toGreed -= 10;
                        break;
                    case ListOfTraits.greedy:
                        toGreed += 20;
                        toPower -= 10;
                        toPurge -= 10;
                        break;
                    default:
                        break;
                }
            }
            // Choose ambition
            int choice = rand.Next(100);
            if (choice < toPurge)
                secretDesire = Desires.SeeTheWorldBurn;
            else if (choice < toPurge + toGreed)
                secretDesire = Desires.GetRich;
            else
                secretDesire = Desires.Power;
            // Get a fake desire
            System.Array arr = System.Enum.GetValues(typeof(NiceDesires));
            knownDesire = (NiceDesires)arr.GetValue(rand.Next(arr.GetLength(0)));
        }
        else
        {
            // DONT BREAK BAD
            System.Array arr = System.Enum.GetValues(typeof(NiceDesires));
            knownDesire = (NiceDesires)arr.GetValue(rand.Next(arr.GetLength(0)));
            // DONT TOUCH THE ENUMS
            secretDesire = (Desires)(int)knownDesire;
        }
    }
    public void setupStats(System.Random rand)
    {
        int RegularMean = 80;
        double StdDev = 10;
        // Open to changes
        // Maybe go down each turn
        loyalty = randNormal(RegularMean, StdDev, rand);
        charisma = randNormal(RegularMean-10, StdDev, rand);
        talent = randNormal(RegularMean-10, StdDev, rand);
    }
    public int randNormal(int mean, double stdDev, System.Random rand)
    {

        double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0 - rand.NextDouble();
        double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) *
                     System.Math.Sin(2.0 * System.Math.PI * u2); //random normal(0,1)
        return System.Math.Max(System.Math.Min((int) (mean + stdDev * randStdNormal), 100), 0); //random normal(mean,stdDev^2)
    }
    public barStats getStats(bool trueStats)
    {
        // We will later forward those as barStats
        int sumLoy = loyalty;
        int sumChar = charisma;
        int sumTal = talent;

        foreach (trait t in traits)
        {
            // only count the revealed traits in the shown bars
            if (trueStats||(t.isRevealed))
            {
                // Open to changes
                switch (t.theTrait)
                {
                    case ListOfTraits.Brave:
                        sumChar += 10;
                        sumTal += 10;
                        break;
                    case ListOfTraits.Coward:
                        sumChar -= 10;
                        sumTal -= 10;
                        break;
                    case ListOfTraits.Cruel:
                        sumLoy -= 5;
                        sumChar -= 15;
                        sumTal += 5;
                        break;
                    case ListOfTraits.Kind:
                        sumLoy += 5;
                        sumChar += 15;
                        sumTal -= 5;
                        break;
                    case ListOfTraits.Diligent:
                        sumTal += 20;
                        sumChar += 5;
                        break;
                    case ListOfTraits.Lazy:
                        sumTal -= 20;
                        sumChar -= 5;
                        break;
                    case ListOfTraits.Cunning:
                        sumLoy -= 15;
                        sumChar += 5;
                        break;
                    case ListOfTraits.Honest:
                        sumLoy += 5;
                        sumChar += 10;
                        break;
                    case ListOfTraits.greedy:
                        sumLoy -= 10;
                        sumTal -= 10;
                        sumChar += 5;
                        break;
                    case ListOfTraits.modest:
                        sumLoy += 10;
                        sumTal += 10;
                        sumChar += 5;
                        break;
                    case ListOfTraits.Psychopath:
                        sumLoy -= 20;
                        sumTal += 15;
                        sumChar += 15;
                        break;
                    default:
                        break;
                }
            }
        }
        // change stats based on desires
        if ((trueStats)||(secretRevealed))
        {
            switch (secretDesire)
            {
                case Desires.DestroyCorruption:
                    sumLoy += 5;
                    sumTal += 10;
                    break;
                case Desires.HelpThePoeple:
                    sumChar += 15;
                    break;
                case Desires.LoyaltyToTheCrown:
                    sumLoy += 15;
                    break;
                case Desires.KeepThePeace:
                    sumLoy += 5;
                    sumChar += 10;
                    break;
                case Desires.GetRich:
                    sumLoy -= 5;
                    sumTal -= 10;
                    break;
                case Desires.Power:
                    sumLoy -= 15;
                    sumChar += 5;
                    sumTal += 5;
                    break;
                case Desires.SeeTheWorldBurn:
                    sumLoy -= 20;
                    sumChar -= 5;
                    sumTal += 15;
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (knownDesire)
            {
                case NiceDesires.DestroyCorruption:
                    sumLoy += 5;
                    sumTal += 10;
                    break;
                case NiceDesires.HelpThePoeple:
                    sumChar += 15;
                    break;
                case NiceDesires.LoyaltyToTheCrown:
                    sumLoy += 15;
                    break;
                case NiceDesires.KeepThePeace:
                    sumLoy += 5;
                    sumChar += 10;
                    break;
                default:
                    break;
            }
        }
        // max is 100, min is 0
        sumLoy = System.Math.Min(System.Math.Max(sumLoy, 0), 100);
        sumChar = System.Math.Min(System.Math.Max(sumChar, 0), 100);
        sumTal = System.Math.Min(System.Math.Max(sumTal, 0), 100);
        return new barStats(sumLoy, sumChar, sumTal);
    }
    public void DebugReveal()
    {
        foreach (trait t in traits)
        {
            t.isRevealed = true;
        }
        secretRevealed = true;
    }
    public void addToBaseStats(barStats change)
    {
        // Update the stats
        loyalty += change.loyalty;
        charisma += change.charisma;
        talent += change.talent;
    }
}
