using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RemGame
{
    class PathNode
    {
        #region Declarations
        public PathNode ParentNode;
        public PathNode EndNode;
        private Vector2 gridLocation;
        public float TotalCost;
        public float DirectCost;
        #endregion

        #region Properties
  

        public int GridX
        {
            get { return (int)GridLocation.X; }
        }

        public int GridY
        {
            get { return (int)GridLocation.Y; }
        }

        public Vector2 GridLocation { get => gridLocation; set => gridLocation = value; }

        #endregion

        #region Constructor
        public PathNode(
            PathNode parentNode,
            PathNode endNode,
            Vector2 gridLocation,
            float cost)
        {
            ParentNode = parentNode;
            GridLocation = gridLocation;
            EndNode = endNode;
            DirectCost = cost;
            if (!(endNode == null))
            {
                TotalCost = DirectCost + EuclidainCost();
            }
        }
        #endregion

        #region Helper Methods
        public float EuclidainCost()
        {

            return
          (float)Math.Sqrt(Math.Pow((double)EndNode.GridLocation.X - GridLocation.X, 2) + Math.Pow((double)EndNode.GridLocation.Y - GridLocation.Y, 2));

        }
        #endregion

        #region Public Methods
        public bool IsEqualToNode(PathNode node)
        {
            return (GridLocation == node.GridLocation);
        }
        #endregion

    }
}
