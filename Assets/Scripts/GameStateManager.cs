using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;

public enum AccidentState { Invalid, Intro, One, Two, Three, Four, Five, Six, MaxAccidentState };
public enum LoveState { Invalid, Neutral, Friendly, Like, Love, MaxLoveState };
public enum TimeState { Invalid, Morning, Lunch, Afternoon, DateSelection, Date, End, MaxLoveState };

public class GameStateManager : MonoBehaviour
{
    public TransitionManager transitionManager;

    public int numberOfLoveInterests = 7;
    public int minLoveValue = 0;
    public int maxLoveValue = 50;

    public int yandereId = 6;

    public GameObject door;

    public GameObject[] lightingObjects;
    public GameObject[] accidentStateBuckets;

    public Vector3 miniGameStartPoint;
    public Vector3 miniGameEndPoint;

    public GameObject[] miniGameObjects;
    public GameObject controller;
    public GameObject ballBucket;
    public GameObject environment;
    public GameObject lpPlusPrefab;

    public int currentCharacterId;

    private int previousYandereLp = 0;

    private int[] currentLp;

    private TimeState currentTimeState;
    private AccidentState currentAccidentState;
    private int daysSinceStart;

    private GameObject currentAccidentStateBucket;
    private GameObject previousAccidentStateBucket;

    private List<GameObject> currentLpItems;
	void Start ()
    {
        currentLp = new int[numberOfLoveInterests];

	    for (int i = 0; i < numberOfLoveInterests; i++)
        {
            currentLp[i] = minLoveValue;
        }

        lightingObjects[0].SetActive(true);
        currentTimeState = TimeState.Invalid;
        currentAccidentState = AccidentState.Intro;
        daysSinceStart = 0;

        currentAccidentStateBucket = accidentStateBuckets[currentAccidentId()];
        previousAccidentStateBucket = accidentStateBuckets[currentAccidentId()];
        currentAccidentStateBucket.SetActive(true);

        currentLpItems = new List<GameObject>();
    }
	
	void Update ()
    {
        if (currentTimeState == TimeState.Invalid && (Input.GetKeyDown(KeyCode.E) || (RaycastScript.interactingObject == door && ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger))))
        {
            currentTimeState = TimeState.Morning;
            transitionManager.makeTransition(Transition.Intro);
        }

