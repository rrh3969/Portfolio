using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace gdapsProject_teamF
{
    public class SpatialPartition<T>
    {
        //Max ammount of colliders each node can have
        private int MaxObjectsPerNode = 10;
        //The max number of subdives A base quadtree can do before it can no longer be subdivided
        private int MaxDepth = 5;
        //The current depth of the node
        private int currentDepth;
        //the bounds of the current node
        private Rectangle partitionArea;
        //The list of all the colliders stored in this node
        private List<T> storedColliders;
        //an array of child nodes
        private SpatialPartition<T>[] children;

        public SpatialPartition(Rectangle area, int currentDepth)
        {
            partitionArea = area;
            storedColliders = new List<T>();
            children = new SpatialPartition<T>[4];
            this.currentDepth = currentDepth;
        }

        /// <summary>
        /// This method takes in an obj and a rectangle and inserts into a node
        /// If a node is full and we are not at the max depth then it will subdivide and add the object into the new node
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="bounds"></param>
        public void Insert(T obj, Rectangle bounds)
        {
            //if the object we are trying to insert is not within the bounds of this child of the quad tree then return nothing
            if (!partitionArea.Intersects(bounds))
            {
                return;
            }

            //This checks if we have reached our max depth or if we have less then the max number of objects this node can have
            //and this node has no children, then add the object to this nodes list of objects if either of this conditions are met
            if (currentDepth >= MaxDepth || (storedColliders.Count <= MaxObjectsPerNode && children[0] == null))
            {
                storedColliders.Add(obj);
            }

            else
            {
                //This happens when we have no children and we have reached the max number of objects for this current node.
                //It then subdivides this node into 4 child nodes
                if (children[0] == null)
                {
                    SubDivide();
                }

                //Finally it recursively calls itself for each child so that it may populate the children with colliders
                for (int i = 0; i < children.Length; i++)
                {
                    children[i].Insert(obj, bounds);
                }
            }
        }

        /// <summary>
        /// Used to find a list of object a passed in collider is colliding with
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public List<T> Query(Rectangle bounds)
        {
            //List of results that we get from this Query
            List<T> results = new List<T>();

            //First checks if this node even intersects what we are trying to intersect, if it does not then return no results
            if (!partitionArea.Intersects(bounds))
            {
                return results;
            }

            //for each object in this node check if the collider we are checking against collides with it, if it does add it to the list of results
            foreach (T obj in storedColliders)
            {
                if (bounds.Intersects(GetBounds(obj)))
                {
                    results.Add(obj);
                }
            }

            //If there are children, for every child recursively query them and add the results of thier query to the results of this query
            //I think this makes it so that only base nodes return the list of all the results.
            if (children[0] != null)
            {
                for (int i = 0; i < children.Length; i++)
                {
                    results.AddRange(children[i].Query(bounds));
                }
            }

            //Then finally return the results
            return results;
        }

        private void SubDivide()
        {
            int width = partitionArea.Width / 2;
            int height = partitionArea.Height / 2;

            children[0] = new SpatialPartition<T>(new Rectangle(partitionArea.X, partitionArea.Y, width, height), currentDepth + 1);
            children[1] = new SpatialPartition<T>(new Rectangle(partitionArea.X + width, partitionArea.Y, width, height), currentDepth + 1);
            children[2] = new SpatialPartition<T>(new Rectangle(partitionArea.X, partitionArea.Y + height, width, height), currentDepth + 1);
            children[3] = new SpatialPartition<T>(new Rectangle(partitionArea.X + width, partitionArea.Y + height, width, height), currentDepth + 1);

        }

        /// <summary>
        /// This function gets the current bounds of the object we are trying to acess.
        /// It is useful for allowing us to get the rectangle of the generic Type T object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Rectangle GetBounds(T obj)
        {
            //Checks if T has a property called position, and if it does not throw an exception
            if (typeof(T).GetProperty("Position") == null)
            {
                throw new InvalidOperationException("Sry, this can only be used with GameObjects");
            }

            //Returns the value of the "Position" property of our obj
            return (Rectangle)typeof(T).GetProperty("Position").GetValue(obj);
        }

        public Rectangle PartitionArea
        {
            get { return partitionArea; }
        }

        public int TotalColliderAmmount()
        {
            int total = 0;
            if (children[0] == null)
            {
                return storedColliders.Count;
            }
            total += storedColliders.Count;
            for (int i = 0; i < children.Length; i++)
            {
                total += children[i].TotalColliderAmmount();
            }

            return total;
        }
    }
}
