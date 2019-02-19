using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputSelection : MonoBehaviour {
    // VARIABLES //
    [Header("Game Object")]
    public EventSystem eventSystem;
    public GameObject selectedButton;

    private bool buttonSelected;
    // FUNCTIONS //	
	// Update is called once per frame
	void Update () {
        if (Input.GetAxis("Vertical") != 0 && !buttonSelected) {
            eventSystem.SetSelectedGameObject(selectedButton);
            buttonSelected = true;
        }
	}

    private void OnDisable() {
        buttonSelected = false;
    }
}
