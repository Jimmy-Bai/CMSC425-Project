using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControllerPlatform : MonoBehaviour {
    // VARIABLES //
    public int width, height, multiplier;
    public NavMeshSurface surface;

    private readonly List<Vector2> OFFSET = new List<Vector2>() {
        Vector2.down,
        Vector2.up,
        Vector2.left,
        Vector2.right
    };
       
    // Use for DFS
    private List<Vector2> visitedList = new List<Vector2>();
    private Stack<Vector2> potentialStack = new Stack<Vector2>();

    // DFS output, boolean layout converted from adjacencyList
    private List<Vector2>[] adjacencyList;
    private List<Vector2> endpointList;
    private List<Vector3> bgPositionList;
    private bool[,] boolLayout, bgBoolLayout;

    private GameObject[] prefabs;

    private System.Random rand = new System.Random();

    public static ControllerPlatform Instance = null;
    public bool PlatIsDone;

    // FUNCTIONS //
    // Constructor
    private void Awake() {
        Instance = this;
        BuildFullDungeon();
    }

    // Fully build dungeon
    public void BuildFullDungeon() {
        // Load in prefabs and initialize endpoint list
        // Initialize adjacency list
        prefabs = Resources.LoadAll<GameObject>("_Prefabs\\Dungeon\\Platform\\");
        adjacencyList = new List<Vector2>[width * height];
        endpointList = new List<Vector2>();
        bgPositionList = new List<Vector3>();

        BuildFullPlatform();
        surface.BuildNavMesh();
        BuildBGPlatform();
    }

    // Fully build a maze using DFS maze generation and platform generation
    public void BuildFullPlatform() {
        // 1. Build maze adjacency list using DFS
        // 2. Convert adjacency list to boolean layout and expand by multiplier
        // 3. Specify tile type and delete extra tile from boolean layout 
        // 4. Repeat 1 - 3 until there are atleast six endpoints
        // 5. Create new dungeon data
        // 6. Build platform with GameObject and save to DataDungeon

        // Load data if IsLoadedGame is true
        if (DataMain.Current.IsLoadedGame) {
            boolLayout = DataMain.Current.dungeonData.boolLayout;
            endpointList = DataMain.Current.dungeonData.GetEndpointList();
            bgPositionList = DataMain.Current.dungeonData.GetPositionList();

            // Generate new list in dungeon data
            DataMain.Current.dungeonData.NewPlatList();

            // Set PlatIsDone to true
            PlatIsDone = true;
        } else {
            // Generate new list in dungeon data
            DataMain.Current.dungeonData.NewPlatList();

            do {
                BuildMaze();
                bgBoolLayout = InitiateLayout(2);
                boolLayout = InitiateLayout(multiplier);
                BuildLayout();
                BuildSurroundingList();
            } while (endpointList.Count < 6);
        }

        BuildPlatform();
    }

    #region DFS maze generation

    // Generate maze using DFS, and return the number of end points
    public void BuildMaze() {


        // Store neighbors of given root
        List<Vector2> neighbors;

        // Define starting node as current
        Vector2 current = new Vector2(0, 0);

        // Define prev and curr node for adjacency list
        Vector2 prev = new Vector2(-1, -1);
        Vector2 curr = new Vector2(-1, -1);

        // Push start node to stack
        potentialStack.Push(current);

        while (potentialStack.Count > 0) {
            prev = curr;
            curr = current;

            // Add current to visited list
            visitedList.Add(current);

            // Get all valid neighbors for root
            neighbors = GetValidNeighbors(current);

            if (neighbors.Count > 0) {
                // Add current into stack
                potentialStack.Push(current);

                // Move on to a random of the neighboring nodes
                current = neighbors[rand.Next(neighbors.Count)];
            } else {
                current = potentialStack.Pop();
            }

            // Connect nodes
            if (prev != new Vector2(-1, -1)) {
                ConnectNodes(prev, curr);
            }
        }
    }

    // Given a node, return the list of valid neighbors that are inbound and not been visited
    private List<Vector2> GetValidNeighbors(Vector2 root) {
        List<Vector2> neighbors = new List<Vector2>();

        foreach (Vector2 offset in OFFSET) {
            Vector2 potentialNeighbor = offset + root;

            if (!visitedList.Contains(potentialNeighbor) && IsBounded(potentialNeighbor)) {
                neighbors.Add(potentialNeighbor);
            }
        }

        return neighbors;
    }

    // Given two nodes, connect them on the boolean layout
    private void CreateBooleanPath(Vector2 start, Vector2 end, bool [,] layout) {
        for (int i = (int)start.x; i <= (int)end.x; i++) {
            for (int j = (int)start.y; j <= (int)end.y; j++) {
                layout[i, j] = true;
            }
        }
    }

    // Given two nodes, connect them by adding them to the adjacency list
    private void ConnectNodes(Vector2 prev, Vector2 curr) {
        int prevIndex = GetIndex(prev);

        if (adjacencyList[prevIndex] == null) {
            adjacencyList[prevIndex] = new List<Vector2>() { prev, curr };
        } else {
            adjacencyList[prevIndex].Add(curr);
        }
    }

    // Check if the node is in bound
    private bool IsBounded(Vector2 input) {
        return input.x >= 0 && input.y >= 0 && input.x < width && input.y < height;
    }

    // Given a Vector 2, return the corresponding index number in the array
    private int GetIndex(Vector2 input) {
        return (int)((input.x * height) + input.y);
    }

    // Converts the maze from adjacency list to boolean matirx and expand with the given multiplier
    // Return the result boolean matrix
    public bool[,] InitiateLayout(int multiplier) {
        // Define the size of boolean layout with the given multiplier 
        bool[,] tempBoolLayout = new bool[multiplier * (width - 1) + 1, multiplier * 2 * (height - 1) + 1];

        // Initialize boolean array to be all false
        for (int i = 0; i < multiplier * (width - 1) + 1; i++) {
            for (int j = 0; j < multiplier * (height - 1) + 1; j++) {
                tempBoolLayout[i, j] = false;
            }
        }

        // Convert maze adjacency list to boolean layout
        for (int i = 0; i < width * height; i++) {
            Vector2 root = adjacencyList[i][0];

            foreach (Vector2 neighbor in adjacencyList[i]) {
                CreateBooleanPath(root * multiplier, neighbor * multiplier, tempBoolLayout);
            }
        }

        return tempBoolLayout;
    }

    #endregion

    #region Platform generation

    // Build layout from boolean layout 
    private void BuildLayout() {
        // Get the list of all endpoint 
        for (int i = 0; i < boolLayout.GetLength(0); i++) {
            for (int j = 0; j < boolLayout.GetLength(1); j++) {
                if (boolLayout[i, j]) {
                    Vector2 node = new Vector2(i, j);
                    IsEndPoint(node);
                }
            }
        }

        // Delete endpoint
        foreach (Vector2 endpoint in endpointList) {
            DeleteSurrounding(endpoint);
        }
    }

    // Build platform and store tile information 
    // Creates save file 
    private void BuildPlatform() {
        // Build tile layout
        for (int i = 0; i < boolLayout.GetLength(0); i++) {
            for (int j = 0; j < boolLayout.GetLength(1); j++) {
                if (boolLayout[i, j]) {
                    Instantiate(prefabs[0], new Vector3(10 * i, 0, 10 * j), Quaternion.Euler(new Vector3(-90, 0, 0)), transform);
                }
            }
        }

        // Build platform at each endpoint
        foreach (Vector2 endpoint in endpointList) {
            Instantiate(prefabs[1], new Vector3(10 * endpoint.x, 0, 10 * endpoint.y), Quaternion.Euler(new Vector3(-90, 0, 0)), transform);
        }

        // Build start and end
        Instantiate(prefabs[5], new Vector3(100, 0, -50), Quaternion.Euler(new Vector3(0, 0, 0)), transform);
        Instantiate(prefabs[6], new Vector3(100, 0, 510), Quaternion.Euler(new Vector3(0, 0, 0)), transform);

        // Save data to dungeonData
        DataMain.Current.dungeonData.boolLayout = boolLayout;
        DataMain.Current.dungeonData.AddToEndpointList(endpointList);
    }

    // Given a node, check if it is the end point with only one direction
    // If it is, add it to a list, 
    private void IsEndPoint(Vector2 node) {
        int count = 0;

        foreach (Vector2 offset in OFFSET) {
            Vector2 tempNode = offset + node;

            if (IsOffsetBounded(tempNode) && IsTile(tempNode)) {
                count++;
            }
        }

        if (count == 1) {
            endpointList.Add(node);
        }
    }

    // Given a node, delete surrounding tile for the 4 cardinal direction and itself
    private void DeleteSurrounding(Vector2 node) {
        // Delete self
        boolLayout[(int)node.x, (int)node.y] = false;

        // Delete surrounding
        foreach (Vector2 offset in OFFSET) {
            Vector2 tempNode = offset + node;

            if (IsOffsetBounded(tempNode) && IsTile(tempNode)) {
                boolLayout[(int)tempNode.x, (int)tempNode.y] = false;
            }
        }
    }

    // Given a Vector2, check if the coordinate given is a true on the boolean layout
    private bool IsTile(Vector2 node) {
        return boolLayout[(int)node.x, (int)node.y];
    }

    // Check if the node is in bound
    private bool IsOffsetBounded(Vector2 input) {
        return input.x >= 0 && input.y >= 0 && input.x < boolLayout.GetLength(0) && input.y < boolLayout.GetLength(1);
    }

    #endregion

    #region Generate surroundings

    // Build random kind of surrounding
    public void BuildSurroundingList() {
        for (int i = 0; i < bgBoolLayout.GetLength(0); i++) {
            for (int j = 0; j < bgBoolLayout.GetLength(1); j++) {
                if(!bgBoolLayout[i, j]) {
                    Vector2 node = new Vector2(i, j);
                    
                    if (HasThreeWall(node)) {
                        Vector3 position = new Vector3(((node.x) / 2f) * multiplier * 10, rand.Next(0, 5) * 10, ((node.y) / 2f) * multiplier * 10);
                        bgPositionList.Add(position);
                    }
                }
            }
        }
    }

    // Build surrounding platform
    private void BuildBGPlatform() {
        // Build surrounding at each vertex
        foreach (Vector3 vertex in bgPositionList) {
            GameObject BGobject = Instantiate(prefabs[rand.Next(2, 5)], vertex, Quaternion.Euler(new Vector3(0, 0, 0)), transform);
            BGobject.transform.localScale = new Vector3(.75f, .75f, .75f);
        }

        // Save data to dungeonData
        DataMain.Current.dungeonData.AddToPositionList(bgPositionList);
    }

    // Given a Vector2, check if the coordinate given is true on the boolean layout
    private bool IsBGTile(Vector2 node) {
        return bgBoolLayout[(int)node.x, (int)node.y];
    }

    // Check if the node is in bound
    private bool IsBGBounded(Vector2 input) {
        return input.x >= 0 && input.y >= 0 && input.x < bgBoolLayout.GetLength(0) && input.y < bgBoolLayout.GetLength(1);
    }

    // Given a Vector2, check if the coordinates given is surrounded by 3 wall
    private bool HasThreeWall(Vector2 node) {
        int count = 0;

        foreach(Vector2 offset in OFFSET) {
            Vector2 tempNode = offset + node;

            if (IsBGBounded(tempNode) && IsBGTile(tempNode)) {
                count++;
            }
        }

        return count == 3;
    }

    // Return end point list 
    public List<Vector2> GetEndpointList() {
        return endpointList;
    }

    #endregion
}
