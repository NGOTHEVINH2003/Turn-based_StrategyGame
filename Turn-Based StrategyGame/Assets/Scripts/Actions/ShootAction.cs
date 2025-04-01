using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    [SerializeField] private int damage = 40;

    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    private enum State
    {
        Aiming, Shooting, CoolOff,
    }
    private int maxShootDistance = 5;
    private State state;
    private float stateTimer;
    private Unit targetUnit;
    private bool canShootBullet;


    void Update()
    {
        if (!isActive)
        {
            return;
        }
        stateTimer-= Time.deltaTime;
        switch (state)
        {
            case State.Aiming:
                Vector3 aimDir = (targetUnit.getWorldPosition() - unit.getWorldPosition()).normalized;
                float rotateSpped = 30f;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * rotateSpped);

                break;
            case State.Shooting:
                if (canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            case State.CoolOff:
                break;
        }

        if(stateTimer <= 0f)
        {
            NextState();
        }

    }

    private void Shoot()
    {
        OnShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = targetUnit,
            shootingUnit = unit
        }) ;
        targetUnit.Damage(damage);
    }

    private void NextState()
    {
        switch (state)
        {
            case State.Aiming:
                state = State.Shooting;
                float shootingStateTime = 0.1f;
                stateTimer = shootingStateTime;
                break;
            case State.Shooting:
                state = State.CoolOff;
                float coolOffStateTime = 0.5f;
                stateTimer = coolOffStateTime;
                break;
            case State.CoolOff:
                ActionComplete();
                break;
        }
        Debug.Log(state);
    }

    public override string GetActionName()
    {
        return "Shoot";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition);
    }
    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue; 
                }
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if(testDistance > maxShootDistance)
                {
                    continue;
                }
                // test shoot range with visual.
              /*  validGridPositionList.Add(testGridPosition);
                continue;
*/ 
                if (!LevelGrid.Instance.UnitExistOnThisGrid(testGridPosition))
                {
                    //Grid is Empty, no unit on grid
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if(targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    //check for same Team
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;

    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    { 

        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = State.Aiming;
        float amingStateTime = 1f;
        stateTimer = amingStateTime;
        canShootBullet = true;

        ActionStart(onActionComplete);

    }

    public Unit GetTargetedUnit()
    {
        return targetUnit;
    }

    public int GetMaxShootDistanceRange()
    {
        return maxShootDistance;
    }

    protected override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit =  LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 100 + Mathf.RoundToInt((1-targetUnit.GetHealthNormalized()) * 100f),
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }
}

