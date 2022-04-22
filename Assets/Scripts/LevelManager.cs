using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] public Transform map;
    [SerializeField] private GameObject[] tilePrefabs;
    [SerializeField] private GameObject spawnerPrefab;
    [SerializeField] private GameObject despawnerPrefab;

    public Level level;
    public Spawner Spawner { get; set; }
    public Spawner Despawner { get; set; }

    private Point bounds;

    public Stack<Vector2> path;

    public Stack<Vector2> Path
    {
        get
        {
            return new Stack<Vector2>(new Stack<Vector2>(path));
        }
    }

    public Dictionary<Point, TileScript> Tiles { get; set; }

    public float TileSize
    {
        get { return tilePrefabs[2].GetComponent<SpriteRenderer>().sprite.bounds.size.x; }
    }

    void Start()
    {
        CreateLevel();
    }

    private void CreateLevel()
    {
        this.level = Level.ReadFromFile(GameManager.Instance.level);

        Tiles = new Dictionary<Point, TileScript>();
        string[] map = this.level.levelMap;
        
        Vector3 worldStart = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height));
        for (int y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[0].Length; x++)
            {
                PlaceTile(map[y].ToCharArray()[x].ToString(), x, y, worldStart);
                if (map[y].ToCharArray()[x].ToString() == "0")
                    Tiles[new Point(x, y)].IsEmpty = false;
            }
        }
        this.bounds = new Point(map[0].Length - 1, map.Length - 1);

        CreateSpawners(this.level.spawnerPoint, this.level.despawnerPoint);
        GetStaticPath();
    }

    private void PlaceTile(string tileType, int x, int y, Vector3 worldStart)
    {
        int tileIndex = int.Parse(tileType);
        TileScript newTile = Instantiate(tilePrefabs[tileIndex]).GetComponent<TileScript>();
        newTile.Setup(new Point(x, y), new Vector3(worldStart.x + TileSize * x, worldStart.y - TileSize * y, 0), map);
    }

    private void CreateSpawners(Point spawnerPoint, Point despawnerPoint)
    {
        Spawner = Instantiate(spawnerPrefab, Tiles[spawnerPoint].GetComponent<TileScript>().WorldPosition,
            Quaternion.identity).GetComponent<Spawner>();
        Spawner.name = "Spawner";
        Spawner.GridPosition = spawnerPoint;

        Despawner = Instantiate(despawnerPrefab, Tiles[despawnerPoint].GetComponent<TileScript>().WorldPosition,
            Quaternion.identity).GetComponent<Spawner>();
        Despawner.name = "Despawner";
        Despawner.GridPosition = despawnerPoint;
    }

    public bool OutOfBounds(Point p)
    {
        if (p.X > bounds.X || p.X < 0 || p.Y > bounds.Y || p.Y < 0)
            return true;
        return false;
    }

    private void GetStaticPath()
    {
        Stack<Vector2> staticPath = new Stack<Vector2>();
        Point currPoint = Spawner.GridPosition;
        
        List<Point> walked = new List<Point>();
        while (currPoint.X != Despawner.GridPosition.X || currPoint.Y != Despawner.GridPosition.Y)
        {
            walked.Add(currPoint);
            if (!OutOfBounds(new Point(currPoint.X - 1, currPoint.Y)) && this.level.levelMap[currPoint.Y][currPoint.X - 1] == '0' && !walked.Contains(new Point(currPoint.X - 1, currPoint.Y)))
            {
                currPoint = new Point(currPoint.X - 1, currPoint.Y);
                staticPath.Push(new Vector2(Tiles[currPoint].WorldPosition.x, Tiles[currPoint].WorldPosition.y));
            }
           
            else if (!OutOfBounds(new Point(currPoint.X + 1, currPoint.Y)) && this.level.levelMap[currPoint.Y][currPoint.X + 1] == '0' && !walked.Contains(new Point(currPoint.X + 1, currPoint.Y)))
            {
                currPoint = new Point(currPoint.X + 1, currPoint.Y);
                staticPath.Push(new Vector2(Tiles[currPoint].WorldPosition.x, Tiles[currPoint].WorldPosition.y));
            }
           
            else if (!OutOfBounds(new Point(currPoint.X, currPoint.Y - 1)) && this.level.levelMap[currPoint.Y - 1][currPoint.X] == '0' && !walked.Contains(new Point(currPoint.X, currPoint.Y - 1)))
            {
                currPoint = new Point(currPoint.X, currPoint.Y - 1);
                staticPath.Push(new Vector2(Tiles[currPoint].WorldPosition.x, Tiles[currPoint].WorldPosition.y));
            }
            else if (!OutOfBounds(new Point(currPoint.X, currPoint.Y + 1)) && this.level.levelMap[currPoint.Y + 1][currPoint.X] == '0' && !walked.Contains(new Point(currPoint.X, currPoint.Y + 1)))
            {
                currPoint = new Point(currPoint.X, currPoint.Y + 1);
                staticPath.Push(new Vector2(Tiles[currPoint].WorldPosition.x, Tiles[currPoint].WorldPosition.y));
            }
        }

        staticPath = new Stack<Vector2>(staticPath);
        this.path = staticPath;
    }
}
