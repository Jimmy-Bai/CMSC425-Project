using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour {
    // VARIABLES //
    [Header("Game Object")]
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerLevel;
    public Image expBar;
    public Image hpBar;

    private ObjectStats playerStats;
    // FUNCTION //
    // Initialize player UI with information
    private void Start() {
        playerStats = ControllerPlayer.Instance.GetComponent<ObjectStats>();

        // Prefill UI
        playerName.text = DataMain.Current.playerData.playerName;
        playerLevel.text = "Lv: " + DataMain.Current.playerData.level;
        expBar.fillAmount = DataMain.Current.playerData.expRatio;
        hpBar.fillAmount = (float)playerStats.currentHealth / (float)playerStats.maxHealth;


    }

    private void LateUpdate() {
        if (DataMain.Current.playerData.leveledUp) {
            playerLevel.text = "Lv: " + DataMain.Current.playerData.level;

            DataMain.Current.playerData.leveledUp = false;
        }

        if (DataMain.Current.playerData.gainedExp) {
            expBar.fillAmount = DataMain.Current.playerData.expRatio;

            DataMain.Current.playerData.gainedExp = false;
        }

        hpBar.fillAmount = (float)playerStats.currentHealth / (float)playerStats.maxHealth;
    }
}
