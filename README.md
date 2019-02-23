# CMSC425 Game Programming Final Project
This is the final game programming project for CMSC425. This was meant to be a group project, but I want to see how far I can go if I work alone. Below is the results of 4 weeks of non stop working. There were alot of frustrating times while I was debugging, but seeing my efforst came to live was rewarding. All in all, I had tremendous fun while doing this project. It is not fully completed yet since I only have around 4 weeks to finish this. I plan to go back and work on this in the future.

## Team member
Just me.

## Short description
Tower in Heaven is a dungeon crawler type RPG. It follows the story of a young girl who woke up next to the entrance of an enormous tower, only to find out that she lost her memory. However, she did gain something. She gains the power of controlling light, making her able to use that power to create all sorts of objects, including weapons. The girl decides to climb the tower to see if she can gain her memories back. Follow her as she battles monsters, fight off bosses, and learn more about her power as she advances through the tower.

## More detailed description
### Starting scene
There is a total of 7 scenes in the game file, but only 3 of them are used for the game: Menu, Level_Dungeon, and Level_Town. However, to run the game in Unity, the starting scene must be the Menu scene. This is because the Menu scene is where the character save file is created/loaded, and dungeon creating, player creation, enemy’s creation is base on. More on saving and loading later. 
### Features
I implemented a lot of the features that resemble the real RPG in this game. This includes saving and loading, level progression and stats progression, etc. I will be detailing all the notable and basic features that I implemented below.
#### Saving and loading
Saving a character’s progression is huge in an RPG. This allows the player to come back to where they are left off without restarting, and it is very common in games that do not have an obvious game winning/losing objective, such as an RPG that follows a story. 

The saving and loading are done by using serialization. Serialization is basically turning data into a binary format. This allows security so that player can’t just edit the save file, and yet it is very easy to load in existing save data. The save file location is using the build in data path from Unity: Application.persistentDataPath. This path is different depending on which operating system the game is run on. On Windows machines, for example, it is stored in the AppData folder.

There is a caveat of serialization. It can only serialize basic types such as Boolean, integer, float, doubles, strings. It can also store a collection of these basic types. However, it is not able to save Unity objects such as Vector3 or Quaternion. This poses a problem for me since I need a way to store the player location when saving. After searching, I created my own serializable Vector3 and Quaternion structure using a guide online. With this, I can store player location and rotation.

The save file structure is very simple. I have a Main game data that holds two other datasets: player data and dungeon data. Player data holds player information such as last known location, stats, name, which floor did the player visits, what floor the player is on, etc. Dungeon data holds the structure of the platform that the player is in, as well as the location of the enemies, location of the background, and the number of enemies left. This is important since the dungeon in this game is procedurally generated, and saving is required to not cause an error. Below is a graph structure of what the main game data save file looks like:

<p align="center">
  <img src="https://i.imgur.com/kj5haco.png"/>
</p>
<p align="center">  
  <i>Figure 1. The main game data structure</i>
</p>

As mention above, I created a Serializable Vector3 structure. This is used in saving the player’s current position, as well as the platform endpoint list, platform Boolean layout, and enemy position list. The player game data structure seems simply obvious, I will explain what each of the dungeon data entry does. Platform endpoint list stores where the dungeon hits a dead end. This where enemies will spawn. This list basically holds the location of the enemies. Platform Boolean layout holds the layout of the platform. True being there’s a tile there and false otherwise. Enemy position list holds the position of the enemies. They aren’t necessarily on their spawn point since they could have moved around. And lastly, the background position list holds the background object of the dungeon. This is important so that these objects don’t spawn on top of a path. 
Every time the player creates a new game, any existing save file will be deleted, and a new save file will be created. The player can save whenever they want through the use of a pause menu. I did consider making it so that the player can only save by accessing a physical save point in town/dungeon. But out of consideration of how hard that can be later on, I scratched that idea.

#### Environment 
Both town and dungeon use two different types of environment. The town is more of a 2D platform style, where the player can only move left and right. However, I gave it some depth and the player can move in the z-direction when it’s applicable. The dungeon environment is more of an open world style. The player can move in all 8 direction, and the camera can rotate, zoom, and move along with the player. This type of environment style switching was inspired by a game called Nier: Automata by Yoko Taro. Below is what the town and dungeons look like: 

<p align="center">
  <img src="https://imgur.com/TtQ4clH.png"/>
</p>
<p align="center">  
  <i>Figure 2a. Dungeon</i>
</p>

<p align="center">
  <img src="https://imgur.com/f3Ripyr.png"/>
</p>
<p align="center">  
  <i>Figure 2b. Town</i>
</p>

The dungeon, on the other hand, is procedurally generated. This as done using the good-old breadth-first-search. First, I specify the width to length ratio I want for the maze. I made this maze to be a long rectangle, so the height to width have a 2:1 ratio. I set up a graph matrix using the width and length I specify, and the program will run BFS starting from the center node. This will discover a path that crosses every single cell in the graph matrix. The next step is to expand it. If I would just to spawn prefabs at the current locations now, the maze will look like a giant slab of the ground. So I expand the matrix by a multiplier of 5. This allows a path to show, and also allows me to add more environment assets. Next step was to spawn the platform where enemies will spawn. I calculated where I want the platforms to spawn by spawning them at the end of a path. To ensure that there is at least 7 platforms in each dungeon, I ran BFS on the first step until I have at least 7 endpoints. The last step was to place the entrance and exit assets, spawn the environment prefabs, and add in the enemies. 

