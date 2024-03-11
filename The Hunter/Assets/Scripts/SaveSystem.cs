using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public static class SaveSystem
{
    public static Action finishedSaving;
    //in simple words the SaveData method will create a new file
    //in that file a binary encoded verion of the games data will be stored
    public static string SaveData(string data, string path)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
        finishedSaving.Invoke();
        return "Saved on this path: " + path + Environment.NewLine + data;
    }

    //the LoadPlayer method will decode that data and load them into the game when the game starts
    public static string LoadData(string path)
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            string data = formatter.Deserialize(stream) as string;
            stream.Close();
            Debug.Log("Data loaded from file: " + path + " = " + data);
            return data;
        }else
        {
            Debug.Log("Save file was not found in " + path);
            return null;
        }    
    }
}
