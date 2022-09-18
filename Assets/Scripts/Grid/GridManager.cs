using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;


public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField]
    public Vector2Int gridSize;
    private Dictionary<Vector2Int, HexCell> GridSet;

    private Grid _gridHelper;


    public Vector2Int testSpawnerCoords;
    public Vector2Int testBrainCoords;

    
    void Awake()
    {
        Instance = this;
        GridSet = new Dictionary<Vector2Int, HexCell>();
        GenerateGrid(gridSize.x, gridSize.y);
    }


    private void GenerateGrid(int gridWidth, int gridNumRows, bool fromCenter = false)
    {
        for (int yIndex = 0; yIndex < gridNumRows; yIndex++)
        {
            var rowOffset = yIndex >>1;
            for (int xIndex = -rowOffset; xIndex < gridWidth - rowOffset; xIndex++)
            {
                var newCell = new HexCell();
                GridSet.Add(new Vector2Int(xIndex, yIndex), newCell);

            }
        }

        var hexList = GridSet.Values.ToList();
        foreach (var hexCell in hexList)
        {
            hexCell.CacheNeighbors(hexList);
        }
    }



    public bool CalculatePath(HexCell startCell)
    {
        var target = testBrainCoords;

        var cellPathsToTest = new List<HexCell> { startCell };
        var processed = new List<HexCell>();

        while (cellPathsToTest.Any())
        {
            // just get the 
            HexCell currentCell = cellPathsToTest.First();
            if(cellPathsToTest.Count > 1)
            {
                foreach (var t in cellPathsToTest)
                {
                    if (t.Pathfinding.F < currentCell.Pathfinding.F ||
                        // ReSharper disable once CompareOfFloatsByEqualityOperator
                        (t.Pathfinding.F == currentCell.Pathfinding.F && t.Pathfinding.H < currentCell.Pathfinding.H))
                    {
                        currentCell = t;
                    }
                }
            }

            processed.Add(currentCell);
            cellPathsToTest.Remove(currentCell);

            if (currentCell.Coords.X == testBrainCoords.x && currentCell.Coords.Z == testBrainCoords.y)
            {
                return true;
            }

            foreach (var nextCellTest in currentCell.NeighborCellCache.Where(cell => cell.Walkable && 
                                                                                    !processed.Contains(cell)))
            {
                var costForMove = currentCell.Pathfinding.G + currentCell.GetDistance(nextCellTest.Coords);


                var inSearch = cellPathsToTest.Contains(nextCellTest);

                if (!inSearch || costForMove < nextCellTest.Pathfinding.G)
                {
                    nextCellTest.Pathfinding.G = costForMove;

                    if (!inSearch)
                    {
                        // set the total distance to the goal
                        nextCellTest.Pathfinding.H = nextCellTest.GetDistance(testBrainCoords);
                        cellPathsToTest.Add(nextCellTest);

                    }
                }

            }

        }

        // something has gone really wrong
        Debug.LogError("Pathfinding couldnt get to destination!!");

        return false;
    }
}
