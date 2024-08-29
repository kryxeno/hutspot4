using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public string saveName = "SaveData_";
    [Range(0, 10)]
    public int saveDataIndex = 0;

    public void SaveData(string dataToSave)
    {
        if (WriteToFile(saveName + saveDataIndex, dataToSave))
        {
            Debug.Log("Data saved successfully!");
        }
    }

    public string LoadData()
    {
        if (ReadFromFile(saveName + saveDataIndex, out string data))
        {
            Debug.Log("Data loaded successfully!");
            return data;
        }
        return null;
    }

    private bool WriteToFile(string name, string content)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, name);
        try
        {
            File.WriteAllText(fullPath, content);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to write to file: " + fullPath + "\n" + e.Message);
            return false;
        }
    }

    private bool ReadFromFile(string name, out string content)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, name);
        try
        {
            content = File.ReadAllText(fullPath);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to read from file: " + fullPath + "\n" + e.Message);
            content = null;
            return false;
        }
    }
}
