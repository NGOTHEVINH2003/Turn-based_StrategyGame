using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject 
{
    private GridPosition gridPosition;
    private GridSystem<GridObject> gridSystem;
    private List<Unit> unitList;
    private IInteractable interactable;

    public GridObject(GridPosition gridPosition, GridSystem<GridObject> gridSystem) 
    {
        this.gridPosition = gridPosition;
        this.gridSystem = gridSystem;
        unitList = new List<Unit>();
    }

    public override string ToString()
    {
        string unitString = "";
        foreach (Unit unit in unitList)
        {
            unitString += unit.ToString() + "\n";
        }
        return gridPosition.ToString() + "\n" + unitString;
    }


    public List<Unit> GetUnitList()
    {
        return unitList;
    }

    public void AddUnit(Unit unit)
    {
        unitList.Add(unit);
    }
    public void RemoveUnit(Unit unit)
    {
        unitList.Remove(unit);
    }

    public bool HasAnyUnit()
    {
        return unitList.Count > 0;
    }

    public Unit GetUnit()
    {
        if (HasAnyUnit())
        {
            return unitList[0];
        }

        return null;
    }

    public IInteractable GetInteractale()
    {
        return interactable;
    }

    public void SetInteractable(IInteractable interactable) { 
        this.interactable = interactable;
    }
}
