﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System.IO;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class HandleTargetInteractiveInfo : MonoBehaviour
{
    [SerializeField]
    Text debugText;
    // Start is called before the first frame update
    void Start()
    {
        List<string[]> data = ReadCSVFile(Path.Combine(Application.dataPath, "InteractiveData.csv"));
        AddInteractiveTextRecursive(gameObject, data);
        SetChildrenActiveRecursive(gameObject, true);
    }

    public void AddInteractiveTextRecursive(GameObject navGO, List<string[]> data)
    {
        if (IsTarget(navGO))
        {
            string text = getInteractiveText(navGO, data);
            SetInteractiveText(navGO, text);
            return;
        }
        int childCount = navGO.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            GameObject child = navGO.transform.GetChild(i).gameObject;
            AddInteractiveTextRecursive(child, data);
        }
    }

    public void SetInteractiveText(GameObject gameObject, string str)
    {
        TextMeshPro text = gameObject.GetComponentInChildren<TextMeshPro>();
        text.text = str;
    }

    public string getInteractiveText(GameObject target, List<string[]> data)
    {
        string[] row = FindRowByAuditoryNumber(data, target.name);
        if (row != null) {
            try
            {
                return 
                    ifNotEmpty($"№ аудитории: {row[1]}", row[1]) +
                    ifNotEmpty($"Кол-во посадочных мест: " + row[2], row[2]) +
                    ifNotEmpty($"Площадь: {row[3]} м²", row[3]) +
                    ifNotEmpty($"Назначение: {row[4]}", row[4]) +
                    ifNotEmpty($"Фактическое использование: {row[5]}", row[5]) +
                    ifNotEmpty($"Кафедра: {row[6]}", row[6]) +
                    ifNotEmpty($"Институт: {row[7]}", row[7]);
            }
            catch (System.IndexOutOfRangeException e)
            {
                Debug.Log("Error with" + row[1]);
                throw new System.Exception("Wrong format. Delete \\n form .csv");
            }

            }
        return "Аудитория " + target.name;
    }

    private string ifNotEmpty(string res, string forCheck)
    {
       if ((forCheck ?? "") != "") // if not null or ""
       {
            return res + "\n";
       }
        return "";
    }

    public string[] FindRowByAuditoryNumber(List<string[]> listOfRows, string auditoryNumber)
    {
        for (int i = 0; i < listOfRows.Count; i++)
        {
            string[] row = listOfRows[i];
            foreach (string cell in row)
            {
                if (cell.Contains(auditoryNumber))
                {
                    return row; // Return the index of the row where the auditory number was found
                }
            }
        }

        return null; // Return if the auditory number was not found in any row
    }


    public List<string[]> ReadCSVFile(string filePath)
    {
        List<string[]> data = new List<string[]>();

        using (StreamReader reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] row = line.Split(';');

                data.Add(row);
            }
        }
        return data;
    }


    public bool IsTarget(GameObject gameObject)
    {
        int childCount = gameObject.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            if (child.name == "isTarget")
            {
                return true;
            }
        }
        return false;
    }

    public static void SetChildrenActiveRecursive(GameObject parent, bool active)
    {
        int childCount = parent.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            GameObject child = parent.transform.GetChild(i).gameObject;
            child.SetActive(active);
            SetChildrenActiveRecursive(child, active);
        }
    }

}