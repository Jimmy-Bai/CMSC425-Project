using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectStats : MonoBehaviour {
    // VARIABLES //
    [Header("Game Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public Stats attack;
    public Stats defence;

    public event System.Action<int, int> OnHealthChanged;

    // FUNCTIONS //
    private void Awake() {
        currentHealth = maxHealth;
    }

    // Take damage
    public void TakeDamage(int damage) {
        damage = Mathf.Max(damage - defence.GetValue(), 0);
        currentHealth = Mathf.Max(currentHealth - damage, 0);

        if (OnHealthChanged != null) {
            OnHealthChanged(maxHealth, currentHealth);
        }

        if (currentHealth <= 0) {
            Die();
        }
    }

    // Method to be overriden
    public virtual void Die() { }
}
