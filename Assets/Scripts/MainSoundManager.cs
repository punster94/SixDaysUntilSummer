using UnityEngine;
using System.Collections;

public class MainSoundManager : MonoBehaviour
{
    public AudioSource backgroundSoundPlayer;
    public AudioSource soundEffectPlayer;

    public AudioClip[] backgroundMusic;
    public AudioClip[] soundEffects;

	void Start ()
    {
        playHallwayMusic();
	}
	
	void Update ()
    {
	
	}

    public void playMainBackgroundMusic()
    {
        playNewBackgroundMusic(0);
    }

    public void playMinigameMusic()
    {
        playNewBackgroundMusic(1);
    }

    public void playAccusationMusic()
    {
        playNewBackgroundMusic(2);
    }

    public void playEndScreenMusic()
    {
        playNewBackgroundMusic(3);
    }

    public void playHallwayMusic()
    {
        playNewBackgroundMusic(4);
    }

    public void playMorningSound()
    {
        playNewSoundEffect(0);
        playMainBackgroundMusic();
    }

    public void playLunchSound()
    {
        playNewSoundEffect(1);
    }

    public void playAfternoonSound()
    {
        playNewSoundEffect(2);
    }

    public void playDateSound()
    {
        playNewSoundEffect(3);
    }

    public void playLPPlusSound()
    {
        playNewSoundEffect(4);
    }

    public void playLPZeroSound()
    {
        playNewSoundEffect(5);
    }

    private void playNewBackgroundMusic(int index)
    {
        backgroundSoundPlayer.Stop();
        backgroundSoundPlayer.clip = backgroundMusic[index];
        backgroundSoundPlayer.Play();
    }

    private void playNewSoundEffect(int index)
    {
        soundEffectPlayer.Stop();
        soundEffectPlayer.clip = soundEffects[index];
        soundEffectPlayer.Play();
    }
}
