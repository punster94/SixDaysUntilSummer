using UnityEngine;
using System.Collections;

public class Date
{
    public int characterId;

    public Statement yes;
    public Statement no;
    public Statement response;

    public Date(int ci, Statement y, Statement n, Statement r)
    {
        characterId = ci;
        yes = y;
        no = n;
        response = r;
    }
}
