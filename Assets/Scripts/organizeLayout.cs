using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class organizeLayout : MonoBehaviour
{
	void Awake ()
    {
        //set the layout properties in the card bank to fit exactly 5 (cardCount) cards
        int cardCount = GetComponent<drop>().max;
        RectTransform rect = GetComponent<RectTransform>();
        GridLayoutGroup layout = GetComponent<GridLayoutGroup>();
        layout.cellSize = new Vector2(rect.rect.width / cardCount, rect.rect.height);
	}
}
