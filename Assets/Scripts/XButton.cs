using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XButton : MonoBehaviour
{
    public void X()
    {
		// find what you were
		Helper.UpdateCardsTargetMatch(actionData.Targets.None);
		Helper.UpdateCardsCondition (null);
		//close whatever you are in
        transform.parent.gameObject.SetActive(false);
    }
}
