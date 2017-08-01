using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debug : MonoBehaviour
{
	public void UpdateCardPanel()
	{
        transform.parent.GetComponent<characterPanel> ().card.character.data.DebugReveal ();
		transform.parent.GetComponent<characterPanel> ().card.character.UpdatePanel (transform.parent.GetComponent<RectTransform>());
	}
}
