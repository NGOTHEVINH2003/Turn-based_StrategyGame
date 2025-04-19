using System.Collections;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpinAction : BaseAction
{


    /*public delegate void SpinCompleteDelegate();*/

    private int maxBandageDistance = 1;
    private Unit targetUnit;
    private float timer;
    private float maxSpinAmount = 10f;

    void Update()
    {
        if (!isActive)
        {
            return;
        }
        timer -= Time.deltaTime;

        Vector3 spinDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
        transform.forward = Vector3.Lerp(transform.forward, spinDir, Time.deltaTime * maxSpinAmount);
        if(timer <= 0f)
        {
            onActionComplete();
        }
    }

    public override string GetActionName()
    {
        return "Bandage";
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        targetUnit.Heal(40);
        float healTime = 0.5f;
        timer = healTime;
        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxBandageDistance; x <= maxBandageDistance; x++)
        {
            for (int z = -maxBandageDistance; z <= maxBandageDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                if(targetUnit == null)
                {
                    continue;
                }

                if (targetUnit.IsEnemy() != unit.IsEnemy())
                {
                    //check for same Team
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;

    }

    public override int GetActionPointsCost()
    {
        return 1;
    }

    protected override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition
            ,
            actionValue = 0,
        };
    }
}
