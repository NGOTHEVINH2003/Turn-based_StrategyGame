using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridSystem<TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    private TGridObject[,] gridObjectArray;

    public GridSystem(int width, int height, float cellSize, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createTGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridObjectArray = new TGridObject[width, height];
        for(int x=0; x< width; x++)
        {
            for(int z=0; z < height; z ++)
            {
                GridPosition gridPosition = new GridPosition(x,z);
                gridObjectArray[x, z] = createTGridObject(this, gridPosition);
            }
        }
    }

    public void CreateDebugObjects(Transform debugPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);

                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPos(gridPosition), Quaternion.identity);
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(gridPosition) as GridObject);
              
            }
        }
    }

    public TGridObject GetGridObject (GridPosition gridPosition)
    {
        return gridObjectArray[gridPosition.x, gridPosition.z];
    }

    public Vector3 GetWorldPos(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0 , gridPosition.z) * cellSize;
    }

    public GridPosition GetGridPosition(Vector3 worldPos)
    {
        return new GridPosition(
            Mathf.RoundToInt(worldPos.x / cellSize),
            Mathf.RoundToInt(worldPos.z / cellSize)
            );
    }

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return  gridPosition.x >= 0 &&
                gridPosition.z >= 0 &&
                gridPosition.x < width &&
                gridPosition.z < height;
    }

    public int GetWidth()
    {
        return width;
    }
    public int GetHeight()
    {
        return height;
    }
}
