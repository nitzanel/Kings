using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttons : MonoBehaviour
{
    public void NewGame()
    {
        Application.LoadLevel("main");
    }

    public void Menu()
    {
        Application.LoadLevel("menu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
