using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCamera : MonoBehaviour {
    // VARIABLES //
    [Header("Game Object")]
    public Transform player;                    // Player

    [Header("Camera Settings")]
    public Vector3 targetOffset;                // Player position offset

    [Header("Zoom Settings")]
    public float distance = 5.0f;               // Distance between player and camera
    public float maxZoomDistance = 100.0f;      // Maximum zoom distance
    public float minZoomDistance = 20.0f;       // Minimum zoom distance
    public float mouseZoomRate = 40;            // Mouse zooming rate
    public float joystickZoomRate = 1;          // Joystick zooming rate
    public float zoomDampening = 10.0f;          // Zoom dampening

    [Header("Rotation Settings")]
    public float yMinLimit = 0.8f;              // Rotation min limit
    public float yMaxLimit = 80.0f;             // Rotation max limit
    public float rotationSpeed = 225.0f;        // Rotation speed
    public float rotationDampening = 15.0f;      // Rotation dampening

    [Header("Switch camera controller")]
    public bool InTown;                         // Switch controller depending if the player is in town or not

    [Header("Town camera controller setting")]
    public Vector3 cameraOffset;                // Camera offset
    public float offsetHeight;                  // Offset height from player position
    public float smoothSpeed;                   // Smooth transition speed
    public float minTownZoom = 50.0f;           // Minimum zoom distance in town
    public float maxTownZoom = 200.0f;          // Maximum zoom distance in town
    public float townZoomSpeed = 25;            // Zooming rate in town

    private float xDeg = 0.0f, yDeg = 0.0f;
    private float currentDistance, desiredDistance;
    private float rotationMin;
    private float zoomDirection;
    private Quaternion currentRotation, desiredRotation, rotation;
    private Vector3 desiredInputPosition, currentInputPosition, currentTargetPosition, position;
    private Transform target;

    // FUNCTIONS //
    private void Start() {
        // Set target to player
        target = player;

        // Set distance
        distance = Vector3.Distance(transform.position, target.position);
        currentDistance = distance;
        desiredDistance = distance;

        // Look camera at target
        //transform.LookAt(target, Vector3.down);

        // Set position and rotation
        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        // Set mouse input degree
        xDeg = Vector3.Angle(Vector3.right, transform.right);
        yDeg = Vector3.Angle(Vector3.up, transform.up);

        if (transform.position.y < target.position.y) {
            yDeg *= -1;
        }
    }

    private void LateUpdate() {
        float newOffsetZ;

        // In town camera controller
        if (InTown) {
            Vector3 nextPosition = target.position + cameraOffset;
            Vector3 smoothPosition = Vector3.Lerp(transform.position, nextPosition, smoothSpeed);
            transform.position = smoothPosition;

            // Look at player
            transform.LookAt(target.position + Vector3.up * offsetHeight);

            // Camera zoom
            if (Input.GetAxis("Mouse ScrollWheel") != 0) {
                newOffsetZ = Mathf.Clamp(cameraOffset.z + Input.GetAxis("Mouse ScrollWheel") * townZoomSpeed, minTownZoom, maxTownZoom);
                cameraOffset = new Vector3(cameraOffset.x, cameraOffset.y, newOffsetZ);
            }

            // Joystick zoom
            // Get input from joy stick
            if (Input.GetButton("Zoom In")) {
                zoomDirection = 1;
            } else if (Input.GetButton("Zoom Out")) {
                zoomDirection = -1;
            } else {
                zoomDirection = 0;
            }

            newOffsetZ = Mathf.Clamp(cameraOffset.z + zoomDirection * joystickZoomRate, minTownZoom, maxTownZoom);
            cameraOffset = new Vector3(cameraOffset.x, cameraOffset.y, newOffsetZ);

            return;
        }

        // Get input from mouse
        if (Input.GetMouseButton(1)) {
            xDeg += Input.GetAxis("Mouse X") * rotationSpeed * 0.02f;
            yDeg -= Input.GetAxis("Mouse Y") * rotationSpeed * 0.02f;
        }
        
        // Get input from joystick
        xDeg += Input.GetAxis("Joystick X") * rotationSpeed * 0.02f;
        yDeg -= Input.GetAxis("Joystick Y") * rotationSpeed * 0.02f;

        // Set camera rotation
        // Clamp the vertical axis for the orbit
        rotationMin = -Mathf.Rad2Deg * Mathf.Asin((target.position.y - yMinLimit) / currentDistance);
        yDeg = ClampAngle(yDeg, rotationMin, yMaxLimit);

        // Set camera rotation
        desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
        currentRotation = transform.rotation;

        rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * rotationDampening);
        transform.rotation = rotation;

        // Set camera position
        // Get input from joy stick
        if (Input.GetButton("Zoom In")) {
            zoomDirection = 1;
        }else if (Input.GetButton("Zoom Out")) {
            zoomDirection = -1;
        }else {
            zoomDirection = 0;
        }

        // Get input from mouse
        if (zoomDirection == 0) {
            desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * mouseZoomRate * Mathf.Abs(desiredDistance);
        } 
        
        desiredDistance -= zoomDirection * Time.deltaTime * joystickZoomRate * Mathf.Abs(desiredDistance);
        
        //  Clamp min and max zoom distance and smooth of the zoom, lerp distance
        desiredDistance = Mathf.Clamp(desiredDistance, minZoomDistance, maxZoomDistance);
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

        // Calculate position based on the new currentDistance 
        currentTargetPosition = target.position;
        currentInputPosition = Vector2.Lerp(currentInputPosition, desiredInputPosition, Time.deltaTime * 5f);///

        // Set camera posistion
        position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
        transform.position = position;
    }

    // In town camera


    // Given an angle and a minimum and maximum angle, clamp the given angle between min and max
    private float ClampAngle(float angle, float min, float max) {
        if (angle < -360) {
            angle += 360;
        }

        if (angle > 360) {
            angle -= 360;
        }

        return Mathf.Clamp(angle, min, max);
    }
}
