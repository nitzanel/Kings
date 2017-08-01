using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bringUpPeoplePanel : MonoBehaviour
{
    RectTransform characterBank;
    float initialY;
    float t = 0;
    bool goingUp = false;
    bool goingDown = false;

	void Start ()
    {
        characterBank = GameObject.Find("CharacterBank").GetComponent<RectTransform>();
        initialY = characterBank.anchoredPosition.y;
        characterBank.anchoredPosition = new Vector2(characterBank.anchoredPosition.x, initialY - characterBank.rect.height);
	}

    void Update()
    {
        if (goingDown)
        {
            characterBank.anchoredPosition = Vector2.Lerp(characterBank.anchoredPosition, new Vector2(characterBank.anchoredPosition.x, initialY - characterBank.rect.height), t);
            t += Time.deltaTime;
            if (t >= 1)
            {
                t = 0;
                goingDown = false;
            }
        }
        if (goingUp)
        {
            characterBank.anchoredPosition = Vector2.Lerp(characterBank.anchoredPosition, new Vector2(characterBank.anchoredPosition.x, initialY), t);
            t += Time.deltaTime;
            if (t >= 1)
            {
                t = 0;
                goingUp = false;
            }
        }
    }

    public void BringUp()
    {
        if (Mathf.Abs(characterBank.anchoredPosition.y - initialY) <= 0.001f)
        {
            t = 0;
            goingUp = false;
            goingDown = true;
        }
        else if (Mathf.Abs(characterBank.anchoredPosition.y - initialY + characterBank.rect.height) <= 0.001f)
        {
            t = 0;
            goingDown = false;
            goingUp = true;
        }
    }
}
