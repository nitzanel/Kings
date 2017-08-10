using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailBox : MonoBehaviour {

	public GameObject Message;
	
    public void CleanMessages()
    {
        foreach (MessageController message in transform.Find("MessagesParent").GetComponentsInChildren<MessageController>())
        {
            if(!message.inUse)
                message.Shrink();
        }
    }

}
