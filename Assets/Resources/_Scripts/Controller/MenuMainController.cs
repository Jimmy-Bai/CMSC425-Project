using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class MenuMainController : MonoBehaviour {
    // VARIABLES //
    // Transition between fromCanvas menu to toCanvas menu
    private CanvasGroup fromCanvas, toCanvas;
    public GameObject from, to;
    public Transform button;
    public SceneUtilities sceneFader;

    // Alpha decresase/increase rate
    private readonly float ALPHA = 0.1f;

    // FUNCTIONS //
    // Fade effects and delay scripts
    public void Transition() {
        StartCoroutine(StartTransition(from, to));
    }

    IEnumerator StartTransition(GameObject from, GameObject to) {
        fromCanvas = from.GetComponent<CanvasGroup>();
        toCanvas = to.GetComponent<CanvasGroup>();

        for (float f = 0.0f; f <= 1; f += ALPHA) {

            fromCanvas.alpha -= ALPHA;
            yield return new WaitForSeconds(0.025f);
        }

        to.SetActive(true);

        for (float f = 0.0f; f <= 1; f += ALPHA) {

            toCanvas.alpha += ALPHA;
            yield return new WaitForSeconds(0.025f);
        }

        from.SetActive(false);
    }


    // Create new character with given name
    public void CreateNewSave(TextMeshProUGUI input) {
        string playerName = input.text;

        if (!(playerName.Length - 1 == 0)) {
            // Create a new main data. This only happens if a new game is pressed
            DataMain.Current = new DataMain();

            // Set up player name then put index 0 in level array to true
            DataMain.Current.playerData.playerName = playerName;

            // Save main data to save file, then load scene
            GameSystemUtilities.Save();

            // Load Scene
            sceneFader.FadeToLevel(DataMain.Current.playerData.GetSceneIndex());

            Debug.Log("New Game");
        }
    }

    // Continue game
    public void ContinueSaveGame() {
        // Load save game from save file 
        GameSystemUtilities.Load();

        // Get correct scene index then load that scene
        // 1 being town and 2 being 
        sceneFader.FadeToLevel(DataMain.Current.playerData.GetSceneIndex());

        Debug.Log("Continue");
    }

    // Exit scripts
    public void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quite();
#endif
    }
}
