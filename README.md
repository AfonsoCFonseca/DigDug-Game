# Dig Dug
Classic DigDug game developed in a Unity 2D project

<p align="center">
  <img src='https://github.com/AfonsoCFonseca/DigDug-Game/blob/main/progress_photos/header.png'>
</p>

---------------------------------------------------------------
# Structure


### LevelManager ###
Responsible for drawing the map based on a 2D array of 14 by 14 integers passed by the LevelMap script, it iterates through every position of the array and sets the slots for each tile. Also responsible for returning all the values related to the map and tiles. Handles the spawn of the different enemies in the current map

### PlayerController ###
The character moves along the X and Y axes by applying speed and Time.deltaTime to the transform.position for each arrow key stroke. Additionally, during movement, an animation is applied to the player sprite based on an Image Spritesheet. The sprite is flipped, and the Quaternion is rotated depending on whether the movement is up/down or left/right.

<div align="center">
  <img width="967" height="160" src='https://github.com/AfonsoCFonseca/DigDug-Game/blob/main/progress_photos/player_new_spritesheet1.png'>
</div>

<div align="center"">
  <p>i've remastered the Player Spritesheet</p>
</div>

### Tile/Slot ###
A tile represents one-fourteenth of the X-axis and one-fourteenth of the Y-axis of the map, through which the player will move. The tile is determined by the integer mentioned in the desired map array for the tile's position. For example, the number 0 represents an undug tile, while numbers 1 or 2 represent a dug tile in the horizontal or vertical position.

Additionally, a tile is composed of 8 slots (4 horizontal and 4 vertical), representing a more detailed representation of how the tile has been dug.

### Enemy ###
A general class is responsible for drawing the enemies and their animations. The class has three states: moving, ghost, and dying. In the moving state, it checks for available tiles, choosing between intersections, and follows the player if the tiles are connected. In Chase mode, it can move through filled tiles and moves towards the last tile where the player passed. The dying mode occurs when the player kills the enemy, triggering the animation of the dying frames and deleting the class at the end.


<div align="center">
  <img width="210" height="210" src='https://github.com/AfonsoCFonseca/DigDug-Game/blob/main/progress_photos/old_enemy_explode1.gif'>
  <img width="210" height="210" src='https://github.com/AfonsoCFonseca/DigDug-Game/blob/main/progress_photos/new_enemy_explode1.gif'>
</div>

<div align="center">
  <p>also created a new spritesheet for the enemy explosion</p>
</div>

---------------------------------------------------------------
# Development
Before officially starting to develop the game, I tried to create the map by dividing it into 14 by 14 chunks and applying a 2D array to represent the status of each chunk (filled or dug). Once I understood that I could overcome this part, I officially started the development of the game.

I applied the background map and the dug images to the dug chunks/tiles and made a few new additions to the array. For example, I created a tile for intersecting the horizontal and vertical tunnels.

Afterward, I implemented the first player movement by enabling movement along the X and Y axes without restrictions. I also added sprite animation that flips or rotates the sprite depending on the direction of the player's movement. 
When calculating the movement, I developed the system that reaches the next tile based on the direction you are heading and updates the player transform.position to that location. The player must stick to this position until he reaches it, regardless the key pressed

<div align="center">
   <img width="200" height="250" src='https://github.com/AfonsoCFonseca/DigDug-Game/blob/main/progress_photos/draft_1.png'>
   <img width="200" height="250" src='https://github.com/AfonsoCFonseca/DigDug-Game/blob/main/progress_photos/_1.png'>
</div>

Next, I shifted the building proccess of the game and changed to the UI implementation system. Added the life system and UI, implemented the highscore and score system and added the rounds UI and functions (levels)

After starting to implement the digging logic, I created a 2D box collider in my player GameObject that detects trigger collisions with any of the tile Slots. When a collision occurs, it activates the rendering of the slots and deactivates their colliders. To simplify collision detection based on direction (up/down or right/left), I began tracking the player's orientation using booleans. Once the movement and excavation were established, I applied general limitations to player movement, such as restricting movement beyond the board's limits and allowing backward movement. In this game, backward movement is the only direction allowed, even if the next neighbor Tile position is already set.

I started working on the Enemy class, initially focusing on applying animations to the enemy and its spawning on the map. After that, I concentrated on creating three state enums: Move, Chase, and Dead for the class and worked on the Move state. The Move state involves moving in one of the four directions, choosing between directions when arriving at an intersection, and turning back when reaching a dead end. Once this was achieved, I added the new enemy to the game and applied the logic for map spawning. After that, I began focusing on implementing the logic for the enemy to follow the player once the tiles are connected between them.

<div align="center">
   <img width="200" height="250" src='https://github.com/AfonsoCFonseca/DigDug-Game/blob/main/progress_photos/_2.png'>
   <img width="200" height="250" src='https://github.com/AfonsoCFonseca/DigDug-Game/blob/main/progress_photos/draft_2.png'>
