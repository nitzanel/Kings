using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTop : MonoBehaviour
{
	void Update ()
    {
        transform.SetAsLastSibling();
	}
}
