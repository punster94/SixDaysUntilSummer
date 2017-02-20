using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum Transition { Invalid, Intro, Morning, Lunch, Afternoon, DateSelection, Date, Accusation, Ending, MaxTransition }

public class TransitionManager : MonoBehaviour
{
    public GameObject cameraRig;

    public Vector3 startingPosition;

    public float secondsToDisplayTransition;
    public float timeToPlayMinigameInSeconds;

    public GameObject[] transitionImages;

    public GameObject transitionBackgroundImage;

    public MainSoundManager mainSoundManager;

    public Text timerText;

    private Transition currentTransition;

    private int currentTransitionId;

    private float currentTimerTime;

    private bool shouldTickDown;

    void Start()
    {
        shouldTickDown = false;
    }

    void Update()
    {
        if (shouldTickDown)
        {
            currentTimerTime -= Time.deltaTime;

            timerText.text = Mathf.FloorToInt(currentTimerTime).ToString();
        }
    }

    public void disableTimer()
    {
        shouldTickDown = false;
        timerText.gameObject.SetActive(false);
    }

    public void makeTransition(Transition transition, int endingOffset = 0)
    {
        currentTransition = transition;
        currentTransitionId = transitionToIndex(transition) + endingOffset;
        startTransition();
    }

    private void startTransition()
    {
        transitionImages[currentTransitionId].SetActive(true);
        transitionBackgroundImage.SetActive(true);
        cameraRig.transform.position = startingPosition;

        if (currentTransition == Transition.Ending)
        {
            secondsToDisplayTransition = 0;
        }

        Invoke("endTransition", secondsToDisplayTransition);
    }

    private void endTransition()
    {
        switch (currentTransition)
        {
            case Transition.Intro:
                mainSoundManager.playMorningSound();
                mainSoundManager.playMainBackgroundMusic();
                break;
            case Transition.Morning:
                mainSoundManager.playMorningSound();
                mainSoundManager.playMainBackgroundMusic();
                break;
            case Transition.Lunch:
                mainSoundManager.playLunchSound();
                break;
            case Transition.Afternoon:
                mainSoundManager.playAfternoonSound();
                break;
            case Transition.DateSelection:
                mainSoundManager.playDateSound();
                break;
            case Transition.Date:
                mainSoundManager.playMinigameMusic();
                shouldTickDown = true;
                currentTimerTime = timeToPlayMinigameInSeconds;
                timerText.gameObject.SetActive(true);
                break;
            case Transition.Accusation:
                mainSoundManager.playAccusationMusic();
                break;
            case Transition.Ending:
                mainSoundManager.playEndScreenMusic();
                break;
            default:
                mainSoundManager.playMorningSound();
                break;
        }

        if (currentTransition != Transition.Ending)
        {
            transitionImages[currentTransitionId].SetActive(false);
            transitionBackgroundImage.SetActive(false);
        }
    }

    private int transitionToIndex(Transition transition)
    {
        int result = 0;

        switch (transition)
        {
            case Transition.Intro:
                result = 0;
                break;
            case Transition.Morning:
                result = 1;
                break;
            case Transition.Lunch:
                result = 2;
                break;
            case Transition.Afternoon:
                result = 3;
                break;
            case Transition.DateSelection:
                result = 4;
                break;
            case Transition.Date:
                result = 5;
                break;
            case Transition.Accusation:
                result = 6;
                break;
            case Transition.Ending:
                result = 7;
                break;
        }

        return result;
    }
}
