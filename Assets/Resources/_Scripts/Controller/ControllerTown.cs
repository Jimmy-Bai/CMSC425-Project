using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using TMPro;

public class ControllerTown : MonoBehaviour {
    // VARIABLES //
    [Header("Game Object")]
    public SceneUtilities sceneFader;       // Scene fader that changes scene
    public GameObject pauseMenu;            // Pause menu
    public GameObject player;               // Player game object
    public GameObject playerUI;             // Player UI
    public TextMeshProUGUI floorNumber;     // Text for floor number
    public AudioClip menuClose;             // Audio clip
    public AudioClip menuOpen;              // Audio clip

    [Header("Game Setting")]
    public Vector3 playerStartPosition;     // Starting player position
    public bool Debugger;                   // Turn debug menu on or off

    private NavMeshAgent playerAgent;
    private AudioSource audio;

    // FUNCTIONS //
    // Use this for initialization
    // For testing and debugging only
    private void Awake() {
        playerAgent = player.GetComponent<NavMeshAgent>();
        floorNumber.text = "Town";

        // Instantiate player
        InstantiatePlayer();

        // Get audio source
        audio = GetComponent<AudioSource>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Option")) {
            Toggle();
        }
    }


    // Toggle menu activty and time scale
    public void Toggle() {
        // Set parent active
        playerUI.SetActive(!playerUI.activeSelf);

        // Pause and unpause game
        if (!pauseMenu.activeSelf) {
            audio.PlayOneShot(menuOpen, 0.2f);
            StartCoroutine(FadeIn());
        } else {
            audio.PlayOneShot(menuClose, 0.2f);
            StartCoroutine(FadeOut());

            Time.timeScale = 1f;
        }
    }

    // Fade in
    IEnumerator FadeIn() {
        float alpha = pauseMenu.GetComponent<CanvasGroup>().alpha;
        pauseMenu.SetActive(!pauseMenu.activeSelf);

        while (alpha <= 1) {
            alpha += Time.deltaTime * 5;
            pauseMenu.GetComponent<CanvasGroup>().alpha = alpha;

            yield return null;
        }

        Time.timeScale = 0f;
    }

    // Fade out
    IEnumerator FadeOut() {
        float alpha = pauseMenu.GetComponent<CanvasGroup>().alpha;

        while (alpha >= 0) {
            alpha -= Time.deltaTime * 5;
            pauseMenu.GetComponent<CanvasGroup>().alpha = alpha;

            yield return null;
        }

        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }

    // Instantiate player
    private void InstantiatePlayer() {
        // If this is a loaded game, intantiate player at the saved position
        if (DataMain.Current.IsLoadedGame) {
            playerStartPosition = DataMain.Current.playerData.currentPosition;
            DataMain.Current.IsLoadedGame = false;
        }else if (DataMain.Current.IsNewGame) {
            // Else this is a new game, instantiate player at the house
            playerStartPosition = new Vector3(55, 20, 135);

            DataMain.Current.IsNewGame = false;
        }

        playerAgent.Warp(playerStartPosition);
        player.SetActive(!player.activeSelf);
    }

    // Debugger menu
    public void OnGUI() {
        // Turn on debugger only if Debugger is true
        if (!Debugger) return;

        GUILayout.BeginArea(new Rect(0, 0, Screen.width / 6, Screen.height));

        GUILayout.Label("Town");
        GUILayout.Label("Current floor: " + DataMain.Current.playerData.currentFloorLevel);
        GUILayout.Space(10);

        if (GUILayout.Button("Next")) {
            if (DataMain.Current.playerData.UpdateCurrentFloor(true)) {
                sceneFader.FadeToLevel(DataMain.Current.playerData.GetSceneIndex());
            }
        }

        if (GUILayout.Button("Previous")) {
            if (DataMain.Current.playerData.UpdateCurrentFloor(false)) {
                sceneFader.FadeToLevel(DataMain.Current.playerData.GetSceneIndex());
            }
        }

        GUILayout.Label("Saving and quiting");
        GUILayout.Space(10);

        if (GUILayout.Button("Save")) {
            // Update player position in player data and save player stats
            DataMain.Current.playerData.UpdateCurrentPosition(playerAgent.transform.position);
            DataMain.Current.playerData.SaveStats();

            GameSystemUtilities.Save();
        }

        if (GUILayout.Button("Quit")) {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quite();
#endif
        }

        GUILayout.EndArea();
    }
}
