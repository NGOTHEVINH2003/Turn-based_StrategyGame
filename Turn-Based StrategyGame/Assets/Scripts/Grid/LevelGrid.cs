using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    

    public static LevelGrid Instance { get; private set; }

    public event EventHandler OnAnyUnitMovePosition;

  

    private GridSystem<GridObject> gridSystem;
    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private int height;
    [SerializeField] private int width;
    [SerializeField] private float cellSize;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than one LevelGrid!" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        gridSystem = new GridSystem<GridObject>(width, height, cellSize, 
            (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject( gridPosition, g));
       // gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }

    private void Start()
    {
        PathFinding.Instance.SetUp(width, height, cellSize);
    }

    public List<Unit> GetUnitAtGridPos(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnitList();
    }
    public void AddUnitAtGridPos(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.AddUnit(unit);

    }
  
    public void RemoveUnitAtGridPos(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);
    }

    public void UnitMovePos(Unit unit, GridPosition from, GridPosition to)
    {
        RemoveUnitAtGridPos(from, unit);
        AddUnitAtGridPos(to, unit);
        OnAnyUnitMovePosition?.Invoke(this, EventArgs.Empty);
    }

    public bool UnitExistOnThisGrid(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();
    }

    public int GetWidth() => gridSystem.GetWidth();
    public int GetHeight() => gridSystem.GetHeight();
    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);
    public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPos(gridPosition);
    public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);

    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnit();
    }

  
}
