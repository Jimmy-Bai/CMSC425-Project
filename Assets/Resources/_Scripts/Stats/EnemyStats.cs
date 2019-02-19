using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

[System.Serializable]
public class EnemyStats : ObjectStats {
    // VARIABLES //
    [Header("Game object")]
    public GameObject explodeEffect;

    [Header("Game setting")]
    public float shakeRoughness;
    public float shakeMagnitude;
    public float shakeFadeInTime;
    public float shakeFadeOutTime;
    public int expBase = 2;
    public float expMod = 1.5f;
    public bool DebugMode = false;
    public bool shrinkEffect = true;

    // FUNCTIONS //
    public override void Die() {
        base.Die();

        // Play death animation and shrink effect if applicable
        GameObject effect = (GameObject)Instantiate(explodeEffect, transform.position + 2.5f * Vector3.up, transform.rotation);

        if (shrinkEffect) {
            effect.transform.localScale = new Vector3(.5f, .5f, .5f);
        }

        // Shake camera
        CameraShaker.Instance.ShakeOnce(shakeMagnitude, shakeRoughness, shakeFadeInTime, shakeFadeOutTime);

        // Deactive game object before destorying 
        gameObject.SetActive(false);
        Destroy(effect, 3);

        // Player gain exp
        if (!DebugMode) {
            PlayerGainedExp();
        }

        Destroy(gameObject);
    }

    // Calculate how much exp player gets 
    // This takes into account the floor number the player is on
    private void PlayerGainedExp() {
        int enemeyExp = 50 + (int)Mathf.Floor(expBase * Mathf.Pow(expMod, DataMain.Current.playerData.currentFloorLevel));
        DataMain.Current.playerData.GainExp(enemeyExp);
    }
}
