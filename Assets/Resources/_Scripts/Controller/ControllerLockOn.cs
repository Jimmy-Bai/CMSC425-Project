using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerLockOn : MonoBehaviour {
    // VARIABLES //
    [Header("Game Objects")]
    public List<Transform> targetList;
    
    // FUNCTIONS //
	// Use this for initialization
	private void Start () {
        targetList = new List<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
