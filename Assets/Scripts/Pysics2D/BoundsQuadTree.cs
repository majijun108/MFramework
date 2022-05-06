using Lockstep.Math;
using Lockstep.UnsafeCollision2D;
using System.Collections.Generic;
using UnityEngine;

public partial class BoundsQuadTree
{
    public int Count { get; private set; }

    // Root node of the octree
    BoundsQuadTreeNode rootNode;

    // Should be a value between 1 and 2. A multiplier for the base size of a node.
    // 1.0 is a "normal" octree, while values > 1 have overlap
    readonly LFloat looseness;

    // Size that the octree was on creation
    readonly LFloat initialSize;

    // Minimum side length that a node can be - essentially an alternative to having a max depth
    readonly LFloat minSize;

    /// <summary>
    /// Constructor for the bounds octree.
    /// </summary>
    /// <param name="initialWorldSize">Size of the sides of the initial node, in metres. The octree will never shrink smaller than this.</param>
    /// <param name="initialWorldPos">Position of the centre of the initial node.</param>
    /// <param name="minNodeSize">Nodes will stop splitting if the new nodes would be smaller than this (metres).</param>
    /// <param name="loosenessVal">Clamped between 1 and 2. Values > 1 let nodes overlap.</param>
    public BoundsQuadTree(LFloat initialWorldSize, LVector2 initialWorldPos, LFloat minNodeSize, LFloat loosenessVal)
    {
        if (minNodeSize > initialWorldSize)
        {
            Debug.Log("Minimum node size must be at least as big as the initial world size. Was: " +
                             minNodeSize + " Adjusted to: " + initialWorldSize);
            minNodeSize = initialWorldSize;
        }

        Count = 0;
        initialSize = initialWorldSize;
        minSize = minNodeSize;
        looseness = LMath.Clamp(loosenessVal, 1.ToLFloat(), 2.ToLFloat());
        rootNode = new BoundsQuadTreeNode(null, initialSize, minSize, looseness, initialWorldPos);
    }

    public void UpdateObj(ColliderProxy obj, LRect bound)
    {
        var node = GetNode(obj);
        if (node == null)
        {
            Add(obj, bound);
        }
        else
        {
            if (!node.ContainBound(bound))
            {
                Remove(obj);
                Add(obj, bound);
            }
            else
            {
                node.UpdateObj(obj, bound);
            }
        }
    }
    // #### PUBLIC METHODS ####


    // #### PUBLIC METHODS ####
    public BoundsQuadTreeNode GetNode(ColliderProxy obj)
    {
        if (BoundsQuadTreeNode.obj2Node.TryGetValue(obj, out var val))
        {
            return val;
        }

        return null;
    }

    /// <summary>
    /// Add an object.
    /// </summary>
    /// <param name="obj">Object to add.</param>
    /// <param name="objBounds">3D bounding box around the object.</param>
    public void Add(ColliderProxy obj, LRect objBounds)
    {
        // Add object or expand the octree until it can be added
        int count = 0; // Safety check against infinite/excessive growth
        while (!rootNode.Add(obj, objBounds))
        {
            Debug.LogError("Grow");
            Grow(objBounds.center - rootNode.Center);
            if (++count > 20)
            {
                Debug.LogError("Aborted Add operation as it seemed to be going on forever (" + (count - 1) +
                               ") attempts at growing the octree.");
                return;
            }
        }

        Count++;
    }

    /// <summary>
    /// Remove an object. Makes the assumption that the object only exists once in the tree.
    /// </summary>
    /// <param name="obj">Object to remove.</param>
    /// <returns>True if the object was removed successfully.</returns>
    public bool Remove(ColliderProxy obj)
    {
        bool removed = rootNode.Remove(obj);

        // See if we can shrink the octree down now that we've removed the item
        if (removed)
        {
            Count--;
            Shrink();
        }

        return removed;
    }

