using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManager
{
    public static void SavePlayerData (ProgressManager progress)
    {
        //creates a binary formater to convert data to binary
        BinaryFormatter formatter = new BinaryFormatter();
        //name of the string, using a persistent file path to always be consistent with where it saves
        string path = Application.persistentDataPath + "/Raider.Rampage";

        //creates and opens a file stream
        FileStream stream = new FileStream(path, FileMode.Create);

        //establishes and saves the player data
        PlayerData data = new PlayerData(progress);
        formatter.Serialize(stream, data);

        //closes the data stream
        stream.Close();
    }

    public static PlayerData LoadPlayerData()
    {
        string path = Application.persistentDataPath + "/Raider.Rampage";

        if (File.Exists(path)) //save data exsists
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        }
        else // no save data exsists, default values, ran when game is first loaded and there is no save data. 
        {
            //sets defauly progress value, only level 1 should be accesible
            ProgressManager defaultProgress = new ProgressManager();
            defaultProgress.highestLevelCompleted = 1;

            //player settings here

            //sets default data to a player data, saves generated default data
            SavePlayerData(defaultProgress);

            PlayerData defaultData = new PlayerData(defaultProgress);
            return defaultData;
        }
    }

    //deletes the save data, largly for testing but may be added to a settings menu. 
    //note: will delete both player settings pref and progress, so maybe a diffrent system that just overwrites one or the other would be a good idea
    public static void DeleteSaveData()
    {
        string path = Application.persistentDataPath + "/Raider.Rampage";
        if(File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
