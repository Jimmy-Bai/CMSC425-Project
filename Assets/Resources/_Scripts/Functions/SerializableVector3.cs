using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SerializableVector3 {
    // VARIABLES //
    private float x, y, z;

    // FUNCTIONS //
    // Constructor
    public SerializableVector3(float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    // Overriding the ToString() method
    public override string ToString() {
        return string.Format("[{0}, {1}, {2}]", x, y, z);
    }

    // Automatic conversion from SerializableVector3 to Vector3
    public static implicit operator Vector3(SerializableVector3 input) {
        return new Vector3(input.x, input.y, input.z);
    }

    // Automatic conversion from Vector3 to SerializableVector3
    public static implicit operator SerializableVector3(Vector3 input) {
        return new SerializableVector3(input.x, input.y, input.z);
    }
}
