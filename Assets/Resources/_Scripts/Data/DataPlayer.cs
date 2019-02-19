using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataPlayer {
    // VARIABLES //
    public readonly int MAX_LEVEL = 20;
    public SerializableVector3 currentPosition;

    [Header("Player stats")]
    public int level = 1;           // Player level determined by experience
    public int exp = 0;             // Player experience
    public int expBase = 100;       // Base experience for level 1
    public int expLeft = 10;        // Experience left for the next level
    public int health;              // Player health
    public int maxHealth = 100;     // Player max health determined by level
    public float expMod = 1.25f;    // Experience modifier
    public Stats attack;            // Player attack determined by level
    public Stats defence;           // Player defence determined by level
    public string playerName;       // Player name
    public float expRatio;          // Ratio between current exp and exp base
    public bool leveledUp;          // If the player level up or not
    public bool gainedExp;          // If the player gainedExp or not

    // maxFloorLevel holds the maximum floor the player reached
    // currentFloorLevel is the current floor the player is on
    // 0 being town, from 1 to MAX_LEVEL is dungeon
    public int maxFloorLevel, currentFloorLevel;

    // FUNCTIONS //
    // Constructor
    public DataPlayer() {
        // Set up new player stats and set initial position
        // Initial position for a new game is in begeinning of town
        currentPosition = new SerializableVector3(50, 20, 135);
        maxFloorLevel = 0;
        currentFloorLevel = 0;
    }

    // Updates player's position from player model's transform
    public void UpdateCurrentPosition(Vector3 input) {
        currentPosition = input;
    }

    // Return scene index depending on the current floor level the player is on
    public int GetSceneIndex() {
        return currentFloorLevel == 0 ? 1 : 2;
    }
    
    // Given a floor number, wrap player to scene
    // No need to check if floor number is less then or equal to max reached floor
    public void WrapToFloor(int floorNumber) {
        currentFloorLevel = floorNumber;
    }

    // Given a boolean move player to next/prev level
    // True is up, and false is down
    public bool UpdateCurrentFloor(bool direction) {
        if (direction && currentFloorLevel == MAX_LEVEL) {  
            Debug.Log("Cannot go up anymore!");
            return false;
        }else if (!direction && currentFloorLevel == 0) {
            Debug.Log("Cannot go down anymore!");
            return false;
        }

        // Player can advance or regress
        if (direction) {
            currentFloorLevel++;
        }else {
            currentFloorLevel--;
        }

        // Update max reached level
        maxFloorLevel = Mathf.Max(currentFloorLevel, maxFloorLevel);

        return true;
    }

    // Given a boolean, return if player can go next/prev level
    // True is up, false is down
    public bool CheckDirection(bool direction) {
        if (direction && currentFloorLevel == MAX_LEVEL) {
            Debug.Log("Cannot go up anymore!");
            return false;
        } else if (!direction && currentFloorLevel == 0) {
            Debug.Log("Cannot go down anymore!");
            return false;
        }

        return true;
    }

    // Gain experience for each enemy the player kills
    // This depends on what floor 
    // Calculate level and see if player level up
    // Return true if player level up, false other wise
    public void GainExp(int enemeyExp) {
        exp += enemeyExp;

        // If current exp is more then then the current exp;
        if (exp >= expLeft) {
            // Set current exp to 0, level up, then increase expLeft
            exp = 0;
            level++;
            expLeft = (int)Mathf.Floor(expBase * Mathf.Pow(expMod, level));

            leveledUp = true;
            CalculateStats();
        }

        expRatio = (float)exp / (float)expLeft;
        gainedExp = true;

        Debug.Log("Current exp: " + DataMain.Current.playerData.exp);
    }

    // Calculate stats when player level ups
    public void CalculateStats() {
        ObjectStats playerStats = ControllerPlayer.Instance.GetComponent<ObjectStats>();

        playerStats.attack = new Stats(50 + (int)Mathf.Floor(2 * Mathf.Pow(1.5f, level)));
        playerStats.defence = new Stats(10 * level);
        playerStats.maxHealth = 150 * level;
        playerStats.currentHealth = playerStats.maxHealth;
    }

    // Save stats to player data
    public void SaveStats() {
        ObjectStats playerStats = ControllerPlayer.Instance.GetComponent<ObjectStats>();

        DataMain.Current.playerData.attack = playerStats.attack;
        DataMain.Current.playerData.defence = playerStats.defence;
        DataMain.Current.playerData.maxHealth = playerStats.maxHealth;
        DataMain.Current.playerData.health = playerStats.currentHealth;
    }

    // TESTING AND DEBUGGING //
    // For testing and debugging only
    
    public void PrintData() {
        Debug.Log(
            "--PLAYER--" + "\n" +
            "Name: " + playerName + "\n" +
            "Level: " + level + "\n" +
            "Exp: " + exp + "\n" +
            "Health: " + health + "\n" +
            "Current position: " + currentPosition + "\n" 
            );
    }
    
}
