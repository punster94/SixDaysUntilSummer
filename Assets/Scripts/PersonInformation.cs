using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersonInformation
{
    public int characterId;

    private List<Statement> information;
    private List<Statement> topResponses;
    private List<Statement> bottomResponses;

    private bool reachedEnd;

    private int nextItemToBeReferenced;

    public PersonInformation(int ci, List<Statement> i, List<Statement> tr, List<Statement> br)
    {
        characterId = ci;
        information = i;
        topResponses = tr;
        bottomResponses = br;
        nextItemToBeReferenced = -1;

        reachedEnd = false;
    }

    public bool isExhausted()
    {
        return reachedEnd || information.Count == nextItemToBeReferenced;
    }

    public bool advanceIndex()
    {
        if (nextItemToBeReferenced + 2 >= information.Count)
            reachedEnd = true;

        nextItemToBeReferenced = (nextItemToBeReferenced + 1) % information.Count;

        return nextItemToBeReferenced == 0;
    }

    public Statement currentTopResponse()
    {
        return topResponses[nextItemToBeReferenced];
    }

    public Statement currentBottomResponse()
    {
        return bottomResponses[nextItemToBeReferenced];
    }

    public Statement currentInfo()
    {
        return information[nextItemToBeReferenced];
    }
}
