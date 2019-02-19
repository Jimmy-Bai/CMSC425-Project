using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats {
    // VARIABLES //
    [SerializeField]
    private int baseValue;

    // FUNCTIONS //
    public int GetValue() {
        return baseValue;
    }

    public Stats(int input) {
        baseValue = input;
    }
}
