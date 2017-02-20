using UnityEngine;
using System.Collections;

public class Dialog
{
    public int accidentId;
    public int characterId;

    public Statement startingStatement;
    public Statement topOption;
    public Statement bottomOption;
    public Statement accidentStatement;

    public Dialog(int ai, int ci, Statement ss, Statement to, Statement bo, Statement accident)
    {
        accidentId = ai;
        characterId = ci;
        startingStatement = ss;
        topOption = to;
        bottomOption = bo;
        accidentStatement = accident;
    }
}
