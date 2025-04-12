using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public static PathFinding Instance { get; private set; }

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private LayerMask obstaclesLayerMask;

    private int height;
    private int width;
    private float cellSize;
    private GridSystem<PathNode> gridSystem;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than one PathFinding!" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        gridSystem = new GridSystem<PathNode>(10, 10, 2f, 
            (GridSystem<PathNode> gameObject, GridPosition gridPosition) => new PathNode(gridPosition));
        // gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

    }

    public void SetUp(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        gridSystem = new GridSystem<PathNode>(width, height, cellSize,
            (GridSystem<PathNode> gameObject, GridPosition gridPosition) => new PathNode(gridPosition));


        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

        for(int x =0;x < width; x++)
        {
            for (int z =0;z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x,z);
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                float raycastOffsetDistance = 5f;
                if(Physics.Raycast(
                    worldPosition + Vector3.down * raycastOffsetDistance, Vector3.up,
                    raycastOffsetDistance * 2, obstaclesLayerMask))
                {
                    GetNode(x, z).SetIsWalkable(false);
                }
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();


        PathNode startNode = gridSystem.GetGridObject(startGridPosition);
        PathNode endNode = gridSystem.GetGridObject(endGridPosition);
        openList.Add(startNode);
        for(int x =0; x < gridSystem.GetWidth(); x++)
        {
            for(int z=0; z< gridSystem.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode pathNode = gridSystem.GetGridObject(gridPosition);

                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if(currentNode == endNode)
            {
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach(PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                //if already go though skips.
                if (closedList.Contains(neighbourNode))
                {
                    continue;
                }
                //skup unwalkable Node;
                if (!neighbourNode.IsWalkable())
                {
                    closedList.Add(neighbourNode);
                    continue;
                }
                //calculate g cost to see which is the best route.
                int tentativeGCost = 
                    currentNode.GetGCost() + 
                    CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());

                if(tentativeGCost < neighbourNode.GetGCost())
                {
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.SetGCost(tentativeGCost);
                    neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(), endGridPosition));
                    neighbourNode.CalculateFCost();
                    //see the best route => add node to the openList(route/path).

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }
        pathLength = 0;
        //No path found
        return null;
    }

    public int CalculateDistance(GridPosition gridA, GridPosition gridB)
    {
        GridPosition gridPositionDistance = gridA - gridB;
        int distance = Mathf.Abs(gridPositionDistance.x) + Mathf.Abs(gridPositionDistance.z);
        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        int remaining = Mathf.Abs(xDistance - zDistance);
        return MOVE_DIAGONAL_COST *  Mathf.Min(xDistance,zDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        return FindPath(startGridPosition, endGridPosition,out int pathLength) != null;
    }

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostPathNode = pathNodeList[0];
        for(int i =0;i< pathNodeList.Count; i++)
        {
            if (pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost())
            {
                lowestFCostPathNode = pathNodeList[i];
            }
        }
        return lowestFCostPathNode;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();


        GridPosition gridPosition = currentNode.GetGridPosition();

        //--GET NEIGHBOUR GRID--
        //LEFT NEIGHBOUR
        if(gridPosition.x -1 >= 0)
        {
            //Get LEFT NEIGHTBOUR
            neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0));

            if(gridPosition.z -1 >= 0)
            {
                //Get LEFT-DOWN NEIGHTBOUR
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1));
            }
            if(gridPosition.z +1 < gridSystem.GetHeight())
            {
                //Get LEFT-UP NEIGHTBOUR
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1));
            }
            
        }
        //RIGHT NEIGHBOUR
        if(gridPosition.x +1 < gridSystem.GetWidth())
        {
            //Get RIGHT NEIGHTBOUR
            neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0));

            if (gridPosition.z -1 >= 0)
            {
                //Get RIGHT DOWN NEIGHTBOUR
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1));
            }

            if (gridPosition.z + 1 < gridSystem.GetHeight())
            {
                //Get RIGHT UP NEIGHTBOUR
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1));
            }

        }


        if (gridPosition.z +1 < gridSystem.GetHeight())
        {
            //Get RIGHT UP NEIGHTBOUR
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1));
        }

        if (gridPosition.z - 1 >= 0)
        {
            //Get RIGHT DOWN NEIGHTBOUR
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1));
        }
        return neighbourList;
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode>();
        pathNodeList.Add(endNode);
        PathNode currentNode = endNode;
        while(currentNode.GetCameFromPathNode() != null)
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }

        pathNodeList.Reverse();

        List<GridPosition> gridPostionList = new List<GridPosition>();
        foreach (PathNode pathNode in pathNodeList)
        {
            gridPostionList.Add(pathNode.GetGridPosition());
        }

        return gridPostionList; 
    }

    public bool IsWalkableGridPosition(GridPosition gridPosition)
    {
        return gridSystem.GetGridObject(gridPosition).IsWalkable(); 
    }
    public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable)
    {
        gridSystem.GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }

    private PathNode GetNode(int x, int z)
    {
        return gridSystem.GetGridObject(new GridPosition(x,z));
    }
}
