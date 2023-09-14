using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Windows;
using System.Text;
using System.Collections;

public class DataManager : MonoBehaviour
{
    private string csvName = "data.csv"; // Name of the CSV file
    private string gameStartTime;
    
    private void Start()
    {
        StartCoroutine(DelayedFunctionCall());     
    }

    private IEnumerator DelayedFunctionCall()
    {      
        // Wait for 2 seconds before calling the function
        yield return new WaitForSeconds(2.0f);

        // Call your function here
        WriteCSVHeadersToEmptyRow();
        gameStartTime = GetCurrentTIme();
    }

    public string GetGameStartTIme()
    {
        return gameStartTime;
    }

    private void WriteCSVHeadersToEmptyRow()
    {
        string csvFilePath = Path.Combine(Application.dataPath, csvName);
        string[] lines = System.IO.File.ReadAllLines(csvFilePath);
        int emptyRowIndex = -1;

        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
            {
                emptyRowIndex = i;
                break;
            }
        }

        string headersString = "Type of Record,Time of Record," +
                "Name of Ghost,Equipped Camo," +
                "First Time on Screen,Number of Times on Screen,Total Screen Time in Seconds,Start Time of Current Sighting," +
                "Ghost Movement Speed,Ghost Rotation Speed,Ghost Is Rotating,Ghost Scale" +
                "Was Hit,Point of Contact," +
                "Ghost Position,Ghost Rotation," +
                "Player Position,Player Rotation," +
                "Game Start Time";

        if (emptyRowIndex >= 0)
        {
            lines[emptyRowIndex] = headersString;
            System.IO.File.WriteAllLines(csvFilePath, lines);           
        }
        else
        {
            // If no truly empty row is found, add a new row at the end of the file.
            using (StreamWriter writer = System.IO.File.AppendText(csvFilePath))
            {
                writer.WriteLine(headersString);
            }           
        }
    }

    public void WriteToCSV(List<string[]> newData)
    {
        string csvFilePath = Path.Combine(Application.dataPath, csvName);
        try
        {
            List<string[]> recordedData = new List<string[]>();

            if (System.IO.File.Exists(csvFilePath))
            {
                using (StreamReader reader = new StreamReader(csvFilePath))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] row = line.Split(',');
                        recordedData.Add(row);
                    }
                }
            }

            int emptyRowIndex = recordedData.FindIndex(row => row.Length == 0);

            if (emptyRowIndex == -1)
            {
                recordedData.Add(new string[0]);
                emptyRowIndex = recordedData.Count - 1;
            }

            // Make sure newData isn't empty
            if (newData.Count > 0)
            {
                // Assuming newData only contains two rows
                recordedData[emptyRowIndex] = newData[0];
                recordedData.Add(newData[1]); // Add the second row
            }

            using (StreamWriter writer = new StreamWriter(csvFilePath, false, Encoding.UTF8))
            {
                foreach (string[] row in recordedData)
                {
                    string line = string.Join(",", row);
                    writer.WriteLine(line);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error writing to CSV: " + e.Message);
        }
    }

    public string ReplaceCommasWithSemicolon(string inputString)
    {
        return inputString.Replace(",", ";");
    }

    public string GetCurrentTIme()
    {
        // Get the current real-world time
        System.DateTime currentTime = System.DateTime.Now;

        // Access different components of the time
        int hour = currentTime.Hour;
        int minute = currentTime.Minute;
        int second = currentTime.Second;

        string currentTimeString = hour + ":" + minute + ":" + second;
        currentTimeString = ReplaceCommasWithSemicolon(currentTimeString);
        return currentTimeString;
    }
}

