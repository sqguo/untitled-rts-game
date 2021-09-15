using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    private Grid Map {get;} = null;
    private HotQueue Open {get;set;} = null;
    private bool[,] Visited;
    private int Width {get;} = 0;
    private int Height {get;} = 0;

    private int NumCachedPaths = 0;
    private ushort KeyCounter = 0;
    private Dictionary<int, LinkedList<Cell>> CachedPaths;

    public const int MaxCachedPaths = 10000;
    public const int ErrPathNotFound = -1;
    public const int ErrOutOfSpace = -2;
    public const int ErrPathKeyNotFound = -3;

    public PathFinder(Grid map)
    {
        this.Map = map;
        this.Width = map.Width;
        this.Height = map.Height;
        Visited = new bool[map.Height, map.Width];
        CachedPaths = new Dictionary<int, LinkedList<Cell>>();
    }

    public int CalcPath(Cell start, Cell destination)
    {
        return CalcPath(start, destination, 1);
    }

    public int CalcPath(Cell start, Cell destination, int minClearance)
    {
        // Initialization of open list with starting location
        LinkedList<Cell> path = new LinkedList<Cell>();
        int estimatedNodeWeight = EstDistance(start, destination, 3);
        Open = new HotQueue(2, new List<int>{estimatedNodeWeight});
        Node currentNode = new Node(start.X, start.Y, 0);
        currentNode.CurrentCost = 0;
        Visited[start.Y, start.X] = true;

        while(currentNode != null) {

            // Record the path if destination reached
            if (currentNode.LocatedAt(destination)) {
                Node tmp = currentNode;
                while (tmp != null) {
                    path.AddFirst(new Cell(tmp.X, tmp.Y));
                    tmp = tmp.Parent;
                }
                break;
            }

            // Continue searching
            List<Cell> neighbors = Map.GetAllNeighborsWithClearance(currentNode, minClearance);
            foreach(Cell neighbor in neighbors) {
                if (!Visited[neighbor.Y, neighbor.X]) {
                    Visited[neighbor.Y, neighbor.X] = true;
                    int currentCost = currentNode.CurrentCost+EstNeighborDistance(currentNode, neighbor);
                    int weight = currentCost+EstDistance(neighbor, destination);
                    Node neighborToInsert = new Node(neighbor.X, neighbor.Y, weight, currentNode);
                    neighborToInsert.CurrentCost = currentCost;
                    Open.Insert(neighborToInsert);
                }
            }

            currentNode = Open.Extract();
        }

        // TODO: caching&resume, multiple agents
        Debug.Log("missed "+Open.GetTotalMissCount()+" time");
        Open.Clear();
        Array.Clear(Visited, 0, Width*Height);

        if (path.Count > 0) {
            Debug.Log("pathfinding success");
            return AddPathToCache(path);
        } else {
            Debug.Log("pathfinding failed");
        }

        return ErrPathNotFound;
    }

    public LinkedList<Cell> GetRawPathData(int pathId)
    {
        if (CachedPaths.ContainsKey(pathId)) {
            return CachedPaths[pathId];
        } else {
            return null;
        }
    }

    public int GetPathLength(int pathId)
    {
        if (CachedPaths.ContainsKey(pathId)) {
            return CachedPaths[pathId].Count;
        } else {
            return ErrPathKeyNotFound;
        }
    }

    private int EstDistance(Cell start, Cell end, double diagonalCost)
    {
        int diffX = Math.Abs(start.X - end.X);
        int diffY = Math.Abs(start.Y - end.Y);
        int diagD = (int) Math.Floor(diagonalCost*Math.Min(diffX, diffY));
        return diagD+2*Math.Abs(diffX-diffY);
    }

    private int EstDistance(Cell start, Cell end)
    {
        return EstDistance(start, end, 2.8);
    }

    private int EstNeighborDistance(Cell start, Cell end)
    {
        if (start.X == end.X || start.Y == end.Y) {
            return 2;
        } else {
            return 3;
        }
    }

    private int AddPathToCache(LinkedList<Cell> path)
    {
        if (NumCachedPaths >= MaxCachedPaths) {
            return ErrOutOfSpace;
        }
        NumCachedPaths++;

        while(true) {
            try {
                KeyCounter++;
                CachedPaths.Add(KeyCounter, path);
                return KeyCounter;
            } catch (ArgumentException) {
                KeyCounter++;
            }
        }
    }

    public void RefreshPath(Cell current, int pathId)
    {

    }

    public void GetNextWayPoint(int pathId)
    {

    }

    public void CompletePath(int pathId)
    {

    }

#if UNITY_EDITOR
    public void DebugPath(int pathId)
    {
        if (CachedPaths.ContainsKey(pathId)) {
            string line = "";
            foreach(Cell node in CachedPaths[pathId]) {
                line+="(";
                line+=node.X;
                line+=",";
                line+=node.Y;
                line+=")\t";
            }
            Debug.Log(line);
        } else {
            Debug.Log("Path ID not found");
        }
    }
#endif
}