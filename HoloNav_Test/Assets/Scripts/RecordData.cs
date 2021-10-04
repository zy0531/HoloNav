using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public static class RecordData
{
    
    public static void SaveData(string data)
    {
        string path = Path.Combine(Application.persistentDataPath, "HoloNavData.txt");
        // This text is added only once to the file.
        if (!File.Exists(path))
        {
           
            // Create a file to write to.
            string createText = "Hello and Welcome" + Environment.NewLine;
            File.WriteAllText(path, createText);
        }

        // This text is always added, making the file longer over time
        // if it is not deleted.
       /* string appendText = "This is extra text" + Environment.NewLine;*/
        File.AppendAllText(path, data);

        // Append new lines of text to the file
        /*File.AppendAllLines(path, data);*/


        /*
                // Open the file to read from.
                string readText = File.ReadAllText(path);
                Console.WriteLine(readText);*/
    }
}
