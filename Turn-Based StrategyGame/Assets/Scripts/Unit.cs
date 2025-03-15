using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UIElements;

public class Unit : MonoBehaviour
{
   
 
    private GridPosition gridPosition;
    private MoveAction moveAction;
    private SpinAction spinAction;
    private int actionPoints = 2;
    private BaseAction[] baseActionArray;


    private void Awake()
    {
        moveAction = GetComponent<MoveAction>();
        spinAction = GetComponent<SpinAction>();
        baseActionArray = GetComponents<BaseAction>();
    }


    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPos(gridPosition, this);
    }

    private void Update()
    {

        //update unit position
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if(newGridPosition != gridPosition)
        {
            LevelGrid.Instance.UnitMovePos(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }

    }
    public bool TrySpendActionPoints(BaseAction baseAction)
    {
        if (CanSpendActionPoint(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        return false;
    }
    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;
    }


    public bool CanSpendActionPoint(BaseAction baseAction)
    {
        if(actionPoints >= baseAction.GetActionPointsCost())
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public int GetActionPoints()
    {
        return actionPoints;
    }

    public SpinAction GetSpinAction() 
    {  
        return spinAction; 
    } 

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public MoveAction GetMoveAction()
    {
        return moveAction;
    }

    public BaseAction[] GetBaseActionsArray()
    {
        return baseActionArray;
    }

}
