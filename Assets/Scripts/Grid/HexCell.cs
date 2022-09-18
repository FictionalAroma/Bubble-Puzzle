﻿
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grid
{
    public class HexCell
    {
        public HexCoords Coords;
        public List<HexCell> NeighborCellCache { get; private set; }

        public PathfindingValues Pathfinding;
        private float _cellOffset;

        public bool Walkable { get; set; }

        public Vector3 Position { get; private set; }
        // Use this for initialization
        internal void Init(int xIndex, int yIndex, float cellOffset, Vector2 startPosition)
        {
            NeighborCellCache = new List<HexCell>();
            Pathfinding = new PathfindingValues();
            Walkable = true;

            Coords = new HexCoords()
            {
                X = xIndex,
                Y = yIndex,
            };

            _cellOffset = cellOffset;
            var posVector = startPosition + (xIndex * new Vector2(_cellOffset, 0) + yIndex * new Vector2(_cellOffset/2, _cellOffset));

            Position = posVector;
        }
        public void CacheNeighbors(List<HexCell> hexList)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
           NeighborCellCache = hexList.Where(cell => !cell.Coords.Equals(this.Coords)  && GetDistance(cell.Coords) == 1).ToList();
        }

        public float GetDistance(HexCoords other) => Coords.GetDistance(other);
        public float GetDistance(Vector2Int other) => Coords.GetDistance(new HexCoords{X = other.x, Y = other.y});

        public void DebugDraw()
        {
            Debug.DrawLine(Position, (Vector2)Position + Vector2.up * _cellOffset/2, Color.red);
            Debug.DrawLine(Position, (Vector2)Position + Vector2.left * _cellOffset / 2, Color.red);
            Debug.DrawLine(Position, (Vector2)Position + Vector2.right * _cellOffset / 2, Color.red);
            Debug.DrawLine(Position, (Vector2)Position + Vector2.down * _cellOffset / 2, Color.red);
            
        }
    }

    public struct HexCoords
    {
        public int X;

        /// <summary>
        /// Think of this one as "Y", but in hexes its Y
        /// </summary>
        public int Y;

        public override string ToString() => $"{X},{Y}";

        public float GetDistance(HexCoords other)
        {
            var coordsDistance = new HexCoords
            {
                Y = this.Y - other.Y,
                X = this.X - other.X,
            };
            var distanceResult = coordsDistance.HexGridLengthAway();

            return distanceResult;
        }

        public float HexGridLengthAway()
        {
            // first if the coords are 0, then the length is 0
            if (X == 0 && Y == 0) return 0;

            // with that out of the way, the craziness begins!
            // no i dont understand what is happening really, i have tried my best to
            // taken from https://www.youtube.com/watch?v=i0x5fj4PqP4
            // please watch this video before even attempting to adjust this code


            // effectivly at this point we have a Vector2 in Grid Form and we are calculating distance of these coords from 0/0
            // there are 6 possible returns, covering all directions
            // if the X is greater than 0, and the Y is or greater than 0, then we are in 1+/1+ range on the grid (aka top right)
            // then we can just return the sum of the two coords
            if (X > 0 && Y >= 0) return X + Y;

            // next up the 0-/1+ range (aka going Left)
            if (X <= 0 && Y > 0)
            {
                // next up we need to see if the positive value of X < Y
                // we have already determined that X is negative, so doing a negative flips the sign
                var positiveX = -X;
                // if positive X is Less than Y, then return the Y distance, otherwise the positive X
                // distances cant be 0 so
                return positiveX < Y ? Y : positiveX;
            }

            // if we get here then BOTH coords are < 0
            // then we can return the inverse sum if the X is negative
            if (X < 0) return -X - Y;

            // finally if all else fails
            // if the Inverse of Y is greater than Q, return InverseX, otherwise return Y;
            return -Y > X ? -Y : X;
        }

        #region EqualityOperators
        public static bool operator ==(HexCoords coords, Vector2Int other)
        {
            return coords.X == other.x && coords.Y == other.y;
        }

        public static bool operator !=(HexCoords coords, Vector2Int other)
        {
            return coords.X != other.x || coords.Y != other.y;
        }

        public bool Equals(HexCoords other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is HexCoords other && Equals(other);
        }

        #endregion

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

    }


    public struct PathfindingValues
    {
        /// <summary>
        /// Distance from Start
        /// </summary>
        public float G { get; set; }
        // distance to target
        public float H { get; set; }

        // final calculation of cost
        public float F => G + H;

        public override string ToString() => $"G - {G}{Environment.NewLine}H - {H}{Environment.NewLine}F-{F}";
    }

}