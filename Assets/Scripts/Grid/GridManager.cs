using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grid
{
    public class GridManager : MonoBehaviour
    {
        private static GridManager _instance;

        public static GridManager Instance { get => _instance; private set => _instance = value; }

        [SerializeField]private Vector2Int gridSize;
        [SerializeField] private float cellSize = 0.5f;
        private Dictionary<Vector2Int, HexCell> _gridSet;


        void Awake()
        {
            _instance = this;
            _gridSet = new Dictionary<Vector2Int, HexCell>();
        
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
                    newCell.Init(xIndex, yIndex, cellSize, this.transform.position);
                    _gridSet.Add(new Vector2Int(xIndex, yIndex), newCell);
                }
            }

            var hexList = _gridSet.Values.ToList();
            foreach (var hexCell in hexList)
            {
                hexCell.CacheNeighbors(hexList);
            }
        }

        private void Update()
        {
            foreach (var hexCell in _gridSet)
            {
                // Debug.DrawLine();
                hexCell.Value.DebugDraw();
            }
        }


        public bool CalculatePath(HexCell startCell)
        {

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

                //if (currentCell.Coords.X == testBrainCoords.x && currentCell.Coords.Y == testBrainCoords.y)
                //{
                //    return true;
                //}

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
                            //nextCellTest.Pathfinding.H = nextCellTest.GetDistance(testBrainCoords);
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
}
