using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pauseButton : MonoBehaviour
{
    public Transform pauseMenuPrefab;

    public void Pause()
    {
        pauseMenuPrefab.gameObject.SetActive(true);
    }
}
