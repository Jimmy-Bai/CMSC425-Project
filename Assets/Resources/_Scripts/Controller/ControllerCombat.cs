using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectStats))]
public class ControllerCombat : MonoBehaviour {
    // VARIABLES //
    [Header("Game Settings")]
    public float attackSpeed = 1.0f;
    public float attackDelay = .6f;

    private float attackCooldown;
    private ObjectStats objectStats;

    // FUNCTIONS //
    private void Start() {
        objectStats = GetComponent<ObjectStats>();
    }

    private void Update() {
        attackCooldown -= Time.deltaTime;
    }

    // Attack function
    public void Attack (ObjectStats targetStats) {
        if (attackCooldown <= 0) {
            StartCoroutine(AttackDelay(targetStats, attackDelay));
            attackCooldown = 1 / attackSpeed;
        }
    }

    IEnumerator AttackDelay (ObjectStats stats, float delay) {
        yield return new WaitForSeconds(delay);

        stats.TakeDamage(objectStats.attack.GetValue());
    }
}
