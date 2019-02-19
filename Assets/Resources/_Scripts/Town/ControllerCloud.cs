using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCloud : MonoBehaviour {
    // VARIABLES //
    // Starting point and endpoint
    public float startX, endX;
    public float maxSpeed;

    private List<Cloud> cloudList;

    // FUNCTIONS //
    // Initialization
    private void Start() {
        Cloud current;
        cloudList = new List<Cloud>();

        // Generate a list of cloud
        foreach (Transform child in transform) {
            current = new Cloud(child);
            cloudList.Add(current);
        }
    }

    private void Update() {
        MoveCloud();
    }

    // Move each cloud toward the the endpoint
    // If cloud reached the endpoint, cloud is position at starting point and moves again
    private void MoveCloud() {
        foreach (Cloud current in cloudList) {
            if (IsAtEnd(current.cloud.position)) {
                current.cloud.position = Relocate(current.cloud.position);
            } else {
                current.cloud.position += Vector3.right * maxSpeed * Time.deltaTime * current.multiplier;
            }
        }
    }

    // Check if cloud is at endpoint yet
    private bool IsAtEnd(Vector3 position) {
        return position.x >= endX;
    }

    // Relocate cloud to starting point
    private Vector3 Relocate(Vector3 position) {
        return new Vector3(startX, position.y, position.z);
    }
}

class Cloud {
    // VARIALBES //
    public Transform cloud;
    public float multiplier; 

    public Cloud(Transform child) {
        cloud = child;
        multiplier = GenerateRandomMuliplier();
    }

    public float GenerateRandomMuliplier() {
        System.Random rand = new System.Random();

        return rand.Next(1, 21) * 0.05f;
    }
}
