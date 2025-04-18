using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGrenadeAction : BaseAction
{
    private int maxThrowDistance = 5;
    [SerializeField] private Transform grenadeProjectilePrefab;
    public override string GetActionName()
    {
        return "Grenade";
    }
    private void Update()
    {
        if (!isActive)
        {
            return;
        }
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxThrowDistance; x <= maxThrowDistance; x++)
        {
            for (int z = -maxThrowDistance; z <= maxThrowDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxThrowDistance)
                {
                    continue;
                }

                if (!PathFinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    //check when click on obstacles grid.
                    continue;
                }
                if (!PathFinding.Instance.HasPath(unitGridPosition, testGridPosition))
                {
                    //no obstacles but also no path to reach destination.
                    continue;
                }

                // test shoot range with visual.
                /*  validGridPositionList.Add(testGridPosition);
                  continue;
                */

                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action  onActionComplete)
    {
        Transform grenadeProjectileTransform = Instantiate(grenadeProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
        GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();
        grenadeProjectile.Setup(gridPosition, OnGrenadeBehaviourComplete);
        ActionStart(onActionComplete);
    }

    protected override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    private void OnGrenadeBehaviourComplete()
    {
        onActionComplete();
    }

    public int GetMaxThrowDistance()
    {
        return maxThrowDistance;
    }
}
