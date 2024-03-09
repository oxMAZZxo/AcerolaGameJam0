using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{

    //in simple words the SaveData method will create a new file
    //in that file a binary encoded verion of the games data will be stored
    public static void SaveData(string data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + ("/GameData.txt");
        FileStream stream = new FileStream(path, FileMode.Create);
        Debug.Log("Saving on this path: " + path);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    //the LoadPlayer method will decode that data and load them into the game when the game starts
    public static string LoadData()
    {
        string path = Application.persistentDataPath + ("/GameData.txt");
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            string data = formatter.Deserialize(stream) as string;
            stream.Close();
            return data;
        }else
        {
            Debug.Log("Save file was not found in " + path);
            return null;
        }    
    }
}
