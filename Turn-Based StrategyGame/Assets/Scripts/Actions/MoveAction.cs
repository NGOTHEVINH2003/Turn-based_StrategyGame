using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;
    [SerializeField] private float MoveSpeed = 4.0f;
    [SerializeField] private int maxMoveDistance = 2;

    private float stoppingDistance = .1f;
    private float rotatingSpeed = 5f;
    private Vector3 targetPosition;


    protected override void Awake()
    {
        base.Awake();
        targetPosition = transform.position;
    }

  

    
    void Update()
    {
        if (!isActive)
        {
            return;
        }

        Vector3 moveDir = (targetPosition - transform.position).normalized;

        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {

            transform.position += moveDir * Time.deltaTime * MoveSpeed;
        }
        else
        {
            OnStopMoving?.Invoke(this, EventArgs.Empty);
            onActionComplete();
        }

        transform.forward = Vector3.Lerp(transform.forward, moveDir, Time.deltaTime * rotatingSpeed);
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        
        this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        OnStartMoving?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }


    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                    continue;
                }

                if (testGridPosition == unitGridPosition)
                {
                    //same grid avoid wasting a turn
                    continue;
                }
                if (LevelGrid.Instance.UnitExistOnThisGrid(testGridPosition))
                {
                    //Grid already exist an unit
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }
    


    public override string GetActionName()
    {
        return "Move";
    }
    protected override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10,
        };
    }
}