    /// <summary>
    /// Removes the specified object at the given position. Makes the assumption that the object only exists once in the tree.
    /// </summary>
    /// <param name="obj">Object to remove.</param>
    /// <param name="objBounds">3D bounding box around the object.</param>
    /// <returns>True if the object was removed successfully.</returns>
    public bool Remove(ColliderProxy obj, LRect objBounds)
    {
        bool removed = rootNode.Remove(obj, objBounds);

        // See if we can shrink the octree down now that we've removed the item
        if (removed)
        {
            Count--;
            Shrink();
        }

        return removed;
    }

    /// <summary>
    /// Check if the specified bounds intersect with anything in the tree. See also: GetColliding.
    /// </summary>
    /// <param name="checkBounds">bounds to check.</param>
    /// <returns>True if there was a collision.</returns>
    public bool IsColliding(ColliderProxy obj, LRect checkBounds)
    {
        //#if UNITY_EDITOR
        // For debugging
        //AddCollisionCheck(checkBounds);
        //#endif
        return rootNode.IsColliding(obj, ref checkBounds);
    }

    /// <summary>
    /// Returns an array of objects that intersect with the specified bounds, if any. Otherwise returns an empty array. See also: IsColliding.
    /// </summary>
    /// <param name="collidingWith">list to store intersections.</param>
    /// <param name="checkBounds">bounds to check.</param>
    /// <returns>Objects that intersect with the specified bounds.</returns>
    public void GetColliding(List<ColliderProxy> collidingWith, LRect checkBounds)
    {
        //#if UNITY_EDITOR
        // For debugging
        //AddCollisionCheck(checkBounds);
        //#endif
        rootNode.GetColliding(ref checkBounds, collidingWith);
    }


    public LRect GetMaxBounds()
    {
        return rootNode.GetBounds();
    }
#if UNITY_EDITOR
    /// <summary>
    /// Draws node boundaries visually for debugging.
    /// Must be called from OnDrawGizmos externally. See also: DrawAllObjects.
    /// </summary>
    //public void DrawAllBounds()
    //{
    //    rootNode.DrawBoundQuadTreeNode(0);
    //}

    ///// <summary>
    ///// Draws the bounds of all objects in the tree visually for debugging.
    ///// Must be called from OnDrawGizmos externally. See also: DrawAllBounds.
    ///// </summary>
    //public void DrawAllObjects()
    //{
    //    rootNode.DrawAllObjects();
    //}
#endif

    public const int NUM_CHILDREN = 4;

    /// <summary>
    /// Grow the octree to fit in all objects.
    /// </summary>
    /// <param name="direction">Direction to grow.</param>
    void Grow(LVector2 direction)
    {
        int xDirection = direction.x >= 0 ? 1 : -1;
        int yDirection = direction.y >= 0 ? 1 : -1;
        BoundsQuadTreeNode oldRoot = rootNode;
        LFloat half = rootNode.BaseLength / 2;
        LFloat newLength = rootNode.BaseLength * 2;
        LVector2 newCenter = rootNode.Center + new LVector2(xDirection * half, yDirection * half);

        // Create a new, bigger octree root node
        rootNode = new BoundsQuadTreeNode(null, newLength, minSize, looseness, newCenter);

        if (oldRoot.HasAnyObjects())
        {
            // Create 7 new octree children to go with the old root as children of the new root
            int rootPos = rootNode.BestFitChild(oldRoot.Center);
            BoundsQuadTreeNode[] children = new BoundsQuadTreeNode[NUM_CHILDREN];
            for (int i = 0; i < NUM_CHILDREN; i++)
            {
                if (i == rootPos)
                {
                    children[i] = oldRoot;
                }
                else
                {
                    xDirection = i % 2 == 0 ? -1 : 1;
                    yDirection = i > 1 ? -1 : 1;
                    children[i] = new BoundsQuadTreeNode(rootNode, oldRoot.BaseLength, minSize, looseness,
                        newCenter + new LVector2(xDirection * half, yDirection * half));
                }
            }

            // Attach the new children to the new root node
            rootNode.SetChildren(children);
        }
    }

    /// <summary>
    /// Shrink the octree if possible, else leave it the same.
    /// </summary>
    void Shrink()
    {
        rootNode = rootNode.ShrinkIfPossible(initialSize);
    }
}
