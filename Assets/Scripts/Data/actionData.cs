using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
public class actionData
{
    public enum Targets { People, City, Castle, Event, All, None };

    public string name;
    public int cost;
	public bool condition;
    public string description;
    public Targets target;
    public string function;
	public string tooltip;

	public bool TestCondition (GameObject target)
	{
		// if there is a condition, check it
		if (condition) 
		{	// invokes a static function that test if this card can be played.
			System.Type actionClassType = System.Type.GetType ("Actions");
			MethodInfo someMethod = actionClassType.GetMethod (function + "Test");
			bool b = (bool)someMethod.Invoke (null, new object [] { target });
			return b;
		}
		// no condition
		else 
		{
			return true;
		}
	}

    public actionData() { }

	public actionData(string Name, int Cost, string Description, Targets Target, string Function, string condition,string tooltip)
    {
        name = Name;
        cost = Cost;
        description = Description;
        target = Target;
        function = Function;
		this.tooltip = tooltip;
		this.condition = false;
		if (condition == "yes")
			this.condition = true;
    }

    public actionData(actionData copy)
    {
        name = copy.name;
        cost = copy.cost;
        description = copy.description;
        target = copy.target;
        function = copy.function;
		condition = copy.condition;
		tooltip = copy.tooltip;
	}
}