using UnityEngine;
using System.Collections;

public class DialogSoundManager : MonoBehaviour
{
    public string dialogAudioFolderName;

    private AudioSource dialogPlayer;
    private AudioClip currentClip;

	void Start ()
    {
        dialogPlayer = GetComponent<AudioSource>();
	}
	
	void Update ()
    {
	    
	}

    public void playDialogSound(string fileName)
    {
        dialogPlayer.Stop();
        Resources.UnloadAsset(currentClip);

        if (fileName == null)
            return;

        currentClip = null;
        currentClip = Resources.Load(dialogAudioFolderName + "/" + fileName) as AudioClip;

        if (currentClip == null)
            return;

        dialogPlayer.clip = currentClip;
        dialogPlayer.Play();
    }
}
