using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    
    public static GridSystemVisual Instance { get; private set; }

    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }
    public enum GridVisualType { White, Blue, Yellow, Red, RedSoft }


    [SerializeField] private List<GridVisualTypeMaterial> materials;
    [SerializeField] private Transform gridSystemVisualPrefab;
    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than one Unit Action System!" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        gridSystemVisualSingleArray = new GridSystemVisualSingle[
            LevelGrid.Instance.GetWidth(),
            LevelGrid.Instance.GetHeight()
        ];
        for(int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for(int z =0;z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x , z);
                Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualPrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);

                gridSystemVisualSingleArray[x , z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }

        UnitActionSystem.Instance.OnSelectedActionChanged += Instance_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitMoveToGridPosition += Instance_OnAnyUnitMovePosition;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
        UpdateGridVisual();
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void Instance_OnAnyUnitMovePosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }
    private void UpdateGridVisual()
    {
        HideAllGridPosition();
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        GridVisualType gridVisualType;
        switch (selectedAction)
        {
            default:
            case MoveAction moveAction:
                gridVisualType = GridVisualType.White;
                break;
            case ShootAction shootAction:
                gridVisualType= GridVisualType.Red;
                //show range.
                ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetMaxShootDistanceRange(), GridVisualType.RedSoft);
                break;
            case SwordAction swordAction:
                gridVisualType= GridVisualType.Red;
                ShowGridPositionRangeSquare(selectedUnit.GetGridPosition(), swordAction.GetMaxSwordDistance(), GridVisualType.RedSoft);
                break;
            case ThrowGrenadeAction grenadeAction:
                gridVisualType= GridVisualType.Yellow;
                ShowGridPositionRange(selectedUnit.GetGridPosition(), grenadeAction.GetMaxThrowDistance(), GridVisualType.RedSoft);
                break;
            case InteractAction interactAction:
                gridVisualType= GridVisualType.Blue;
                break;
            case SpinAction spinAction:
                gridVisualType= GridVisualType.Blue;
                break;
        }

        ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType); 
    }
    private void Instance_OnSelectedActionChanged(object sender, System.EventArgs e)
    {
        UpdateGridVisual();
    }

    public void HideAllGridPosition()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                gridSystemVisualSingleArray[x, z].Hide();
            }
        }
    }
    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositions = new List<GridPosition>();
        for (int x = -range; x <= range; x++){
            for(int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > range)
                {
                    continue;
                }


                gridPositions.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositions, gridVisualType);
    }
    private void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositions = new List<GridPosition>();
        for (int x = -range; x <= range; x++){
            for(int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }


                gridPositions.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositions, gridVisualType);
    }
    public void ShowGridPositionList(List<GridPosition> gridPositions, GridVisualType gridVisualType)
    {
        foreach(GridPosition gridPosition in gridPositions)
        {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in materials) 
        {
            if(gridVisualTypeMaterial.gridVisualType == gridVisualType) return gridVisualTypeMaterial.material;
        }
        Debug.LogError("Could not find GridVisualTypeMAterial for GridVisualType: " + gridVisualType + " at: " + this);
        return null;
    }
}
