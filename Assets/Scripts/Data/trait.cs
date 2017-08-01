using System.Collections;
using System.Collections.Generic;
public enum ListOfTraits { Coward, Brave, Lazy, Diligent, Cruel, Kind, Cunning, Honest, greedy, modest, Psychopath, None};

public class trait
{
    public ListOfTraits theTrait;
    public bool isRevealed;

    public trait(ListOfTraits trait)
    {
        theTrait = trait;
        isRevealed = false;
    }
}