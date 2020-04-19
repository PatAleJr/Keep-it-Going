using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource[] cheerSources;
    [SerializeField]
    private AudioSource clapSource;
    [SerializeField]
    private AudioSource[] booSources;
    
    [SerializeField]
    private AudioClip[] cheerClips;
    [SerializeField]
    private AudioClip clapClip;
    [SerializeField]
    private AudioClip[] booClips;

    public bool cheer = false;
    public bool clap = true;

    void Start()
    {
        clapSource.clip = clapClip;
        clapSource.Play();
    }

    void Update()
    {
        if (cheer)
        {
            for (int i = 0; i < cheerSources.Length; i++)
            {
                if (!cheerSources[i].isPlaying)
                {
                    playCheer(i);
                }
            }
        }
    }

    public void Lost()
    {
        clap = false;
        cheer = false;
        clapSource.Stop();

        //Boo here
        foreach (AudioSource source in booSources)
        {
            source.clip = booClips[Random.Range(0, booSources.Length)];
            source.pitch = Random.Range(0.8f, 1.2f);
            source.volume = Random.Range(0.8f, 1.2f);
            source.Play();
        }
    }

    public void resetGame()
    {
        clap = true;
        cheer = false;
        clapSource.Play();
    }

    public void gameStart()
    {
        clap = true;
        cheer = true;
    }

    void playCheer(int source)
    {
        cheerSources[source].clip = cheerClips[Random.Range(0, cheerClips.Length)];
        cheerSources[source].pitch = Random.Range(0.8f, 1.2f);
        cheerSources[source].volume = Random.Range(0.7f, 1f);
        cheerSources[source].Play();
    }
}
