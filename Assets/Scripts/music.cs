using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class music : MonoBehaviour
{
    public AudioSource player;
    public AudioClip[] clips;

    int index = 0;

    static music instance;

    void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this);
    }

    void Start ()
    {
        player.clip = clips[0];
        player.loop = false;
        player.Play();
        StartCoroutine(WaitForTrackToEnd());
	}

    IEnumerator WaitForTrackToEnd()
    {
        while(true)
        {
            while (player.isPlaying)
                yield return new WaitForSeconds(0.01f);

            index++;
            if (index == clips.Length)
                index = 0;
            player.clip = clips[index];
            player.Play();
        }
    }
}
