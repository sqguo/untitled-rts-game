using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public int Height {get;} = 0;
    public int Width {get;} = 0;
    public int DefaultClearance {get;} = 0;
    private int[,] ClearanceMap;

    public Grid(int height, int width, int defaultClearance)
    {
        this.Height = height;
        this.Width = width;
        this.DefaultClearance = defaultClearance;
        ClearanceMap = new int[height, width];
        for (int i = 0; i <= ClearanceMap.GetUpperBound(0); i++) {
            for (int j = 0; j <= ClearanceMap.GetUpperBound(1); j++) {
                ClearanceMap[i, j] = defaultClearance;
            }
        }
    }

    // A list of neighboring locations
    public List<Cell> GetAllNeighbors(Cell location)
    {
        List<Cell> neighbors = new List<Cell> {
            new Cell(location.X, location.Y+1),
            new Cell(location.X, location.Y-1),
            new Cell(location.X+1, location.Y),
            new Cell(location.X-1, location.Y),
            new Cell(location.X+1, location.Y+1),
            new Cell(location.X+1, location.Y-1),
            new Cell(location.X-1, location.Y+1),
            new Cell(location.X-1, location.Y-1)
        };
        return neighbors.FindAll(c => IsLocationOnGrid(c));
    }

    public List<Cell> GetAllNeighborsWithClearance(Cell location, int minClearance)
    {
        List<Cell> neighbors = new List<Cell> {
            new Cell(location.X, location.Y+1),
            new Cell(location.X, location.Y-1),
            new Cell(location.X+1, location.Y),
            new Cell(location.X-1, location.Y),
            new Cell(location.X+1, location.Y+1),
            new Cell(location.X+1, location.Y-1),
            new Cell(location.X-1, location.Y+1),
            new Cell(location.X-1, location.Y-1)
        };
        return neighbors.FindAll(c => IsLocationOnGrid(c) && ClearanceMap[c.Y, c.X] >= minClearance);
    }

    public bool IsLocationOnGrid(Cell location)
    {
        if (location.X >= 0 && location.Y >= 0 && location.X < Width && location.Y < Height) {
            return true;
        }
        return false;
    }

    public void SetObstacle(List<Cell> locations)
    {
        Queue<Cell> waitlist = new Queue<Cell>();
        foreach (Cell loc in locations) {
            if (IsWalkable(loc)) {
                ClearanceMap[loc.Y, loc.X] = 0;
                waitlist.Enqueue(loc);
            }
        }
        while(waitlist.Count > 0) {
            Cell loc = waitlist.Dequeue();
            int locClearance = ClearanceMap[loc.Y, loc.X];

            // Neighbor of any neighbor with a correct clearance value 
            // will have been updated with the correct value or added to the queue already
            List<Cell> neighbors = GetAllNeighborsWithClearance(loc, locClearance+2);
            foreach (Cell neighbor in neighbors) {
                ClearanceMap[neighbor.Y, neighbor.X] = locClearance+1;
                waitlist.Enqueue(neighbor);
            }
        }
    }

    public void RemoveObstacle(List<Cell> locations)
    {
        RemoveObstacleSlow(locations.FindAll(loc => IsObstacle(loc)));
    }


    private void RemoveObstacleSlow(List<Cell> locations)
    {
        Queue<Cell> waitlist = new Queue<Cell>(
            locations.FindAll(loc => IsLocationOnGrid(loc))
        );
        List<Cell> locationsToLift = new List<Cell>();
        int progressCounter = 0;

        // Update neighboring cells with ripple effect
        while(waitlist.Count > 0) {
            progressCounter++;
            Cell loc = waitlist.Dequeue();
            List<Cell> neighbors = GetAllNeighbors(loc);
            int locClearance = ClearanceMap[loc.Y, loc.X];
            
            // If all neighbors has equal or higher clearance
            // then the current location can be increased
            bool doIncrementClearance = true;
            if (locClearance < DefaultClearance) {
                foreach(Cell neighbor in neighbors) {
                    if (ClearanceMap[neighbor.Y, neighbor.X] < locClearance) {
                        doIncrementClearance = false; 
                        break;
                    }
                }
            } else {
                doIncrementClearance = false;
            }

            // Add all neighbors that needs to be updated to queue
            // Any neighbor equal to locClearance will not be enqueued
            // as they must have a neightbor with lower clearance
            if (doIncrementClearance) {
                ClearanceMap[loc.Y, loc.X]++;
                foreach(Cell neighbor in neighbors) {
                    if (ClearanceMap[neighbor.Y, neighbor.X] == locClearance+1) {
                        waitlist.Enqueue(neighbor);
                    }
                }

                // TODO: refactor this
                if (progressCounter <= locations.Count) {
                    locationsToLift.Add(loc);
                }
            }
        }

        // Continue lifting the clearance of the obstacles
        // Until Max clearance is reached or one of the neightbors
        // have lower clearance
        if (locationsToLift.Count > 0) {
            RemoveObstacleSlow(locationsToLift);
        }
    }

    public bool IsObstacle(Cell location)
    {
        return IsLocationOnGrid(location) && (ClearanceMap[location.Y, location.X] == 0);
    }

    public bool IsWalkable(Cell location, int clearance)
    {
        return IsLocationOnGrid(location) && (ClearanceMap[location.Y, location.X] >= clearance);
    }

    public bool IsWalkable(Cell location)
    {
        return IsWalkable(location, 1);
    }

#if UNITY_EDITOR
    public void DebugGrid()
    {
        string line = "";
        for (int i = ClearanceMap.GetUpperBound(0); i >= 0; i--) {
            for (int j = 0; j <= ClearanceMap.GetUpperBound(1); j++) {
                line += ClearanceMap[i, j].ToString();
                line += "\t";
            }
            line += "\n";
        }
        Debug.Log(line);
    }
#endif
}

