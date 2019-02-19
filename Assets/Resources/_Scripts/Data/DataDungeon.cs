using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataDungeon {
    // VARIABLES //
    public List<SerializableVector2> EndpointList;
    public List<SerializableVector3> BGPositionList;
    public List<SerializableVector3> EnemyPositionList;
    public bool[,] boolLayout;

    // FUNCTIONS //
    // Constructor
    public DataDungeon() {
        EndpointList = new List<SerializableVector2>();
        BGPositionList = new List<SerializableVector3>();
    }

    // Generate a new endpointlist, and bgpositoinlist
    public void NewPlatList() {
        EndpointList = new List<SerializableVector2>();
        BGPositionList = new List<SerializableVector3>();
    }

    // Get list
    // Return a List<Vector2> converted from List<SerializableVector2>
    public List<Vector3> GetSpawnPointList() {
        List<Vector3> newPositionList = new List<Vector3>();

        foreach (SerializableVector3 sVector in EnemyPositionList) {
            newPositionList.Add(sVector);
        }

        return newPositionList;
    }

    // Return a List<Vector2> converted from List<SerializableVector2>
    public List<Vector2> GetEndpointList() {
        List<Vector2> newEndpointList = new List<Vector2>();

        foreach (SerializableVector2 sVector in EndpointList) {
            newEndpointList.Add(sVector);
        }

        return newEndpointList;
    }

    // Return a List<Vector2> converted from List<SerializableVector2>
    public List<Vector3> GetPositionList() {
        List<Vector3> newPositionList = new List<Vector3>();

        foreach (SerializableVector3 sVector in BGPositionList) {
            newPositionList.Add(sVector);
        }

        return newPositionList;
    }

    // Add list 
    // Given a List<Vector2>. add all the vectors to EndpointList
    public void AddToEndpointList(List<Vector2> input) {
        foreach (Vector2 vector in input) {
            EndpointList.Add(vector);
        }
    }

    // Given a List<Vector2>. add all the vectors to BGPositionList
    public void AddToPositionList(List<Vector3> input) {
        foreach (Vector3 vector in input) {
            BGPositionList.Add(vector);
        }
    }

    // Given a List<Vector3>. add all the vectors to EnemyPositionList
    public void AddToSpawnList(List<Vector3> input) {
        foreach (Vector3 vector in input) {
            EnemyPositionList.Add(vector);
        }
    }
}

// Serializable Vector2
[System.Serializable]
public struct SerializableVector2 {
    // VARIABLES //
    private float x, y;

    // FUNCTIONS //
    // Constructor
    public SerializableVector2(float x, float y) {
        this.x = x;
        this.y = y;
    }

    // Overriding the ToString() method
    public override string ToString() {
        return string.Format("[{0}, {1}]", x, y);
    }

    // Automatic conversion from SerializableVector3 to Vector3
    public static implicit operator Vector2(SerializableVector2 input) {
        return new Vector2(input.x, input.y);
    }

    // Automatic conversion from Vector3 to SerializableVector3
    public static implicit operator SerializableVector2(Vector2 input) {
        return new SerializableVector2(input.x, input.y);
    }
}