using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Demo : MonoBehaviour
{

    public Sprite square;
    public Tilemap tilemap;
    public int maxClearance;
    public BoundsInt bounds;
    public GameObject agent;

    private int width = 0;
    private int height = 0;
    private Grid map = null;
    private PathFinder pathfinder = null;
    private Tile walkableTile;
    private Tile obstacleTile;
    private Tile pathTile;
    private Tile startingTile;
    private Tile destinationTile;

    private Cell startingLocation = null;
    private Cell destinationLocation = null;
    private bool pathRefreshedRequired = false;
    private int pathClearance = 1;
    private int computedPathId;

    private readonly KeyCode[] numericKeys = {
        KeyCode.Alpha0,
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9
    };

    void Start()
    {
        width = bounds.xMax - bounds.xMin;
        height = bounds.yMax - bounds.yMin;

        map = new Grid(width, height, maxClearance);
        List<Cell> defaultObstacles = new List<Cell>();
        for(int i = 0; i < width; i++) {
            defaultObstacles.Add(new Cell(i, 0));
            defaultObstacles.Add(new Cell(i, height-1));
        }
        for (int i = 1; i < height-1; i++) {
            defaultObstacles.Add(new Cell(0, i));
            defaultObstacles.Add(new Cell(width-1, i));
        }
        map.SetObstacle(defaultObstacles);

        walkableTile = ScriptableObject.CreateInstance<Tile>();
        walkableTile.sprite = square;
        walkableTile.color = Color.white;

        obstacleTile = ScriptableObject.CreateInstance<Tile>();
        obstacleTile.sprite = square;
        obstacleTile.color = Color.black;

        pathTile = ScriptableObject.CreateInstance<Tile>();
        pathTile.sprite = square;
        pathTile.color = Color.cyan;

        startingTile = ScriptableObject.CreateInstance<Tile>();
        pathTile.sprite = square;
        pathTile.color = Color.blue;

        destinationTile = ScriptableObject.CreateInstance<Tile>();
        pathTile.sprite = square;
        pathTile.color = Color.green;

        pathfinder = new PathFinder(map);
        refreshTileMap();
    }

    void Update()
    {
        if (pathRefreshedRequired) {
            pathRefreshedRequired = false;
            if (startingLocation != null && destinationLocation != null) {
                int pathId = pathfinder.CalcPath(startingLocation, destinationLocation, pathClearance);
                if (pathId > 0) {
                    computedPathId = pathId;
                    int pathLength = pathfinder.GetPathLength(pathId);
                    Vector3Int[] pathTilesLocs = new Vector3Int[pathLength];
                    Tile[] pathTiles = new Tile[pathLength];
                    int counter = 0;
                    foreach(Cell loc in pathfinder.GetRawPathData(pathId)) {
                        pathTilesLocs[counter] = CellToTileMapVector(loc);
                        pathTiles[counter] = pathTile;
                        counter++;
                    }
                    refreshTileMap();
                    tilemap.SetTiles(pathTilesLocs, pathTiles);
                }
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            Vector3Int cellPos = getFocusedCellPos();
            Cell location = TileMapVectorToCell(cellPos);

            if (map.IsWalkable(location)) {
                map.SetObstacle(new List<Cell>{location});
                tilemap.SetTile(cellPos, obstacleTile);
            }
        }

        if (Input.GetMouseButtonUp(1)) {
            Vector3Int cellPos = getFocusedCellPos();
            Cell location = TileMapVectorToCell(cellPos);

            if (map.IsObstacle(location)) {
                map.RemoveObstacle(new List<Cell>{location});
                tilemap.SetTile(cellPos, walkableTile);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)){
            map.DebugGrid();
        }
        
        for (int i = 1; i < numericKeys.Length; i++) {
            if (Input.GetKeyDown(numericKeys[i])){
                Vector3Int cellPos = getFocusedCellPos();
                Cell location = TileMapVectorToCell(cellPos);
                if (map.IsWalkable(location, i) && !location.Equals(startingLocation)) {
                    pathClearance = i;
                    startingLocation = location;
                    tilemap.SetTile(cellPos, startingTile);
                    pathRefreshedRequired = true;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.D)) {
            Vector3Int cellPos = getFocusedCellPos();
            Cell location = TileMapVectorToCell(cellPos);
            if (map.IsWalkable(location) && !location.Equals(destinationLocation)) {
                destinationLocation = location;
                tilemap.SetTile(cellPos, destinationTile);
                pathRefreshedRequired = true;
            }
        }
    }

    private Vector3Int getFocusedCellPos()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return tilemap.WorldToCell(pos);
    }

    private Cell TileMapVectorToCell(Vector3Int cellPos)
    {
        int gridX = cellPos.x - bounds.xMin;
        int gridY = cellPos.y - bounds.yMin;
        // Debug.Log("Cell selected: ("+gridX+", "+gridY+")");
        return new Cell(gridX, gridY);
    }

    private Vector3Int CellToTileMapVector(Cell loc)
    {
        int tileX = loc.X + bounds.xMin;
        int tileY = loc.Y + bounds.yMin;
        return new Vector3Int(tileX, tileY, 0);
    }

    private void refreshTileMap()
    {
        Tile[] tiles = new Tile[width*height];
        for (int i = 0; i < height; i++) {
            for (int j = 0; j < width; j++) {
                if (map.IsObstacle(new Cell(j, i))) {
                    tiles[i*width+j] = obstacleTile;
                } else {
                    tiles[i*width+j] = walkableTile;
                }
            }
        }
        
        tilemap.SetTilesBlock(bounds, tiles);
    }
}
