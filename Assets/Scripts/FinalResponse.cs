using UnityEngine;
using System.Collections;

public class FinalResponse
{
    public int characterId;
    public Statement positiveResponse;
    public Statement negativeResponse;

    public FinalResponse(int ci, Statement pr, Statement nR)
    {
        characterId = ci;
        positiveResponse = pr;
        negativeResponse = nR;

        positiveResponse.resetIndex();
        negativeResponse.resetIndex();
    }

    public Statement selectResponse(int lp)
    {
        Statement response = positiveResponse;

        if (lp <= 0)
            response = negativeResponse;

        return response;
    }
}
