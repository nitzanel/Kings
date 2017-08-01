using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class invitePeople : MonoBehaviour
{
    public int cost = 10000;

    public GameObject cardPrefab;

    public void InvitePeople()
    {
        drop dropZone = transform.parent.Find("DropZone").GetComponent<drop>();
        Realm realm = GameObject.Find("GameManager").GetComponent<GameManager>().realm;

        if (dropZone.transform.childCount < dropZone.max && cost <= realm.gold)
        {
            realm.gold -= cost;
            realm.UpdateUI();
            GameObject card = Instantiate(cardPrefab, GameObject.Find("Canvas").transform);
            card.transform.SetParent(dropZone.transform);
        }
    }
}