</div>

Before initiating the Chase Mode for the enemies, I first had to refine the digging system and construct the initial map tunnels. I uploaded a new image for the slot, representing a round-ended tunnel, and added it as a sprite to the Slot class. During the initial map construction, each tile checks its orientation and the adjacent tile. If the tile is filled with dirt, the last or first slot (depending on the orientation) of the tile changes to the end rounded slot, providing a more natural feeling to the tunnel.

The same approach was implemented during gameplay when the player begins to dig and changes direction. At this point, the code became challenging to comprehend, prompting me to perform some refactoring and address a few bugs.

<div align="center">
   <img width="200" height="250" src='https://github.com/AfonsoCFonseca/DigDug-Game/blob/main/progress_photos/draft_3.png'>
   <img width="200" height="250" src='https://github.com/AfonsoCFonseca/DigDug-Game/blob/main/progress_photos/_4.png'>
</div>

I spent approximately four weekends developing the Enemy pathfinder from scratch. I couldn't remember how I had implemented it for the Pacman game (https://github.com/AfonsoCFonseca/Pacman-Game), but despite having a potential solution in that repository, I decided not to reference it or implement a pathfinding algorithm like A*. Instead, I tried to come up with my own idea.

Before moving, the Enemy checks every free neighboring tile around it and chooses one direction. From there, it checks the next possible free tile on that current neighboring tile and randomly selects one, setting a direction. This process continues until one of three things happens: it reaches a dead end, finds the player, or neither of the previous options occur.

If a dead end is reached, the validation returns to the current enemy position and restarts the pathfinding process(I could have implemented a blacklist for the already investigated paths to improve the algorithm's performance, but I was already tired of this development). If the player is found, the pathfinding process immediately stops and returns the first position of the array that has the path to the player to become the next tile to move to. If neither of these options is met, it will randomly pick one of the available directions to move next.

<div align="center">
  <img width="400" height="250" src='https://github.com/AfonsoCFonseca/DigDug-Game/blob/main/progress_photos/sprite_remaster.gif'>
</div>

<div align="center"">
  <p>Side by side old/mine spritesheet comparison </p>
</div>

In between developments, I shifted focus to create some new sprite sheets for the player and enemy. I completed the sprite sheets but decided against applying them to the game because they didn't match the current art style. I've added them to the repository under /progress_photos and shared some previews in this readme file.

After completing the AI enemy map exploration, I initiated the player attack sequence. This began with triggering the attack, moving the rope forward, and adding a trigger to its endpoint. Then, I established the collision between the enemy and the rope, implementing logic for the enemy's inflation state. To initiate enemy inflation, the player must press the attack button four times within a short timeframe. If this condition is not met, the enemy resets to its initial state and resumes wandering. This functionality was implemented using timers. Additionally, I incorporated an exception for when the player moves during inflation, cancelling the attack and preventing player movement while the rope is still in motion but has not yet reached its endpoint or encountered an enemy.

Next, I implemented a Fire attack for the Fygar Enemy. Every 5 to 21 seconds, the enemy will spit fire. To achieve this, I had to create several timers to manage the activation, deactivation, and sprite switching during the fire attack phase. Additionally, I updated the enemy sprite to include a new fire animation phase and positioned the fire sprites according to the enemy's rotation.
To complete the implementation, I added colliders to each sprite and set them to trigger upon player collision, resulting in a game restart (resulting in the loss of one life). Furthermore, I introduced a slight delay after the player is killed to allow the player to perceive the avatar's collision with the enemy

<div align="center">
  <img width="300" height="375" src='https://github.com/AfonsoCFonseca/DigDug-Game/blob/main/progress_photos/_6_.gif'>
</div>

A chase state for the enemy was implemented. This phase represents a 'ghost' phase where enemies can get close to the player by passing through walls and reaching the player's current position. This phase occurs randomly when the enemy is in a moving state. I have a simple timer that checks the player's current tile and transitions the enemy there, while switching to a different enemy sprite animation (ghost type). When the enemy reaches the player's location, it resets the variables for chasing and sets the enemy back to the moving phase.

Map management and generation were the final steps to wrap up the game. I refactored the Restart method to avoid completely resetting the map, allowing the map path to remain intact when a life is lost. Then, I implemented the logic for progressing to the next level when the player defeats all the enemies on the map. However, to achieve this, I needed a unique map for each level. I decided to generate the map randomly with small increments in difficulty, where enemies become progressively faster. Additionally, for every even-numbered level, I added a new enemy to the game. The generation process itself was based on the number of enemies, with each enemy requiring a 1x3 hole to be created either horizontally or vertically (randomly). The positions of these holes were only created if they differed from the player's initial position and remained within the map dimensions. I achieved this by utilizing a do-while loop and applying these conditions within it. Once the map was successfully generated, I randomly placed an enemy in the middle of each 1x3 hole.

# Future Implementation


---------------------------------------------------------------
# Sketches & Evolution
