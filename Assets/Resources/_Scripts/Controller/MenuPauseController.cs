using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MenuPauseController : MonoBehaviour {
    // VARIABLES //
    [Header("Game Object")]
    public GameObject player;
    public SceneUtilities sceneFader;

    private NavMeshAgent playerAgent;
    // FUNCTIONS //
    // Save game
    public void SaveGame() {
        playerAgent = player.GetComponent<NavMeshAgent>();

        // Update character position to player data and player stats
        DataMain.Current.playerData.UpdateCurrentPosition(playerAgent.transform.position);
        DataMain.Current.playerData.SaveStats();

        // Save enemy spawn position if player is in dungeon scene
        if (SceneManager.GetActiveScene().buildIndex == 2) {
            ControllerEnemySpawn.Instance.SaveEnemyPosition();
        }

        GameSystemUtilities.Save();
    }

    // Go back to main menu
    public void GoToMenu() {
        Time.timeScale = 1.0f;

        DataMain.Current.IsLoadedGame = true;
        GameSystemUtilities.Save();

        sceneFader.FadeToLevel(0);
    }

    // Quit game
    public void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quite();
#endif
    }

    // When user exits the program, set IsLoadedGame to true
    private void OnApplicationQuit() {
        DataMain.Current.IsLoadedGame = true;

        GameSystemUtilities.Save();
    }
}
