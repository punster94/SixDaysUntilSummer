using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using VRTK;

public enum PortraitType { Invalid, Normal, Positive, Negative, MaxPortraitType }

public class DialogUIManager : MonoBehaviour
{
    public string keywordColor;
    public static string keywordColorReference;

    public Text accidentText;
    public Text informationText;
    public Text dialogText;

    public Image topOptionBG;
    public Image bottomOptionBG;
    public Image dialogBG;

    public bool topOptionPointedAt;
    public bool bottomOptionPointedAt;
    public bool dialogBGPointedAt;

    public Sprite normalButton;
    public Sprite hoverButton;

    public Canvas textBoxCanvas;

    public Image[] characterPortraits;
    public Image[] heartIcons;

    public float drawDistanceFromPlayer;

    public float angleOfTextBox;
    public float heightOfCanvas;
    public Vector3 textBoxOffset;

    private int currentCharacterId;
    private int previousHeartIndex;

	void Start ()
    {
        keywordColorReference = keywordColor;
        previousHeartIndex = 0;
	}
	
	void Update ()
    {
        topOptionPointedAt = false;
        bottomOptionPointedAt = false;
        dialogBGPointedAt = false;

        if (topOptionBG == RaycastScript.imageBeingPointedAt)
        {
            topOptionBG.sprite = hoverButton;
            topOptionPointedAt = true;
        }
        else
        {
            topOptionBG.sprite = normalButton;
        }

        if (bottomOptionBG == RaycastScript.imageBeingPointedAt)
        {
            bottomOptionBG.sprite = hoverButton;
            bottomOptionPointedAt = true;
        }
        else
        {
            bottomOptionBG.sprite = normalButton;
        }

        if (dialogBG == RaycastScript.imageBeingPointedAt)
        {
            //dialogBG.sprite = ;
            dialogBGPointedAt = true;
        }
        else
        {
            //dialogBG.sprite = ;
        }
    }

    public void repositionCanvas(Transform cameraPosition)
    {
        Vector3 direction = cameraPosition.forward;
        direction.y = 0;
        direction = Vector3.Normalize(direction);

        transform.eulerAngles = cameraPosition.eulerAngles;

        Vector3 localAngle = transform.localEulerAngles;
        localAngle.x = 0;
        localAngle.z = 0;
        transform.localEulerAngles = localAngle;

        transform.position = cameraPosition.position + (direction * drawDistanceFromPlayer);
        transform.position = new Vector3(transform.position.x, heightOfCanvas, transform.position.z);

        Vector3 addedAngle = new Vector3(angleOfTextBox, 0, 0);
        textBoxCanvas.transform.localEulerAngles = addedAngle;
        textBoxCanvas.transform.localPosition = textBoxOffset;
    }

    // TODO make displayStatement display a given character portrait by currentCharacterId
    public void displayStatement(Statement statement, int characterId, int loveIndex, PortraitType portraitType)
    {
        characterPortraits[currentCharacterId].gameObject.SetActive(false);
        setCharacterId(characterId, portraitType);

        dialogText.text = statement.getCurrentText();
        accidentText.gameObject.SetActive(false);
        informationText.gameObject.SetActive(false);
        dialogText.gameObject.SetActive(true);
        disableOptionBoxes();
        dialogBG.gameObject.SetActive(true);
        characterPortraits[currentCharacterId].gameObject.SetActive(true);

        toggleCurrentHeartIcon(loveIndex);
    }

    public void displayChoices(Dialog currentDialog)
    {
        dialogText.gameObject.SetActive(false);
        enableOptionBoxes();
        dialogBG.gameObject.SetActive(true);

        currentDialog.topOption.advanceIndex();
        accidentText.text = currentDialog.topOption.getCurrentText();
        accidentText.gameObject.SetActive(true);

        currentDialog.bottomOption.advanceIndex();
        informationText.text = currentDialog.bottomOption.getCurrentText();
        informationText.gameObject.SetActive(true);
    }

    public void displayDateChoices(Date date)
    {
        dialogText.gameObject.SetActive(false);
        enableOptionBoxes();
        dialogBG.gameObject.SetActive(true);

        date.yes.resetIndex();
        accidentText.text = date.yes.getCurrentText();
        accidentText.gameObject.SetActive(true);

        date.no.resetIndex();
        informationText.text = date.no.getCurrentText();
        informationText.gameObject.SetActive(true);
    }

    public void displayInfoChoices(PersonInformation information)
    {
        dialogText.gameObject.SetActive(false);
        enableOptionBoxes();
        dialogBG.gameObject.SetActive(true);

        information.currentTopResponse().advanceIndex();
        accidentText.text = information.currentTopResponse().getCurrentText();
        accidentText.gameObject.SetActive(true);

        information.currentBottomResponse().advanceIndex();
        informationText.text = information.currentBottomResponse().getCurrentText();
        informationText.gameObject.SetActive(true);
    }

    public void removeTextBox()
    {
        disableOptionBoxes();

        dialogText.gameObject.SetActive(false);
        dialogBG.gameObject.SetActive(false);
        characterPortraits[currentCharacterId].gameObject.SetActive(false);

        heartIcons[previousHeartIndex].gameObject.SetActive(false);
    }

    private void setCharacterId(int characterIdBase, PortraitType portraitType)
    {
        switch (portraitType)
        {
            case PortraitType.Positive:
                currentCharacterId = characterIdBase + (characterPortraits.Length / 3);
                break;
            case PortraitType.Negative:
                currentCharacterId = characterIdBase + (2 * characterPortraits.Length / 3);
                break;
            default:
                currentCharacterId = characterIdBase;
                break;
        }
        
    }

    private void toggleCurrentHeartIcon(int id)
    {
        heartIcons[previousHeartIndex].gameObject.SetActive(false);
        heartIcons[id].gameObject.SetActive(true);

        previousHeartIndex = id;
    }

    private void enableOptionBoxes()
    {
        
        topOptionBG.gameObject.SetActive(true);
        bottomOptionBG.gameObject.SetActive(true);
    }

    private void disableOptionBoxes()
    {
        accidentText.gameObject.SetActive(false);
        informationText.gameObject.SetActive(false);
        topOptionBG.gameObject.SetActive(false);
        bottomOptionBG.gameObject.SetActive(false);
    }
}
