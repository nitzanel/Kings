using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class actionData
{
    public enum Targets { People, City, Castle, Event, All };

    public string name;
    public int cost;
    public string description;
    public Targets target;
    public string function;

    public actionData() { }

    public actionData(string Name, int Cost, string Description, Targets Target, string Function)
    {
        name = Name;
        cost = Cost;
        description = Description;
        target = Target;
        function = Function;
    }

    public actionData(actionData copy)
    {
        name = copy.name;
        cost = copy.cost;
        description = copy.description;
        target = copy.target;
        function = copy.function;
    }
}