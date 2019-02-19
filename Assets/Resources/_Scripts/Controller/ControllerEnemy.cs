using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControllerEnemy : MonoBehaviour {
    // VARIABLE //
    [Header("Game Object")]
    public GameObject hitEffect;
    public GameObject effect;
    public GameObject attackEffect;
    public GameObject enemyEffect;

    [Header("Game Settings")]
    [Header("Player Search Settings")]
    public float lookRadius = 15f;
    public float smoothSpeed = 5.0f;

    [Header("Enemy Movement Settings")]
    public float knockbackDuration = 0.2f;
    public float knockbackDistance = 8.0f;
    public float knockbackVelocity = 20.0f;

    [Header("Enemy Attack Settings")]
    public float attackChargeTime = 4.0f;
    public float attackCooldown = 4.0f;
    public float attackRadius = 5.0f;
    public float attackDuration = 3.0f;

    [Header("Others")]
    public bool shrinkEffect = true;

    private NavMeshAgent enemyAgent;
    private NavMeshObstacle enemyObstacle;
    private Animator attackAnimator;
    private Animator enemyAnimator;
    private ControllerCombat playerCombat, enemyCombat;
    private ObjectStats enemyStats, playerStats;
    private Transform playerTransform;
    private bool finishedAttack, attackCharged, chargingAttack, isAttacking;
    private bool coroutineStarted, beingKnockback;

    // FUNCTIONS //
    // Use this for initialization
    void Start() {
        // Get navmesh objects
        enemyAgent = GetComponent<NavMeshAgent>();
        enemyObstacle = GetComponent<NavMeshObstacle>();

        // Get animator
        attackAnimator = attackEffect.GetComponent<Animator>();
        enemyAnimator = enemyEffect.GetComponent<Animator>();

        // Get enemy and player's stats and combat controller
        enemyCombat = GetComponent<ControllerCombat>();
        playerCombat = ControllerPlayer.Instance.GetComponent<ControllerCombat>();
        enemyStats = GetComponent<ObjectStats>();
        playerStats = ControllerPlayer.Instance.GetComponent<ObjectStats>();

        // Get player transform
        playerTransform = ControllerPlayer.Instance.transform;
    }

    // Update is called once per frame
    void Update() {
        // Enemy move
        EnemyMove();

        // Enemy get's attack
        EnemyGetsAttack();
        
        // Enemy attacking
        EnemyAttack();
    }
    
    // Enemy gets attack
    // 1. Check if player is inside of enemy's stopping distance
    // 2. If player press attack button, enemy get's attack
    // 3. If enemy is not attacking and it's being knocked back, enemy gets knock back
    private void EnemyGetsAttack() {
        float distance = Vector3.Distance(ControllerPlayer.Instance.transform.position, transform.position);

        // Player can attack enemy if player is within enemy's stopping distance
        if (distance <= enemyAgent.stoppingDistance) {
            // Face target
            FaceTarget();

            // Get's attack
            if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Attack")) {
                GameObject effectIn = Instantiate(hitEffect, transform.position + Vector3.up * 2.5f, transform.rotation);

                if (shrinkEffect) {
                    effectIn.transform.localScale = new Vector3(.35f, .35f, .35f);
                }

                Destroy(effectIn, 2);

                playerCombat.Attack(enemyStats);

                // When the enemy is not attacking, player can knock enemy back
                if (!isAttacking && !beingKnockback) {
                    // Get normalized knockback direction
                    Vector3 knockbackDirection = (enemyAgent.transform.position - playerTransform.forward).normalized;
                    enemyAgent.velocity = knockbackDirection * knockbackVelocity;

                    StartCoroutine(Knockback());
                }
            }
        }
    }

    // Chase after player
    // 1. Check if player is inside of enemy's look radius
    // 2. If enemy is not attacking, enemy moves toward player
    private void EnemyMove() {
        float distance = Vector3.Distance(ControllerPlayer.Instance.transform.position, transform.position);

        // Chase player if distance is within lookRadius
        if (distance <= lookRadius) {
            // Enemy stop moving when enemy is attacking
            if (!isAttacking) {
                enemyAgent.SetDestination(ControllerPlayer.Instance.transform.position);
            }
        }
    }

    // Attack player
    // 1. Check if player is close enough to enemy, if it is and enemy is not attacking already, start charging attack
    // 2. Charge attack, while charging attack, enemy will not move
    // 3. After attack is charged, check distance between player and enemy again
    // 4. If player is within enemy's attack range, enemy will deal damage to player, otherwise attack will miss
    // 5. After attacking, enemy's attack is not charged, and enemy is not attacking anymore, enemy can start to move
    // 6. Enemy cannot attack again until the attack cooldown. After cooldown, go back to step 1
    private void EnemyAttack() {
        float distance = Vector3.Distance(ControllerPlayer.Instance.transform.position, transform.position);

        // If player is close enough to enemy and enemy is already not attacking, start charging attack
        if (distance <= attackRadius && !isAttacking) {
            if (!chargingAttack) {
                // Play enemy charge animation
                enemyAnimator.SetTrigger("enemyIn");

                chargingAttack = true;
                isAttacking = true;

                // Stop enemy agent on it's path
                enemyAgent.isStopped = true;

                StartCoroutine(ChargeAttack(attackChargeTime));
            }
        }

        // Attack is charged
        if (attackCharged) {
            // Check distance between player and enemy again
            distance = Vector3.Distance(ControllerPlayer.Instance.transform.position, transform.position);

            // If player is within enemy's attack range and the attack is charged, attack player
            // Attack for given attack duration
            if (!coroutineStarted) {
                StartCoroutine(AttackDuration(attackDuration));
                coroutineStarted = true;

                // Play attack out animation
                attackAnimator.SetTrigger("AttackIn");
            }

            if (!finishedAttack) {
                // Attack
                if (distance <= attackRadius) {
                    enemyCombat.Attack(playerStats);
                }
            }else {
                // Attack is not charged anymore
                // Enemy is not attacking anymore
                // Coroutine is not started any more
                attackCharged = false;
                coroutineStarted = false;
                isAttacking = false;

                // Play attack out animation and enemy out animation
                attackAnimator.SetTrigger("AttackOut");
                enemyAnimator.SetTrigger("enemyOut");

                // Start enemy agent on it's path
                enemyAgent.isStopped = false;

                // Attack cooldown before enemy can start charging attack again
                StartCoroutine(AttackCooldown(attackCooldown));
            }
        }
    }

    // Knockback
    // Enemy can get knock back when it's moving
    IEnumerator Knockback() {
        beingKnockback = true;

        // Knockback
        enemyAgent.speed = 40;
        enemyAgent.angularSpeed = 0;
        enemyAgent.acceleration = 50;

        yield return new WaitForSeconds(knockbackDuration);

        // Undo knockback
        enemyAgent.speed = 20;
        enemyAgent.angularSpeed = 120;
        enemyAgent.acceleration = 10;

        // Set being knockback to false
        beingKnockback = false;
    }

    // Turn toward player and face player
    private void FaceTarget() {
        Vector3 direction = (ControllerPlayer.Instance.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * smoothSpeed);
    }

    #region Coroutines

    // Attack duration coroutine
    IEnumerator AttackDuration(float duration) {
        float time = 0f;

        while (time < duration) {
            time += Time.deltaTime;

            yield return null;
        }

        finishedAttack = true;
    }

    // Attack cooldown coroutine
    IEnumerator AttackCooldown(float cooldownTime) {
        float time = 0f;

        while (time < cooldownTime) {
            time += Time.deltaTime;

            yield return null;
        }

        chargingAttack = false;
    }

    // Charge attack coroutine
    IEnumerator ChargeAttack(float chargeTime) {
        float time = 0f;

        while (time < chargeTime) {
            time += Time.deltaTime;

            yield return null;
        }

        finishedAttack = false;
        attackCharged = true;
    }

    #endregion

    // DEBUG ONLY //
    private void OnDrawGizmosSelected() {
        // Draw enemy detect radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);

        // Draw player attack radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        // Draw player attack radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, enemyAgent.stoppingDistance);
    }
}
