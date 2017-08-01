using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XButton : MonoBehaviour
{
    public void X()
    {
        //close whatever you are in
        transform.parent.gameObject.SetActive(false);
    }
}
