using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 1. Directional movment
// 2. Stop and face current direction when input is absent
public class ControllerPlayer : MonoBehaviour {
    // VARIABLES //
    [Header("Player movement stats")]
    [SerializeField]
    private float velocity;         // Player velocity
    public float walkVelocity;      // Maximum walk velocity player is allow
    public float runVelocity;       // Maximum run velocity player is allow
    public float dashVelocity;      // Maximum dash velocity player is allow
    public float dashDuration;      // Duration player is allow to dash
    public float acceleration;      // Player acceleration
    public float deceleration;      // Player deceleration
    public float turnSpeed;         // Player turn speed
    
    private Quaternion targetRotation;
    private Transform cameraDirection, currentTransform;
    private NavMeshAgent playerAgent;
    private Vector2 input;
    private float angle;
    private float previousX, previousY;
    private Animator animator;
    private ObjectStats playerStats;

    public static ControllerPlayer Instance = null;

    // FUNCTIONS //
    // Initialization
    private void Awake() {
        Instance = this;

        playerAgent = GetComponent<NavMeshAgent>();
        cameraDirection = Camera.main.transform;
        currentTransform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        playerStats = GetComponent<ObjectStats>();
    }

    // Update player data with player stats
    private void Start() {
        DataMain.Current.playerData.attack = playerStats.attack;
        DataMain.Current.playerData.defence = playerStats.defence;
        DataMain.Current.playerData.maxHealth = playerStats.maxHealth;
        DataMain.Current.playerData.health = playerStats.currentHealth;
    }

    // For attacking
    // For moving
    private void FixedUpdate() {
        Vector3 nextPosition;

        GetInput();
        animator.SetFloat("BlendX", (float)(velocity / walkVelocity) * previousX);
        animator.SetFloat("BlendY", (float)(velocity / walkVelocity) * previousY);

        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Attack")) {
            return;
        }

        // Does nothing if no input was given
        if (Mathf.Abs(input.x) < 1 && Mathf.Abs(input.y) < 1) {
            // Decelerate and keep moving forward
            ChangeSpeed(false);
            nextPosition = transform.forward * velocity * Time.deltaTime;
            playerAgent.Move(nextPosition);

            animator.SetFloat("BlendX", (float)(velocity / walkVelocity) * previousX);
            animator.SetFloat("BlendY", (float)(velocity / walkVelocity) * previousY);

            return;
        }

        // Accelerate, calulate direction base on camera and rotate toward said direction
        ChangeSpeed(true);
        CalculateDirection();
        Rotate();

        nextPosition = transform.forward * velocity * Time.deltaTime;

        playerAgent.Move(nextPosition);
    }

    // Get keyboard input from player
    private void GetInput() {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        if (!(Mathf.Abs(input.x) < 1) || !(Mathf.Abs(input.y) < 1)) {
            previousX = input.x;
            previousY = input.y;
        }
    }
    
    // Calculate the direction player will be facing
    private void CalculateDirection() {
        angle = Mathf.Atan2(input.x, input.y);
        angle = Mathf.Rad2Deg * angle;
        angle += cameraDirection.eulerAngles.y;
    }

    // Rotates toward angle 
    private void Rotate() {
        targetRotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    // Accelerate or deccelerate
    private void ChangeSpeed(bool state) {
        if (state && velocity <= walkVelocity) {
            // Accelereate
            velocity += acceleration * Time.deltaTime;
        }else if (!state && velocity > 0) {
            float newVelocity = velocity - deceleration * Time.deltaTime;
            velocity = Mathf.Max(newVelocity, 0);
        }
    }
    
    // Calculate the signed angle between two vector
    private float SignedAngle(Vector3 a, Vector3 b) {
        return Vector3.Angle(a, b) * Mathf.Sign(Vector3.Cross(a, b).y);
    }
}
