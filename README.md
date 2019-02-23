# CMSC425 Game Programming Final Project
This is the final game programming project for CMSC425. It is not fully completed yet since I only have around 4 weeks to finish this. I plan to go back and work on this in the future.

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

![alt text](https://i.imgur.com/kj5haco.png)
