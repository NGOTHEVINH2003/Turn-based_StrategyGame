using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UIElements;

public class Unit : MonoBehaviour
{
    private const int ACTION_POINTS_MAX = 4;
    private const int ACTION_POINTS_ADD = 3;


    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    [SerializeField] private bool isEnemy;


    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private int actionPoints = 3;
    private BaseAction[] baseActionArray;


    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        baseActionArray = GetComponents<BaseAction>();
    }


    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPos(gridPosition, this);

        TurnSystem.Instance.OnTurnEnd += TurnSystem_OnTurnEnd;
        healthSystem.OnDead += HealthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);

    }

   
    private void Update()
    {

        //update unit position
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if(newGridPosition != gridPosition)
        {
            GridPosition oldGridPosition = gridPosition;
           
            gridPosition = newGridPosition;

            LevelGrid.Instance.UnitMovePos(this, oldGridPosition, newGridPosition);
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

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void TurnSystem_OnTurnEnd(object sender, EventArgs e)
    {
        if(
            (IsEnemy() && !TurnSystem.Instance.IsPlayerTurn())
            || (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn())
          )
        {
            actionPoints += ACTION_POINTS_ADD;
            if (actionPoints > ACTION_POINTS_MAX) actionPoints = ACTION_POINTS_MAX;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
       
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPos(gridPosition, this);
        Destroy(gameObject);

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
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


    public T GetAction<T>() where T : BaseAction
    {
        foreach(BaseAction baseAction in baseActionArray)
        {
            if(baseAction is T)
            {
                return (T)baseAction;
            }
        }
        return null;
    }

    public int GetActionPoints()
    {
        return actionPoints;
    }


    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public BaseAction[] GetBaseActionsArray()
    {
        return baseActionArray;
    } 
    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }
    public void Heal(int healthAmount)
    {
        healthSystem.Heal(healthAmount);
    }


    public float GetHealthNormalized()
    {
        return healthSystem.GetHealthNormalized();
    }
  

}
