using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public static class XMLParser
{
    public static List<Dialog> parseAccidentDialogs(string accidentXML)
    {
        List<Dialog> dialogList = new List<Dialog>();

        XmlDocument dialogFile = new XmlDocument();
        dialogFile.LoadXml(accidentXML);

        foreach (XmlNode dialog in dialogFile.DocumentElement.ChildNodes)
        {
            int characterId = System.Int32.Parse(dialog.Attributes["characterId"].InnerText);
            int accidentId = System.Int32.Parse(dialog.Attributes["accidentId"].InnerText);

            Statement startingStatement = null, topOption = null, bottomOption = null, accidentStatement = null;

            foreach (XmlNode field in dialog.ChildNodes)
            {
                switch (field.Name)
                {
                    case "StartingStatement":
                        startingStatement = generateStatement(field);
                        break;
                    case "AccidentStatement":
                        accidentStatement = generateStatement(field);
                        break;
                    case "TopOption":
                        topOption = generateStatement(field);
                        break;
                    case "BottomOption":
                        bottomOption = generateStatement(field);
                        break;
                    default:
                        break;
                }
            }

            dialogList.Add(new Dialog(accidentId, characterId, startingStatement, topOption, bottomOption, accidentStatement));
        }

        return dialogList;
    }

    public static List<PersonInformation> parsePersonInformation(string infoXML)
    {
        List<PersonInformation> personInformation = new List<PersonInformation>();

        XmlDocument infoFile = new XmlDocument();
        infoFile.LoadXml(infoXML);

        foreach (XmlNode information in infoFile.DocumentElement.ChildNodes)
        {
            int characterId = System.Int32.Parse(information.Attributes["characterId"].InnerText);

            List<Statement> informationStatements = new List<Statement>();
            List<Statement> topResponses = new List<Statement>();
            List<Statement> bottomResponses = new List<Statement>();

            foreach (XmlNode informationItem in information.ChildNodes)
            {
                foreach (XmlNode field in informationItem.ChildNodes)
                {
                    switch (field.Name)
                    {
                        case "Statement":
                            informationStatements.Add(generateStatement(field));
                            break;
                        case "TopResponseStatement":
                            topResponses.Add(generateStatement(field));
                            break;
                        case "BottomResponseStatement":
                            bottomResponses.Add(generateStatement(field));
                            break;
                        default:
                            break;
                    }
                }
            }

            personInformation.Add(new PersonInformation(characterId, informationStatements, topResponses, bottomResponses));
        }

        return personInformation;
    }

    public static List<FinalResponse> parseFinalStatements(string finalXML)
    {
        List<FinalResponse> finalStatements = new List<FinalResponse>();

        XmlDocument finalFile = new XmlDocument();
        finalFile.LoadXml(finalXML);

        foreach (XmlNode response in finalFile.DocumentElement.ChildNodes)
        {
            int characterId = System.Int32.Parse(response.Attributes["characterId"].InnerText);

            Statement positiveResponse = null, negativeResponse = null;

            foreach (XmlNode component in response.ChildNodes)
            {
                switch (component.Name)
                {
                    case "PositiveResponse":
                        positiveResponse = generateStatement(component);
                        break;
                    case "NegativeResponse":
                        negativeResponse = generateStatement(component);
                        break;
                }
            }

            finalStatements.Add(new FinalResponse(characterId, positiveResponse, negativeResponse));
        }

        return finalStatements;
    }

    public static List<Date> parseDateFile(string dateXML)
    {
        List<Date> dates = new List<Date>();

        XmlDocument dateFile = new XmlDocument();
        dateFile.LoadXml(dateXML);

        foreach (XmlNode date in dateFile.DocumentElement.ChildNodes)
        {
            int characterId = System.Int32.Parse(date.Attributes["characterId"].InnerText);

            Statement yes = null, no = null, response = null;

            foreach (XmlNode component in date.ChildNodes)
            {
                switch (component.Name)
                {
                    case "Yes":
                        yes = generateStatement(component);
                        break;
                    case "No":
                        no = generateStatement(component);
                        break;
                    case "Response":
                        response = generateStatement(component);
                        break;
                }
            }

            dates.Add(new Date(characterId, yes, no, response));
        }

        return dates;
    }

    public static List<Accusation> parseAccusationFile(string accusationXML)
    {
        List<Accusation> accusations = new List<Accusation>();

        XmlDocument accusationFile = new XmlDocument();
        accusationFile.LoadXml(accusationXML);

        foreach (XmlNode accusation in accusationFile.DocumentElement.ChildNodes)
        {
            int characterId = System.Int32.Parse(accusation.Attributes["characterId"].InnerText);

            Statement yes = null, no = null, response = null;

            foreach (XmlNode component in accusation.ChildNodes)
            {
                switch (component.Name)
                {
                    case "Yes":
                        yes = generateStatement(component);
                        break;
                    case "No":
                        no = generateStatement(component);
                        break;
                    case "Response":
                        response = generateStatement(component);
                        break;
                }
            }

            accusations.Add(new Accusation(characterId, yes, no, response));
        }

        return accusations;
    }

    private static Statement generateStatement(XmlNode node)
    {
        int lpValue = 0;

        if (node.Attributes["lp"] != null)
            lpValue = System.Int32.Parse(node.Attributes["lp"].InnerText);

        List<string> texts = new List<string>();
        List<string> paths = new List<string>();
        List<string> keywords = new List<string>();

        foreach (XmlNode field in node.ChildNodes)
        {
            switch (field.Name)
            {
                case "Text":
                    texts.Add(field.InnerText);
                    break;
                case "FilePath":
                    paths.Add(field.InnerText);
                    break;
                case "Keyword":
                    keywords.Add(field.InnerText);
                    break;
                default:
                    break;
            }
        }

        return new Statement(lpValue, texts, paths, keywords);
    }
}
