using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameSystemUtilities {
    // VARIABLES //
    private static readonly string FILE_PATH = Application.persistentDataPath + "/save.dat";

    // FUNCTIONS //
    // Save game data
    public static void Save() {
        // Create binary formatter and file stream from save file path
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(FILE_PATH, FileMode.Create);
        
        // Serialize main data into file stream and close stream
        formatter.Serialize(fileStream, DataMain.Current);
        fileStream.Close();

        Debug.Log("Saving Data...");
    }

    // Load game data
    public static void Load() {
        if (File.Exists(FILE_PATH)) {
            // Create binary formatter and open file stream from save file path
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(FILE_PATH, FileMode.Open);

            // Deserialize main data into from file stream into current main data
            // Close stream after
            DataMain.Current = formatter.Deserialize(fileStream) as DataMain;
            fileStream.Close();

            Debug.Log("Loading Data...");
        }else {
            Debug.Log("ERROR: Save file not found in" + FILE_PATH);
        }
    }
}
