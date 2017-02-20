using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;

public enum DialogState { Invalid, NotTalking, StartingStatement, DisplayingChoices, ShowingAccidentStatement, ShowingInfoStatement, DisplayingInfoChoices, DisplayingFinalResponse, AskOnDateChoice, AskOnDateResponse, AccuseChoice, AccuseResponse, MaxDialogState }
public class DialogManager : MonoBehaviour
{
    public TextAsset dialogFile;
    public TextAsset informationFile;
    public TextAsset finalResponseFile;
    public TextAsset dateFile;
    public TextAsset accusationFile;

    public Camera mainCamera;

    public GameStateManager gameStateManager;

    private List<Dialog> dialogs;
    private List<PersonInformation> personInformation;
    private List<FinalResponse> finalResponses;
    private List<Date> dates;
    private List<Accusation> accusations;

    private DialogState dialogState;

    private DialogUIManager ui;
    private DialogSoundManager soundManager;

    private Dialog currentDialog;

    private Statement currentStatement;

    private bool pickedAccident;

    private int currentCharacterId;
    private int currentAccidentId;

    void Start()
    {
        dialogs = XMLParser.parseAccidentDialogs(dialogFile.text);
        personInformation = XMLParser.parsePersonInformation(informationFile.text);
        finalResponses = XMLParser.parseFinalStatements(finalResponseFile.text);
        dates = XMLParser.parseDateFile(dateFile.text);
        accusations = XMLParser.parseAccusationFile(accusationFile.text);

        dialogState = DialogState.NotTalking;

        ui = GetComponent<DialogUIManager>();
        soundManager = GetComponent<DialogSoundManager>();

        currentAccidentId = 0;
    }

    void Update()
    {
        switch (dialogState)
        {
            case DialogState.NotTalking:
                notTalkingUpdate();
                break;
            case DialogState.StartingStatement:
                startingStatementUpdate();
                break;
            case DialogState.DisplayingChoices:
                displayingChoicesUpdate();
                break;
            case DialogState.ShowingAccidentStatement:
                resultUpdate();
                break;
            case DialogState.ShowingInfoStatement:
                resultUpdate();
                break;
            case DialogState.DisplayingInfoChoices:
                infoChoicesUpdate();
                break;
            case DialogState.DisplayingFinalResponse:
                finalResponseUpdate();
                break;
            case DialogState.AskOnDateChoice:
                askOnDateChoiceUpdate();
                break;
            case DialogState.AskOnDateResponse:
                askOnDateResponseUpdate();
                break;
            case DialogState.AccuseChoice:
                accuseChoiceUpdate();
                break;
            case DialogState.AccuseResponse:
                accuseResponseUpdate();
                break;
            default:
                break;
        }
    }

    public void setAccidentId(int accidentId)
    {
        currentAccidentId = accidentId;
    }

    public void initiateDialog(int characterId)
    {
        currentAccidentId = gameStateManager.currentAccidentId();
        ui.repositionCanvas(mainCamera.transform);
        currentCharacterId = characterId;
        selectCurrentDialog();
        mainCamera.cullingMask = 0 | (1 << 5);

        currentDialog.startingStatement.resetIndex();

        if (characterId != 0)
        {
            currentStatement = currentDialog.startingStatement;
            playCurrentStatement(PortraitType.Normal);
            dialogState = DialogState.StartingStatement;
        }
        else
        {
            personInformation[currentCharacterId].advanceIndex();
            currentStatement = personInformation[currentCharacterId].currentInfo();
            pickedAccident = false;
            currentStatement.resetIndex();
            playCurrentStatement(PortraitType.Normal);
            dialogState = DialogState.ShowingInfoStatement;
        }
    }

