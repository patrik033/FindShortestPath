using System;
using System.Collections.Generic;
using System.Drawing;
using Spectre.Console;
using Spectre.Console.Rendering;

static class Program
{
    static Random random = new Random();
    static int[,] grid = new int[10, 10]; // Grid representing the map

    static void Main(string[] args)
    {
        InitializeGrid();
        Point start = new Point(0, 0);
        Point exit = FindExitPoint();
        List<Point> shortestPath = FindShortestPath(start, exit);
        List<List<Point>> allPaths = GetAllPaths(start, exit);
        PrintGrid(shortestPath, allPaths);
    }

    // Initialize the grid with walls, start point, and exit point
    static void InitializeGrid()
    {
        PlaceRandomWalls();
        grid[0, 0] = (int)Cell.Start;
        PlaceExitPoint();
    }

    // Place random walls on the grid
    static void PlaceRandomWalls()
    {
        int numWalls = random.Next(6, 11);
        for (int i = 0; i < numWalls; i++)
        {
            PlaceRandomWall();
        }
    }

    // Place a random wall on the grid
    static void PlaceRandomWall()
    {
        int x = random.Next(0, grid.GetLength(0));
        int y = random.Next(0, grid.GetLength(1));

        while (grid[x, y] != 0)
        {
            x = random.Next(0, grid.GetLength(0));
            y = random.Next(0, grid.GetLength(1));
        }

        grid[x, y] = (int)Cell.Wall;
    }

    // Place the exit point on the grid
    static void PlaceExitPoint()
    {
        int x = random.Next(0, grid.GetLength(0));
        int y = random.Next(0, grid.GetLength(1));

        while (grid[x, y] != 0)
        {
            x = random.Next(0, grid.GetLength(0));
            y = random.Next(0, grid.GetLength(1));
        }

        grid[x, y] = (int)Cell.Exit;
    }

    // Find the exit point on the grid
    static Point FindExitPoint()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] == (int)Cell.Exit)
                {
                    return new Point(i, j);
                }
            }
        }
        return null;
    }

    // Find all possible paths from start to end using BFS
    static List<List<Point>> GetAllPaths(Point start, Point end)
    {
        Dictionary<Point, List<List<Point>>> paths = new Dictionary<Point, List<List<Point>>>();
        paths[start] = new List<List<Point>> { new List<Point> { start } };
        Queue<Point> queue = new Queue<Point>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            Point current = queue.Dequeue();
            List<Point> currentPath = paths[current][0];

            foreach (var neighbor in GetNeighbors(current))
            {
                if (grid[neighbor.X, neighbor.Y] == (int)Cell.Wall) // Skip walls
                    continue;

                if (!paths.ContainsKey(neighbor))
                {
                    paths[neighbor] = new List<List<Point>>();

                    foreach (var path in paths[current])
                    {
                        var newPath = new List<Point>(path);
                        newPath.Add(neighbor);
                        paths[neighbor].Add(newPath);
                    }

                    queue.Enqueue(neighbor);
                }
                else
                {
                    foreach (var path in paths[current])
                    {
                        var newPath = new List<Point>(path);
                        newPath.Add(neighbor);
                        paths[neighbor].Add(newPath);
                    }
                }
            }
        }

        return paths[end];
    }

    // Find the shortest path from start to end using Dijkstra's algorithm
    static List<Point> FindShortestPath(Point start, Point end)
    {
        Dictionary<Point, int> distance = new Dictionary<Point, int>();
        Dictionary<Point, Point> previous = new Dictionary<Point, Point>();
        HashSet<Point> visited = new HashSet<Point>();
        PriorityQueue<Point> queue = new PriorityQueue<Point>();

        foreach (var point in GetAllPoints())
        {
            distance[point] = int.MaxValue;
            previous[point] = null;
        }

        distance[start] = 0;
        queue.Enqueue(start, 0);

        while (queue.Count > 0)
        {
            Point current = queue.Dequeue();

            if (current.Equals(end))
                break;

            visited.Add(current);

            foreach (var neighbor in GetNeighbors(current))
            {
                if (visited.Contains(neighbor) || grid[neighbor.X, neighbor.Y] == (int)Cell.Wall)
                    continue;

                int alt = distance[current] + 1; // Assuming uniform edge weight

                if (alt < distance[neighbor])
                {
                    distance[neighbor] = alt;
                    previous[neighbor] = current;
                    queue.Enqueue(neighbor, alt);
                }
            }
        }

        List<Point> shortestPath = new List<Point>();
        Point trace = end;

        while (trace != null)
        {
            shortestPath.Add(trace);
            trace = previous[trace];
        }

        shortestPath.Reverse();
        return shortestPath;
    }

    // Get the neighbors of a point on the grid
    static List<Point> GetNeighbors(Point p)
    {
        List<Point> neighbors = new List<Point>();

        Point[] directions = { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) }; // Right, Left, Down, Up

        foreach (var dir in directions)
        {
            int newX = p.X + dir.X;
            int newY = p.Y + dir.Y;

            if (newX >= 0 && newX < grid.GetLength(0) && newY >= 0 && newY < grid.GetLength(1))
            {
                neighbors.Add(new Point(newX, newY));
            }
        }

        return neighbors;
    }

    // Get all points on the grid
    static List<Point> GetAllPoints()
    {
        List<Point> points = new List<Point>();

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                points.Add(new Point(i, j));
            }
        }
        return points;
    }

    // Print the grid with paths
    static void PrintGrid(List<Point> shortestPath, List<List<Point>> allPaths)
    {
        var grids = new Grid();
        for (int j = 0; j < grid.GetLength(1); j++)
        {
            grids.AddColumn();
        }

        // Print the grid with paths
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            var rowItems = new List<IRenderable>();

            for (int j = 0; j < grid.GetLength(1); j++)
            {

                if (grid[i, j] == (int)Cell.Wall)
                {
                    rowItems.Add(new Spectre.Console.Text("X ", new Style(Spectre.Console.Color.Red)));
                }
                else if (grid[i, j] == (int)Cell.Exit)
                {
                    rowItems.Add(new Spectre.Console.Text("E ", new Style(Spectre.Console.Color.Aqua)));
                }
                else
                {
                    if (shortestPath != null && shortestPath.Exists(p => p.X == i && p.Y == j))
                    {
                        rowItems.Add(new Spectre.Console.Text("* ", new Style(Spectre.Console.Color.Blue)));
                    }
                    else
                    {
                        rowItems.Add(new Spectre.Console.Text("_ ", new Style(Spectre.Console.Color.Green)));
                    }
                }

            }
            grids.AddRow(rowItems.ToArray());
            AnsiConsole.WriteLine();
        }

        AnsiConsole.Write(grids);
        

        // Print all available paths
        var pathText = new List<string>();
        AnsiConsole.WriteLine("\nAll Available Paths:");
        HashSet<List<Point>> distinctPaths = new HashSet<List<Point>>(new PathComparer());
        int pathCount = 1;
        foreach (var path in allPaths)
        {
            if (!distinctPaths.Contains(path))
            {
                var pathLine = new List<string>();

                foreach (var point in path)
                {
                    pathLine.Add($"({point.X},{point.Y}) -> ");
                }
                pathText.Add(new($"Path {pathCount++}: {string.Join("", pathLine)}"));
            }
        }
        AnsiConsole.WriteLine($"Number of Paths: {pathText.Count}");
        AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("")
            .PageSize(10)
            .MoreChoicesText("Scroll down or enter any button to exit")
            .AddChoices(pathText));

    }
}

enum Cell
{
    OpenSpace = 0,
    Wall = 1,
    Exit = 2,
    Start = 3
}
