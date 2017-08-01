using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eventButton : MonoBehaviour
{
    public string function;

    public void CallFunction()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().realm.thisTurnEventFunctions.Add(function);
        transform.parent.parent.gameObject.SetActive(false);
        DestroyImmediate(transform.parent.parent.gameObject);
    }
}
