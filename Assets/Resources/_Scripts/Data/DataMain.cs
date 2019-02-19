using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataMain {
    // VARIABLES //
    // Main data instance
    public static DataMain Current;
    public DataPlayer playerData;
    public DataDungeon dungeonData;
    public bool IsLoadedGame;
    public bool IsNewGame;

    // FUNCTIONS //
    // Constructor
    public DataMain() {
        playerData = new DataPlayer();
        dungeonData = new DataDungeon();

        IsLoadedGame = false;
        IsNewGame = true;
    }
}
