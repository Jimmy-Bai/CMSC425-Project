using UnityEngine;

public class Interactable : MonoBehaviour {
    // VARIABLES //
    [Header("Game Setting")]
    public float radius = 3.0f;

    private bool hasInteracted = false;

    // FUNCTIONS //
    public virtual void Interact() {

    }

    private void Update() {
        if (!hasInteracted) {

        }
    }

    // Debug only
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
