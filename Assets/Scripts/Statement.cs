using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Statement
{
    public int lpValue;
    private List<string> texts;
    private List<string> dialogSoundFilePaths;

    private int currentIndex;

    public Statement(int lpv, List<string> t, List<string> dsfp, List<string> k)
    {
        currentIndex = -1;

        lpValue = lpv;
        texts = t;
        dialogSoundFilePaths = dsfp;

        filterTextValues(k);
    }

    public string getCurrentText()
    {
        return texts[currentIndex];
    }

    public string getCurrentSoundFilePath()
    {
        if (dialogSoundFilePaths.Count <= currentIndex)
            return null;

        return dialogSoundFilePaths[currentIndex];
    }

    public bool advanceIndex()
    {
        currentIndex++;
        currentIndex = currentIndex % texts.Count;

        return currentIndex != 0;
    }

    public void resetIndex()
    {
        currentIndex = 0;
    }

    private void filterTextValues(List<string> keywords)
    {
        List<string> newTexts = new List<string>();

        foreach (string text in texts)
        {
            string newText = text;

            foreach (string keyword in keywords)
            {
                if (keyword != "")
                    newText = newText.Replace(keyword, "<color=#" + DialogUIManager.keywordColorReference + "><i>" + keyword + "</i></color>");
            }

            newTexts.Add(newText);
        }

        texts = newTexts;
    }
}
