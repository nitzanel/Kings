using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttons : MonoBehaviour
{
    public void NewGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("main");
        //Application.LoadLevel("main");
    }

    public void Menu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("menu");
        //Application.LoadLevel("menu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