    private void notTalkingUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E) && gameStateManager.getCurrentTimeState() != TimeState.Invalid)
        {
            initiateDialog(0);
        }
        else if (continueButtonPressed() && RaycastScript.canInteract && gameStateManager.getCurrentTimeState() != TimeState.Date)
        {
            initiateDialog(RaycastScript.interactingObject.GetComponent<CharacterManager>().characterId);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            initiateDialog(1);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            initiateDialog(2);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            initiateDialog(3);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            initiateDialog(4);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            initiateDialog(5);
        }

        if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            initiateDialog(6);
        }
    }

    private void exitDialog()
    {
        ui.removeTextBox();
        mainCamera.cullingMask = 0xFF;
        dialogState = DialogState.NotTalking;
    }

    private void selectCurrentDialog()
    {
        foreach (Dialog dialog in dialogs)
        {
            if (dialog.accidentId == currentAccidentId && dialog.characterId == currentCharacterId)
            {
                currentDialog = dialog;
                break;
            }
        }
    }

    private bool continueButtonPressed()
    {
        return Input.GetKeyDown(KeyCode.E) || ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger);
    }

    private bool rightButtonPressed()
    {
        return Input.GetKeyDown(KeyCode.D) || (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger) && ui.bottomOptionPointedAt);
    }

    private bool leftButtonPressed()
    {
        return Input.GetKeyDown(KeyCode.A) || (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger) && ui.topOptionPointedAt);
    }

    private void startingStatementUpdate()
    {
        if (continueButtonPressed())
        {
            if (currentDialog.startingStatement.advanceIndex())
            {
                currentStatement = currentDialog.startingStatement;
                playCurrentStatement(PortraitType.Normal);
            }
            else
            {
                if (gameStateManager.getCurrentTimeState() == TimeState.DateSelection)
                {
                    if (gameStateManager.isLastDay())
                    {
                        ui.displayDateChoices(accusations[currentCharacterId]);
                        dialogState = DialogState.AccuseChoice;
                    }
                    else
                    {
                        ui.displayDateChoices(dates[currentCharacterId]);
                        dialogState = DialogState.AskOnDateChoice;
                    }
                }
                else
                {
                    ui.displayChoices(currentDialog);
                    dialogState = DialogState.DisplayingChoices;
                }
            }
        }
    }

    private void displayingChoicesUpdate()
    {
        if (leftButtonPressed()) // raycast public text is equal to top dialog
        {
            currentStatement = currentDialog.accidentStatement;
            pickedAccident = true;
            currentStatement.resetIndex();
            gameStateManager.addToLoveState(currentCharacterId, currentDialog.topOption.lpValue);
            playCurrentStatement(portraitTypeFromLP(currentDialog.topOption.lpValue));
            dialogState = DialogState.ShowingAccidentStatement;
        }
        else if (rightButtonPressed()) // raycast public text is equal to bottom dialog
        {
            personInformation[currentCharacterId].advanceIndex();
            currentStatement = personInformation[currentCharacterId].currentInfo();
            pickedAccident = false;
            currentStatement.resetIndex();
            gameStateManager.addToLoveState(currentCharacterId, currentDialog.bottomOption.lpValue);
            playCurrentStatement(portraitTypeFromLP(currentDialog.bottomOption.lpValue));
            dialogState = DialogState.ShowingInfoStatement;
        }
    }

    private PortraitType portraitTypeFromLP(int lp)
    {
        PortraitType portraitType = PortraitType.Positive;

        if (lp <= 0)
            portraitType = PortraitType.Negative;

        return portraitType;
    }

    private void resultUpdate()
    {
        if (continueButtonPressed())
        {
            if (currentStatement.advanceIndex())
            {
                playCurrentStatement(PortraitType.Normal);
            }
            else
            {
                if (pickedAccident)
                {
                    exitDialog();
                    gameStateManager.progressTimeState();
                }
                else
                {
                    ui.displayInfoChoices(personInformation[currentCharacterId]);
                    dialogState = DialogState.DisplayingInfoChoices;
                }
            }
        }
    }

    private void infoChoicesUpdate()
    {
        if (leftButtonPressed())
        {
            if (currentCharacterId != 0)
            {
                int lp = personInformation[currentCharacterId].currentTopResponse().lpValue;
                gameStateManager.addToLoveState(currentCharacterId, lp);
                currentStatement = finalResponses[currentCharacterId].selectResponse(lp);
                playCurrentStatement(portraitTypeFromLP(lp));
                dialogState = DialogState.DisplayingFinalResponse;
            }
            else if (personInformation[0].isExhausted())
            {
                exitDialog();
                gameStateManager.advancetoNextDay();
            }
            else
            {
                personInformation[0].advanceIndex();
                currentStatement = personInformation[0].currentInfo();
                currentStatement.resetIndex();
                playCurrentStatement(PortraitType.Normal);
                dialogState = DialogState.ShowingInfoStatement;
            }
        }
        else if (rightButtonPressed())
        {
            if (currentCharacterId != 0)
            {
                int lp = personInformation[currentCharacterId].currentBottomResponse().lpValue;
                gameStateManager.addToLoveState(currentCharacterId, lp);
                currentStatement = finalResponses[currentCharacterId].selectResponse(lp);
                playCurrentStatement(portraitTypeFromLP(lp));
                dialogState = DialogState.DisplayingFinalResponse;
            }
            else if (personInformation[0].isExhausted())
            {
                exitDialog();
                gameStateManager.advancetoNextDay();
            }
            else
            {
                personInformation[0].advanceIndex();
                currentStatement = personInformation[0].currentInfo();
                currentStatement.resetIndex();
                playCurrentStatement(PortraitType.Normal);
                dialogState = DialogState.ShowingInfoStatement;
            }
        }
    }

    private void finalResponseUpdate()
    {
        if (continueButtonPressed())
        {
            exitDialog();
            gameStateManager.progressTimeState();
        }
    }

    private void askOnDateChoiceUpdate()
    {
        if (leftButtonPressed())
        {
            dates[currentCharacterId].response.resetIndex();
            currentStatement = dates[currentCharacterId].response;
            playCurrentStatement(PortraitType.Positive);
            dialogState = DialogState.AskOnDateResponse;

            gameStateManager.currentCharacterId = currentCharacterId;
        }
        else if (rightButtonPressed())
        {
            exitDialog();
        }
    }

    private void askOnDateResponseUpdate()
    {
        if (continueButtonPressed())
        {
            if (currentStatement.advanceIndex())
            {
                playCurrentStatement(PortraitType.Positive);
            }
            else
            {
                exitDialog();
                gameStateManager.progressTimeState();
            }
        }
    }

    private void accuseChoiceUpdate()
    {
        if (leftButtonPressed())
        {
            accusations[currentCharacterId].response.resetIndex();
            currentStatement = accusations[currentCharacterId].response;
            playCurrentStatement(PortraitType.Negative);
            dialogState = DialogState.AccuseResponse;

            gameStateManager.currentCharacterId = currentCharacterId;
        }
        else if (rightButtonPressed())
        {
            exitDialog();
        }
    }

    private void accuseResponseUpdate()
    {
        if (continueButtonPressed())
        {
            if (currentStatement.advanceIndex())
            {
                playCurrentStatement(PortraitType.Positive);
            }
            else
            {
                exitDialog();
                gameStateManager.progressTimeState();
            }
        }
    }

    private void playCurrentStatement(PortraitType portraitType)
    {
        ui.displayStatement(currentStatement, currentCharacterId, gameStateManager.loveIndexOfCharacter(currentCharacterId), portraitType);
        soundManager.playDialogSound(currentStatement.getCurrentSoundFilePath());
    }
}
