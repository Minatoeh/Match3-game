using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public AudioSource[] destryoNoise;
    public AudioSource backgroundMuic;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if(PlayerPrefs.GetInt("Sound") == 0)
            {
                backgroundMuic.Play();
                backgroundMuic.volume = 0;
            }
            else
            {
                backgroundMuic.Play();
                backgroundMuic.volume = 1;
            }
        }
        else
        {
            backgroundMuic.Play();
            backgroundMuic.volume = 1;
        }
    }

    public void AdjustVolume()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                backgroundMuic.volume = 0;
            }
            else
            {
                backgroundMuic.volume = 1;
            }
        }
    }

    public void PlayRandomDestroyNoise()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                //Choose a random number
                int clipToPlay = Random.Range(0, destryoNoise.Length);
                //play that clip
                destryoNoise[clipToPlay].Play();
            }

        }
        else
        {
            //Choose a random number
            int clipToPlay = Random.Range(0, destryoNoise.Length);
            //play that clip
            destryoNoise[clipToPlay].Play();
        }
    }
}
