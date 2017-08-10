using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActionCard : MonoBehaviour
{
    public actionData data; //data of the person in the card

    public const drop.DropType type = drop.DropType.Action;

    void Start()
    {
        GetComponent<card>().type = drop.DropType.Action;
    }

    public void UpdateCard()
    {
        transform.Find("Title").GetComponent<Text>().text = data.name;
        transform.Find("Description").GetComponent<Text>().text = data.description;
        transform.Find("Art").GetComponent<Image>().sprite = Resources.Load<Sprite>("ImprovedGraphics/Cards/" + data.function);
        transform.Find("AP").GetComponent<Text>().text = data.cost.ToString();
    }

    public void UpdatePanel(RectTransform panel)
    {
        //to be continued
    }
}
