using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using System.Text;
using UnityEngine;

public static class RecordData
{
    /// <summary>
    /// Save data to a file
    /// </summary>
    /// <param name="FileName"></param>
    /// <param name="data"></param>
    public static void SaveData(string FileName, string data)
    {
        string dataPath = Path.Combine(Application.persistentDataPath, FileName);
        // This text is added only once to the file.
        if (!File.Exists(dataPath))
        {

            // Create a file to write to.
            File.Create(dataPath).Dispose();
            File.WriteAllText(dataPath, data);
            //File.WriteAllText(dataPath, data);
        }
        else{
            // This text is always added, making the file longer over time, if it is not deleted.
            File.AppendAllText(dataPath, data);

            // Append new lines of text to the file
            /*File.AppendAllLines(path, data);*/
        }

    }


    //https://stackoverflow.com/questions/69147519/save-and-load-data-from-application-persistentdatapath-works-in-unity-but-not-o
    /// <summary>
    /// Load data at a specified file and folder location
    /// </summary>
    /// <param name="FileName"></param>
    /// <returns></returns>
    public static T LoadData<T>(string FileName)
    {
        // get the data path of this save data
        string dataPath = Path.Combine(Application.persistentDataPath, FileName);

        // if the file path or name does not exist, return the default SO
        if (!Directory.Exists(Path.GetDirectoryName(dataPath)))
        {
            Debug.Log("File or path does not exist! " + dataPath);
            return default(T);
        }

        //load in the save data as text in line
        string[] lines = null;

        try
        {
            lines = File.ReadAllLines(dataPath);
            Debug.Log("<color=green>Loaded all data from: </color>" + dataPath);
            Debug.Log("lines: "+lines);
            foreach (string line in lines)
            {
                Debug.Log("\t" + line);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to load data from: " + dataPath);
            Debug.LogWarning("Error: " + e.Message);
            return default(T);
        }

        // return the casted object to use
        return (T)Convert.ChangeType(lines, typeof(T));

    }


    /// <summary>
    /// Delete a specific line in a file 
    /// </summary>
    /// <param name="FileName"></param>
    /// <param name="LineIndex"></param>
    /// <returns></returns>
    public static void DeleteLine(string FileName, int LineIndex)
    {
        // get the data path
        string dataPath = Path.Combine(Application.persistentDataPath, FileName);

        List<string> linesList = File.ReadAllLines(dataPath).ToList();

        linesList.RemoveAt(LineIndex);

        //File.WriteAllLines(dataPath, linesList.ToArray());
        File.WriteAllLines(dataPath, linesList);

    }
}
