using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableQuaternion {
    // VARIABLES //
    private float x, y, z, w;

    // FUNCTIONS //
    // Constructor
    public SerializableQuaternion(float x, float y, float z, float w) {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    // Overriding the ToString() method
    public override string ToString() {
        return string.Format("[{0}, {1}, {2}]", x, y, z);
    }

    // Automatic conversion from SerializableVector3 to Vector3
    public static implicit operator Quaternion(SerializableQuaternion input) {
        return new Quaternion(input.x, input.y, input.z, input.w);
    }

    // Automatic conversion from Vector3 to SerializableVector3
    public static implicit operator SerializableQuaternion(Quaternion input) {
        return new SerializableQuaternion(input.x, input.y, input.z, input.w);
    }
}
