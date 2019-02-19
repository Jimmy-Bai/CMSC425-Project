using UnityEngine;

[System.Serializable]
public class PlayerStats : ObjectStats {
    // FUNCTIONS //
    public override void Die() {
        base.Die();
        // Maybe transport back to down
    }
}
