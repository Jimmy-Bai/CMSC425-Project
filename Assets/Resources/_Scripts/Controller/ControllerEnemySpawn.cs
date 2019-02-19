using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControllerEnemySpawn : MonoBehaviour {
    // VARIABLE //
    [Header("Game Settings")]
    public int enemyPerPlatform = 2;

    [Header("Game Objects")]
    public GameObject enemy;
    public GameObject portal;

    private List<Vector2> endpointList;
    private List<Vector3> spawnPositionList;
    private int totalEnemyCount;
    private ObjectStats enemyStat;
    private bool spawnPortal = true;

    public static ControllerEnemySpawn Instance = null;
    public bool EnemyIsDone;

    // FUNCTIONS
    // Use this to initialize Instance
    private void Awake() {
        Instance = this;    
    }

    // Use this for initialization
    void Start () {
        // Get endpoint list from platform controller or from save data
        BuildPositionList();

        // Spawn enemy
        SpawnEnemy();  
	}
    
    // Spawn enemy from position list
    private void SpawnEnemy() {
        foreach (Vector3 position in spawnPositionList) {
            GameObject enemyObject = Instantiate(enemy, position, Quaternion.Euler(new Vector3(0, 0, 0)), transform);

            // Scale down enemey
            enemyObject.transform.localScale = new Vector3(.25f, .25f, .25f);

            // Wrapped enemy to correct location
            NavMeshAgent enemyAgent = enemyObject.GetComponent<NavMeshAgent>();
            enemyAgent.Warp(position);

            // Update enemy stats depending on level
            enemyStat = enemyObject.GetComponent<ObjectStats>();
            enemyStat.maxHealth = (int)Mathf.Floor(100 * Mathf.Pow(1.3f, DataMain.Current.playerData.currentFloorLevel));
            enemyStat.defence = new Stats(5 * DataMain.Current.playerData.currentFloorLevel);
            enemyStat.attack = new Stats(5 + (int)Mathf.Floor(2 * Mathf.Pow(1.5f, DataMain.Current.playerData.currentFloorLevel)));
        }
    }

    // Initialize enemies at endpoint
    private void BuildPositionList() {
        spawnPositionList = new List<Vector3>();
        
        // Get position list from save file if this is a loaded game
        if (DataMain.Current.IsLoadedGame) {
            spawnPositionList = DataMain.Current.dungeonData.GetSpawnPointList();

            // Set enemy is done to true
            EnemyIsDone = true;

            return;
        }
       
        // Form new enemy position list
        endpointList = ControllerPlatform.Instance.GetEndpointList();

        foreach (Vector2 position in endpointList) {
            // Get position from endpoint list and calculate platform position
            Vector3 newPosition = new Vector3(position.x * 10, 0, position.y * 10);

            spawnPositionList.Add(newPosition);

        }
        Debug.Log("Inn here");
    }

    // Save all enemy position 
    public void SaveEnemyPosition() {
        List<Vector3> enemyPositionList = new List<Vector3>();

        foreach (Transform child in transform) {
            enemyPositionList.Add(child.transform.position);
        }

        // Create a new list in dungeon data and add this temporary list to dungeon data list
        DataMain.Current.dungeonData.EnemyPositionList = new List<SerializableVector3>();
        DataMain.Current.dungeonData.AddToSpawnList(enemyPositionList);
    }
	
	// Update is called once per frame
	void Update () {
		if (transform.childCount == 0 && spawnPortal) {
            portal.SetActive(!portal.activeSelf);
            StartCoroutine(MovePortal());
            spawnPortal = false;
        }
	}

    IEnumerator MovePortal() {
        float target = 30f;

        while (portal.transform.position.y <= target) {
            portal.transform.position = new Vector3(portal.transform.position.x, portal.transform.position.y + 75 * Time.deltaTime, portal.transform.position.z);

            yield return null;
        }

        portal.transform.position = new Vector3(portal.transform.position.x, target, portal.transform.position.z);

    }
}
