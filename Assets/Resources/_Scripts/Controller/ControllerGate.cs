using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControllerGate : MonoBehaviour {
    // VARIABLES //
    private bool trigger = false;

    // Game Objects
    [Header("Game Object")]
    public Image sprite;                // Interactable sprite
    public GameObject buttonPrefabs;    // Buttons prefabs
    public GameObject levelSelectMenu;  // Level selector menu
    public GameObject playerUI;         // Player UI
    public RectTransform parentPanel;   // Button's parent panel
    public SceneUtilities sceneFader;   // Scene fader 
    public bool InTown;                 // Determined if the gate is inside town or in floor

    [Header("Game Object")]
    public Button nextFloor;
    public Button previousFloor;
    public Button backToTown;

    private int currentFloor;           // Store max floor number player reached
    private int maxFloorNumber;         // Stores current floor number player is on

    // FUNCTIONS //
    // Initialization
    private void Start() {
        // Set initial alpha to 0
        sprite.CrossFadeAlpha(0, 0, false);

        // Set currentFloor and max FloorNumber
        currentFloor = DataMain.Current.playerData.currentFloorLevel;
        maxFloorNumber = DataMain.Current.playerData.maxFloorLevel;
    }

    // Check each frame
    private void Update() {
        if (trigger) {
            sprite.CrossFadeAlpha(1, .2f, false);

            if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Interact")) {
                Toggle();

                if (levelSelectMenu.activeSelf) {
                    // Create basic menu
                    CreateBasicMenu();

                    if (InTown) {
                        CreateWrappingMenu();
                    }
                }else {
                    // Destroy all previous buttons first
                    foreach (Transform child in parentPanel.transform) {
                        if (child != nextFloor.transform && child != previousFloor.transform && child != backToTown.transform) {
                            GameObject.Destroy(child.gameObject);
                        }
                    }
                }
            }
        }else {
            sprite.CrossFadeAlpha(0, .2f, false);
        }
    }
    // Create basic level select menu
    private void CreateBasicMenu() {
        // Next floor button
        if (DataMain.Current.playerData.CheckDirection(true)) {
            nextFloor.onClick.AddListener(() => SelectLevel(true));
        } else {
            nextFloor.interactable = false;
        }

        // Prev floor button
        if (DataMain.Current.playerData.CheckDirection(false)) {
            previousFloor.onClick.AddListener(() => SelectLevel(false));
        } else {
            previousFloor.interactable = false;
        }

        // Back to town button
        if (currentFloor != 0) {
            backToTown.onClick.AddListener(() => WrapToLevel(0));
        } else {
            backToTown.interactable = false;
        }
    }

    // Create teleporting level select menu
    private void CreateWrappingMenu() {
        Button buttonObject;

        // Instantiate buttons
        for (int i = 1; i <= maxFloorNumber; i++) {
            buttonObject = CreateSingleButton();
            SetButtonText(buttonObject, i);

            // Set listener
            int tempFloorNumber = i;
            buttonObject.onClick.AddListener(() => WrapToLevel(tempFloorNumber));

            // If floor number equals to thee current floor player is in, set button to not interactable
            if (i == currentFloor) {
                buttonObject.interactable = false;
            }
        }
    }

    // Instantiate a single button and return that button
    public Button CreateSingleButton() {
        Button newButton = Instantiate(buttonPrefabs).GetComponent<Button>();

        // Set parents and scale
        newButton.transform.SetParent(parentPanel, false);
        newButton.transform.localScale = new Vector3(1, 1, 1);

        return newButton;
    }

    // Set button text
    public void SetButtonText(Button button, int floor) {
        string text = "";

        if (floor == 0) {
            text = "Town";
        }else {
            text = "Level " + floor;
        }

        button.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    // Toggle menu activty and time scale
    private void Toggle() {
        // Set parent active
        levelSelectMenu.SetActive(!levelSelectMenu.activeSelf);
        playerUI.SetActive(!playerUI.activeSelf);

        // Pause and unpause game
        if (levelSelectMenu.activeSelf) {
            Time.timeScale = 0f;
        } else {
            Time.timeScale = 1f;
        }
    }

    // Go to next/previous level
    public void SelectLevel(bool direction) {
        DataMain.Current.playerData.UpdateCurrentFloor(direction);
        sceneFader.FadeToLevel(DataMain.Current.playerData.GetSceneIndex());
        Toggle();
    }

    // Wrap to given floor
    public void WrapToLevel(int floor) {
        DataMain.Current.playerData.WrapToFloor(floor);
        sceneFader.FadeToLevel(DataMain.Current.playerData.GetSceneIndex());
        Toggle();
    }

    // Check if player enters the trigger
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            trigger = true;
        }
    }

    // Check if player exits the trigger
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            trigger = false;
        }
    }
}
