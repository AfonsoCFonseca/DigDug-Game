# Bejeweled
Classic DigDug game developed in a Unity 2D project



---------------------------------------------------------------
# Structure


### LevelManager ###
Responsible for drawing the map based on a 2D array of 14 by 14 integers passed by the LevelMap script, it iterates through every position of the array and sets the slots for each tile.

### PlayerController ###
The character moves along the X and Y axes by applying speed and Time.deltaTime to the transform.position for each arrow key stroke. Additionally, during movement, an animation is applied to the player sprite based on an Image Spritesheet. The sprite is flipped, and the Quaternion is rotated depending on whether the movement is up/down or left/right.

### Tile/Slot ###
A tile represents one-fourteenth of the X-axis and one-fourteenth of the Y-axis of the map, through which the player will move. The tile is determined by the integer mentioned in the desired map array for the tile's position. For example, the number 0 represents an undug tile, while numbers 1 or 2 represent a dug tile in the horizontal or vertical position.

Additionally, a tile is composed of 8 slots (4 horizontal and 4 vertical), representing a more detailed representation of how the tile has been dug.


---------------------------------------------------------------
# Development
Before officially starting to develop the game, I tried to create the map by dividing it into 14 by 14 chunks and applying a 2D array to represent the status of each chunk (filled or dug). Once I understood that I could overcome this part, I officially started the development of the game.

I applied the background map and the dug images to the dug chunks/tiles and made a few new additions to the array. For example, I created a tile for intersecting the horizontal and vertical tunnels.

Afterward, I implemented the first player movement by enabling movement along the X and Y axes without restrictions. I also added sprite animation that flips or rotates the sprite depending on the direction of the player's movement. 
When calculating the movement, I developed the system that reaches the next tile based on the direction you are heading and updates the player transform.position to that location



# Future Implementation


---------------------------------------------------------------
# Sketches & Evolution