#### NavMeshSurface
To allow the player and enemies to move around, both town and dungeon have a NavMeshSurface that bakes it NavMesh. In town scene, since there is only one town and the structure does not change, the NavMesh can be baked ahead of time. 

<p align="center">
  <img src="https://imgur.com/FZzaq8s.png"/>
</p>
<p align="center">  
  <i>Figure 3a. NavMeshSurface in town</i>
</p>

To avoid all the trees, houses and other assets be consider in the NavMeshBaking, I put the object that the player can walk on under an empty GameObject with a NavMeshSurface component added and set the collect object to child only. 

<p align="center">
  <img src="https://imgur.com/Lbsx0Al.png"/>
</p>
<p align="center">  
  <i>Figure 3b. Town NavMeshSurface settings</i>
</p>

<p align="center">
  <img src="https://imgur.com/8SVbNW0.png"/>
</p>
<p align="center">  
  <i>Figure 3c. Town NavMeshSurface without any of the game assets</i>
</p>

A similar process is used for the dungeon. I first build the basic platform, then bake the NavMeshSurface at runtime, then populate the dungeon with other assets. One thing to note here, the stairs are baked differently than any other object. In order to allow the player and enemies to move smoothly up and down the stairs, the stair is comprised of two game objects: one being the actual model of the stairs, and one being a slope with the same degree of incline. The NavMeshAgent will bake the NavMeshSurface before the stairs are populated. This method works for the town, but not in the dungeon. In the dungeon, in order to save time and minimize complexity, the stairs object is spawned as a single game object. However, I put the actual stair model under a different layer other than default and tells the NavMeshAgent to bake all child except that layer. This gives me the result I want. 

<p align="center">
  <img src="https://imgur.com/8SVbNW0.png"/>
</p>
<p align="center">  
  <i>Figure 3c. Town NavMeshSurface without any of the game assets</i>
</p>

#### Camera
Since there are two types of environment style, the camera needs to be different. The camera controller is all under one script, with a boolean that tells the camera if it is an in-town style or in dungeon style. The in-town style camera is fairly simple. It follows the player along the x and y-axis, and the player can zoom by moving the camera along the z-axis. The dungeon camera, however, is more complicated. In order to allow the player to rotate the camera, there was a lot of settings that I had to play with in order to get an optimal camera rotation and translation. In the end, the camera is able to rotate around the player, zooming in and moving a lot of the player. It covers a hemisphere around the player. 

There was also an attempt to allow the player to lock on to an enemy. While locking on, the camera will rotate around the enemy that’s being lock on, and yet the player should still be in view. Moving the player left and right will result in the camera rotating around the enemy. However, I didn’t have time to fully implement this feature. In the end, the camera in the dungeon is like any typical RPG camera, allowing the player to move it to where ever they want. After that, I did three more steps to make the final view more aesthetic.

First, I change the color space from gamma to linear. This gave me a lot of colors to work with. This is done by going to the player settings. Second, I switch the rendering path from User Default to Deferred. This helped tremendously in the final rendering. User Default rendering path renders each object individually, which results in overlapping rendering. However, Deferred rendering path renders all the object at once, which results in a lot of cleaner render. Lastly, I added a post-processing layer to the camera. This allows me to change to add color grading, ambient occlusion and other post-processing that you can normally use while editing photos. 

#### Character movement and combat
The character has a NavMeshAgent component since it moves on top of a NavMeshSurface. Instead of using a mouse click to move the character, the player can use a controller to move in 8 direction. The player can also use dash, which actually shifts the player from walking to sprinting.

For animation, I use two blend trees and other states to animate the player. One blend tree animates the character when it’s not equipped with a weapon, the other one is with a weapon equipped.

<p align="center">
  <img src="https://imgur.com/E0RFB4q.png"/>
</p>
<p align="center">  
  <i>Figure 5. Character animation states</i>
</p>

#### Enemy AI, Movement, and Combat
The enemy also uses a NavMeshAgent to move around. It follows the player around and attacks once the player enters its attack range. While it only does stationary attacks right now, the enemy can attack immediately when the player is in range and will follow the player until it can attack.

<p align="center">
  <img src="https://imgur.com/CSIKv9O.png"/>
</p>
<p align="center">  
  <i>Figure 5. The red is the player detection range. Green is attack range. Blue is the range the player can attack the enemy.</i>
</p>

### Model and resources
For this game, I created a few of the models using Blender. I created the basic building blocks of the town and platforms using Blender. I also created the trees and the fences in town. All the models I made is in the 3D model folder. I also created the UI button. The enemy attack animation is made up of Unity 3d objects and particle effects.

### Known issues and bugs
There is one issue right now. If the player is in the dungeon and decides to go back to the main menu without saving, in theory, the player didn’t save. However, the player will actually spawn in a random location, not on the platform. If the player saves before going to the main menu, there should be no problem. 

### External resources
I used a lot of external resources and tutorial for this project. In particular, the POLYGON Adventure pack makes up a large portion of my environment, and the shader I use for most of my material is from Toony Color Pro 2. The main character is also a Unity asset call Unity-Chan!. I put all the links I used in the source file.

### Video
https://drive.google.com/open?id=16hUM0c9kNnY4WolUCNVSypiTgoU0HC0G
