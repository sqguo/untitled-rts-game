# untitled RTS game
Currently at prototyping phase to test agent pathfinding and collision avoidance algorithms.

### Obstacle Creation and Removal:
Grid and some obstacles (border) are dynamically generated at runtime.<br />
User can create additional obstacles anywhere on the grid by left clicking on the grid with the mouse. <br />
Obstacles can be removed by right clicking on the obstacle block. <br />

### Setting Starting Point and Destination
A starting point can be set by hovering the mouse over a position and hitting the numerical key 1 to 9. Each key indicates the minimal distance between the agent and the obstacle. For example, a value of 1 allows the agent to move right next to the block. A destination point can be set by hovering the mouse over a position and hitting the key 'd'. Once both starting and destination points are set, the path will be highlighted in green. 

The below screenshot illustrates an agent with a minimal clearance requirement of 3. (Must be at least 2 blocks away from any obstacle)

![image](https://user-images.githubusercontent.com/59301688/134130356-ecf57d81-92da-4d88-85b3-c8e7622664c5.png)


### Notes
Increase main camera Size if the grid is clipped. 
The size of the grid should be set on the bounds variable of the "DEMO" empty game object through the editor (set X and Y)
Current settings
```
X: 40
Y: 40
Z: 1 (do not change Z)
```



### References
http://theory.stanford.edu/~amitp/GameProgramming/ImplementationNotes.html  <br />
https://archive.ph/uk09E  <br />
