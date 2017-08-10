using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// this shouldnt be Monobehaviour, but some changes are required.
public class nameGenerator : MonoBehaviour
{
    char[] consonant = { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z' };
    char[] vowel = { 'a', 'e', 'i', 'o', 'u' };

    const int ConLen = 21;
    const int VowLen = 5;

    void Awake()
    {
        GetComponent<Text>().text = GetName() + " " + GetName();
    }

    public string GetName()
    {
        int length = Random.Range(5, 10);
        char[] name = new char[length];

        for (int i = 0; i < length; i++)
        {
            if (i == 0)
            {
                name[i] = consonant[Random.Range(0, ConLen)];
            }
            else
            {
                if (isCon(name[i - 1], consonant) != 0)
                {
                    if (name[i - 1] == 'c' || name[i - 1] == 's')
                    {
                        if (Random.Range(1, 3) > 1)
                        {
                            name[i] = 'h';
                        }
                        else
                        {
                            name[i] = vowel[Random.Range(0, VowLen)];
                        }
                    }
                    else
                    {
                        name[i] = vowel[Random.Range(0, VowLen)];
                    }
                }
                else
                {
                    if (name[i - 1] == 'e' && name[i - 2] != 'e' && Random.Range(1, 3) > 1)
                    {
                        name[i] = vowel[Random.Range(0, 2)];
                    }
                    else if (name[i - 1] == 'o' && name[i - 2] != 'o' && Random.Range(1, 3) > 1)
                    {
                        name[i] = vowel[Random.Range(3, 5)];
                    }
                    else
                    {
                        name[i] = consonant[Random.Range(0, ConLen)];
                    }
                }
            }
        }

        return new string(name);
    }

    int isCon(char letter, char[] consonant)
    {
        int i;

        for (i = 0; i < ConLen; i++)
        {
            if (letter == consonant[i]) return 1;
        }
        return 0;
    }
}