        if (currentTimeState == TimeState.Date && (Input.GetKeyDown(KeyCode.A) || ViveInput.GetPressDown(HandRole.LeftHand, ControllerButton.Trigger)))
        {
            addToLoveState(currentCharacterId, 20);
            progressTimeState();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            advancetoNextDay();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            progressTimeState();
        }
    }

    public void addToLoveState(int characterId, int lpDelta)
    {
        currentLp[characterId] = currentLp[characterId] + lpDelta; // Math.Min(maxLoveValue, Math.Max(minLoveValue, currentLp[characterId] + lpDelta));

        if (lpDelta > 0)
        {
            transitionManager.mainSoundManager.playLPPlusSound();
        }
        else
        {
            transitionManager.mainSoundManager.playLPZeroSound();
        }

        Vector3 prefabPosition = new Vector3();

        foreach (Transform character in accidentStateBuckets[currentAccidentId()].transform)
        {
            if (character.gameObject.GetComponentInChildren<CharacterManager>().characterId == characterId)
            {
                prefabPosition = character.position;
                break;
            }
        }

        Vector3 direction = prefabPosition - controller.transform.position;
        float magnitude = Vector3.Magnitude(direction);
        direction = Vector3.Normalize(direction);

        prefabPosition = controller.transform.position + (direction * (magnitude * 0.9f));
        prefabPosition.y += 2.0f;

        GameObject lpObject = GameObject.Instantiate(lpPlusPrefab);

        lpObject.transform.position = prefabPosition;

        lpObject.transform.rotation = controller.transform.rotation;

        lpObject.transform.localScale = new Vector3(lpDelta, lpDelta, lpDelta) * 0.2f;

        currentLpItems.Add(lpObject);
        Invoke("destroyLpItems", 1.0f);
    }

    private void destroyLpItems()
    {
        foreach (GameObject lpItem in currentLpItems)
        {
            Destroy(lpItem);
        }

        currentLpItems = new List<GameObject>();
    }

    public TimeState getCurrentTimeState()
    {
        return currentTimeState;
    }

    public bool isLastDay()
    {
        return daysSinceStart >= numberOfLoveInterests;
    }

    public int loveIndexOfCharacter(int characterId)
    {
        int index = 0;

        switch (determineLoveState(currentLp[characterId]))
        {
            case LoveState.Friendly:
                index = 1;
                break;
            case LoveState.Like:
                index = 2;
                break;
            case LoveState.Love:
                index = 3;
                break;
        }

        return index;
    }

    private LoveState determineLoveState(int lp)
    {
        LoveState state = LoveState.Invalid;

        if (lp < 15)
            state = LoveState.Neutral;
        else if (lp < 30)
            state = LoveState.Friendly;
        else if (lp < 40)
            state = LoveState.Like;
        else if (lp >= 40)
            state = LoveState.Love;

        return state;
    }

    public void advancetoNextDay()
    {
        foreach (GameObject lighting in lightingObjects)
        {
            lighting.SetActive(false);
        }

        if (currentLp[yandereId] <= previousYandereLp)
            progressAccidentState();

        previousYandereLp = currentLp[yandereId];

        daysSinceStart++;

        previousAccidentStateBucket.SetActive(false);
        currentAccidentStateBucket.SetActive(true);

        if (isLastDay())
        {
            lightingObjects[3].SetActive(true);
            currentTimeState = TimeState.DateSelection;
            transitionManager.makeTransition(Transition.Accusation);
        }
        else
        {
            lightingObjects[0].SetActive(true);
            currentTimeState = TimeState.Morning;
            transitionManager.makeTransition(Transition.Morning);
        }
    }

    public void progressTimeState()
    {
        switch (currentTimeState)
        {
            case TimeState.Morning:
                lightingObjects[0].SetActive(false);
                lightingObjects[1].SetActive(true);
                currentTimeState = TimeState.Lunch;
                transitionManager.makeTransition(Transition.Lunch);
                break;
            case TimeState.Lunch:
                lightingObjects[1].SetActive(false);
                lightingObjects[2].SetActive(true);
                currentTimeState = TimeState.Afternoon;
                transitionManager.makeTransition(Transition.Afternoon);
                break;
            case TimeState.Afternoon:
                lightingObjects[2].SetActive(false);
                lightingObjects[3].SetActive(true);
                currentTimeState = TimeState.DateSelection;
                transitionManager.makeTransition(Transition.DateSelection);
                break;
            case TimeState.DateSelection:
                if (isLastDay())
                {
                    currentTimeState = TimeState.End;
                    transitionManager.makeTransition(Transition.Ending, 0);
                }
                else
                {
                    currentTimeState = TimeState.Date;
                    transitionManager.makeTransition(Transition.Date);
                }

                setUpDateGameObjects();
                break;
            case TimeState.Date:
                lightingObjects[3].SetActive(false);
                lightingObjects[0].SetActive(true);
                currentTimeState = TimeState.Morning;

                cleanUpDateGameObjects();

                advancetoNextDay();
                break;
        }
    }

    private void setUpDateGameObjects()
    {
        for(int i = 0; i < miniGameObjects.Length; i++)
        {
            GameObject gameObject = GameObject.Instantiate(miniGameObjects[i]);
            gameObject.transform.parent = environment.transform;

            float x = UnityEngine.Random.Range(miniGameStartPoint.x, miniGameEndPoint.x);
            float y = UnityEngine.Random.Range(miniGameStartPoint.y, miniGameEndPoint.y);
            float z = UnityEngine.Random.Range(miniGameStartPoint.z, miniGameEndPoint.z);
            gameObject.transform.localPosition = new Vector3(x, y, z);

            gameObject.GetComponent<MiniGameObjectScript>().setObjectData(i, ballBucket, controller);
        }

        foreach (Transform character in accidentStateBuckets[currentAccidentId()].transform)
        {
            if (character.GetComponentInChildren<CharacterManager>().characterId != currentCharacterId)
            {
                character.gameObject.SetActive(false);
            }
        }
    }

    private void cleanUpDateGameObjects()
    {
        foreach (Transform remainingSphere in ballBucket.transform)
        {
            Destroy(remainingSphere.gameObject);
        }

        foreach (Transform character in accidentStateBuckets[currentAccidentId()].transform)
        {
            character.gameObject.SetActive(true);
        }

        transitionManager.disableTimer();
    }

    private void progressAccidentState()
    {
        previousAccidentStateBucket = currentAccidentStateBucket;

        switch (currentAccidentState)
        {
            case AccidentState.Intro:
                currentAccidentState = AccidentState.One;
                break;
            case AccidentState.One:
                currentAccidentState = AccidentState.Two;
                break;
            case AccidentState.Two:
                currentAccidentState = AccidentState.Three;
                break;
            case AccidentState.Three:
                currentAccidentState = AccidentState.Four;
                break;
            case AccidentState.Four:
                currentAccidentState = AccidentState.Five;
                break;
            case AccidentState.Five:
                currentAccidentState = AccidentState.Six;
                break;
            default:
                break;
        }

        currentAccidentStateBucket = accidentStateBuckets[currentAccidentId()];
    }

    public int currentAccidentId()
    {
        int id = 0;

        switch (currentAccidentState)
        {
            case AccidentState.Intro:
                id = 0;
                break;
            case AccidentState.One:
                id = 1;
                break;
            case AccidentState.Two:
                id = 2;
                break;
            case AccidentState.Three:
                id = 3;
                break;
            case AccidentState.Four:
                id = 4;
                break;
            case AccidentState.Five:
                id = 5;
                break;
            case AccidentState.Six:
                id = 6;
                break;
        }

        return id;
    }
}